using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieStore.API.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MovieStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public AccountsController(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(User user, string password)
        {
            if (user == null)
            {
                return BadRequest();
            }
            
            this.CreatePasswordHash(password, out byte[] passwordSalt, out byte[] passwordHash);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            //var currentUser = _dbContext.Users.FirstOrDefault
            //    (x => x.Username == login.Username && EF.Functions.Collate(x.Password, "SQL_Latin1_General_CP1_CS_AS") == login.Password);

            var currentUser = _dbContext.Users.FirstOrDefault(x => x.Username == login.Username);
           
            if (currentUser == null)
            {
                return BadRequest("Invalid Username");
            }

            var isValidPassword = VerifyPassword(login.Password, currentUser.PasswordSalt, currentUser.PasswordHash);

            if (!isValidPassword)
            {
                return BadRequest("Invalid Password");
            }

            var token = GenerateToken(currentUser);

            if (token == null)
            {
                return NotFound("Invalid credentials");
            }

            return Ok(token);
        }

        [NonAction]
        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var myClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Email, user.EmailId),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken(issuer: _configuration["JWT:issuer"],
                                             audience: _configuration["JWT:audience"],
                                             claims: myClaims,
                                             expires: DateTime.Now.AddDays(1),
                                             signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("GetName"), Authorize]
        public IActionResult GetName()
        {
            var FullName = User.FindFirstValue(ClaimTypes.GivenName);
            return Ok(FullName);
        }

        [NonAction]
        public void CreatePasswordHash(string password, out byte[] passwordSalt, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        [NonAction]
        public bool VerifyPassword(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return hash.SequenceEqual(passwordHash);
            }
        }
    }
}

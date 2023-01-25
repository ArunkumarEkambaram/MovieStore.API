using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieStore.API.Models;
using MovieStore.API.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IRepository<Movie> _repository;
        private readonly IMovieRepository _movieRepository;
        private readonly IGetRepository<MovieDto> _movieDtoRepository;

        public MoviesController(IRepository<Movie> repository, IMovieRepository movieRepository, IGetRepository<MovieDto> movieDtoRepository)
        {
            _repository = repository;
            _movieRepository = movieRepository;
            _movieDtoRepository = movieDtoRepository;
        }

        [HttpGet("GetAllMovies")]
        public IEnumerable<MovieDto> GetMovies()
        {
            return _movieDtoRepository.GetAll();
        }

        [HttpGet]
        [Route("GetMovieById/{id}", Name = "GetMovieById")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var movie = await _movieDtoRepository.GetById(id);
            if (movie != null)
            {
                return Ok(movie);
            }
            return NotFound();
        }

        [HttpGet("SearchMovie/{genreName}")]
        public async Task<IActionResult> SearchMovieByGenre(string genreName)
        {
            var result = await _movieRepository.SearchByGenre(genreName);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound("Please provide valid genre");
        }

        [Authorize(Roles ="admin, employee")]
        [HttpPost("CreateMovie")]
        public async Task<IActionResult> CreateMovie([FromBody] Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _repository.Create(movie);
            return CreatedAtRoute("GetMovieById", new { id = movie.Id }, movie);
        }

        [Authorize(Roles = "admin, employee")]
        [HttpPut("UpdateMovie/{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _repository.Update(id, movie);
            if (result != null)
            {
                return NoContent();
            }
            return NotFound("Movie not found");
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteMovie/{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var result = await _repository.Delete(id);
            if (result != null)
            {
                return Ok();
            }
            return NotFound("Movie not found");
        }

        [HttpGet("GetGenres")]
        //Get Genres
        public async Task<IActionResult> GetGenres()
        {
            var gernes = await _movieRepository.GetGenres();
            return Ok(gernes);
        }
    }

}

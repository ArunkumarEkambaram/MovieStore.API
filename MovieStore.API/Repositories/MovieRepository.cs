using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MovieStore.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStore.API.Repositories
{
    public class MovieRepository : IRepository<Movie>, IGetRepository<MovieDto>, IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(Movie obj)
        {
            if (obj != null)
            {
                _context.Movies.Add(obj);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Movie> Update(int id, Movie obj)//api/Movies/Update/1
        {
            var movieInDb = await _context.Movies.FindAsync(id);
            if (movieInDb != null)
            {
                movieInDb.MovieName = obj.MovieName;
                movieInDb.ProductionName = obj.ProductionName;
                movieInDb.ReleaseDate = obj.ReleaseDate;
                movieInDb.GenreId = obj.GenreId;
                _context.Movies.Update(movieInDb);
                await _context.SaveChangesAsync();
                return movieInDb;
            }
            return null;
        }

        public async Task<Movie> Delete(int id)
        {
            var movieInDb = await _context.Movies.FindAsync(id);
            if (movieInDb != null)
            {
                _context.Movies.Remove(movieInDb);
                await _context.SaveChangesAsync();
                return movieInDb;
            }
            return null;
        }

        public IEnumerable<MovieDto> GetAll()
        {
            var movies = _context.Movies.Include(m => m.Genre).Select(x => new MovieDto
            {
                Id = x.Id,
                MovieName = x.MovieName,
                ProductionName = x.ProductionName,
                ReleaseDate = x.ReleaseDate,
                GenreId = x.GenreId,
                GenreName = x.Genre.GenreName
            }).ToList();
            return movies;
        }

        public async Task<MovieDto> GetById(int id)
        {
            var movies = await _context.Movies.Include(x => x.Genre).Select(x => new MovieDto
            {
                Id = x.Id,
                MovieName = x.MovieName,
                ProductionName = x.ProductionName,
                ReleaseDate = x.ReleaseDate,
                GenreId = x.GenreId,
                GenreName = x.Genre.GenreName
            }).ToListAsync();

            var movie  = movies.FirstOrDefault(x=>x.Id==id);

            if (movie != null)
            {
                return movie;
            }
            return null;
        }

        public async Task<IEnumerable<Movie>> SearchByGenre(string genreName)
        {
            if (!string.IsNullOrWhiteSpace(genreName))
            {
                var movies = await _context.Movies.Include(x => x.Genre).Where(x => x.Genre.GenreName.Contains(genreName)).ToListAsync();
                return movies;
            }
            return null;
        }

        //Get Genres
        public async Task<IEnumerable<Genre>> GetGenres()
        {
            var genres = await _context.Genres.ToListAsync();
            return genres;
        }
    }
}

using ITB2203Application.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ITB2203Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly DataContext _context;

        public MoviesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public ActionResult<Movie> GetMovie(int id)
        {
            var movie = _context.Movies!.Find(id);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Movie>> GetMovies()
        {
            var query = _context.Movies!.AsQueryable().ToList();

            return query;
        }

        [HttpPut("{id}")]
        public IActionResult PutMovie(int id, Movie movie)
        {
            var dbMovie = _context.Movies!.AsNoTracking().FirstOrDefault(x => x.Id == movie.Id);
            if (id != movie.Id)
            {
                return BadRequest();
            }
            if (dbMovie == null)
            {
                return NotFound();
            }

            _context.Update(movie);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public ActionResult<Movie> PostMovie(Movie movie)
        {
            if (_context.Movies!.Any())
            {
                int maxMovieId = _context.Movies!.Max(x => x.Id);
                movie.Id = maxMovieId + 1;
            }

            _context.Add(movie);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetMovie), new { Id = movie.Id }, movie);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            var movie = _context.Movies!.Find(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Remove(movie);
            _context.SaveChanges();

            return Ok();
        }
    }
}
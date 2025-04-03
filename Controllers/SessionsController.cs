using ITB2203Application.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace ITB2203Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly DataContext _context;

        public SessionsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public ActionResult<Session> GetSession(int id)
        {
            var session = _context.Sessions!.Find(id);

            if (session == null)
            {
                return NotFound();
            }

            return Ok(session);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Session>> GetSessions(DateTime? periodStart, DateTime? periodEnd, string? movieTitle)
        {
            var query = _context.Sessions!.AsQueryable().ToList();

            if (periodStart != null && periodEnd != null)
            {
                List<Session> validTimeSessions = new List<Session>();
                foreach (Session session in query)
                {
                    int dateComparisonStart = DateTime.Compare((DateTime)periodStart, session.StartTime);
                    int dateComparisonEnd = DateTime.Compare((DateTime)periodEnd, session.StartTime);
                    if (dateComparisonStart <= 0 && dateComparisonEnd >= 0)
                    {
                        validTimeSessions.Add(session);
                    }
                }
                return validTimeSessions;
            }
            else if (periodStart != null)
            {
                List<Session> validTimeSessions = new List<Session>();
                foreach (Session session in query)
                {
                    int dateComparison = DateTime.Compare((DateTime)periodStart, session.StartTime);
                    if (dateComparison <= 0)
                    {
                        validTimeSessions.Add(session);
                    }
                }
                return validTimeSessions;
            }
            else if (periodEnd != null)
            {
                List<Session> validTimeSessions = new List<Session>();
                foreach (Session session in query)
                {
                    int dateComparisonEnd = DateTime.Compare((DateTime)periodEnd, session.StartTime);
                    if (dateComparisonEnd >= 0)
                    {
                        validTimeSessions.Add(session);
                    }
                }
                return validTimeSessions;
            }
            if (movieTitle != null)
            {
                List<Session> validMovieTitleSessions = new List<Session>();
                foreach (Session session in query)
                {
                    Movie? sessionMovie = _context.Movies!.AsNoTracking().FirstOrDefault(x => x.Id == session.MovieId);
                    if (sessionMovie!.Title == movieTitle)
                    {
                        validMovieTitleSessions.Add(session);
                    }
                }
                return validMovieTitleSessions;
            }
            else
            {
                return query;
            }
        }

        [HttpGet]
        [Route("{id}/tickets")]
        public ActionResult<IEnumerable<Ticket>> GetSessionTickets(int id)
        {
            var session = _context.Sessions!.Find(id);

            if (session == null)
            {
                return NotFound();
            }

            List<Ticket>? sessionTickets = _context.Tickets!.AsNoTracking().Where(x => x.SessionId == session.Id).ToList();

            if (sessionTickets == null)
            {
                return NotFound();
            }

            return Ok(sessionTickets);
        }


        [HttpPut("{id}")]
        public IActionResult PutSession(int id, Session session)
        {
            var dbSession = _context.Sessions!.AsNoTracking().FirstOrDefault(x => x.Id == session.Id);
            if (id != session.Id)
            {
                return BadRequest();
            }
            if (dbSession == null)
            {
                return NotFound();
            }

            DateTime currentTime = DateTime.Now;
            int dateComparison = DateTime.Compare(session.StartTime, currentTime);

            if (dateComparison <= 0)
            {
                return BadRequest();
            }

            Movie? sessionMovie = _context.Movies!.AsNoTracking().FirstOrDefault(x => x.Id == session.MovieId);
            if (sessionMovie == null)
            {
                return BadRequest();
            }

            _context.Update(session);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public ActionResult<Session> PostSession(Session session)
        {
            if (_context.Sessions!.Any())
            {
                int maxSessionId = _context.Sessions!.Max(x => x.Id);
                session.Id = maxSessionId + 1;
            }

            DateTime currentTime = DateTime.Now;
            int dateComparison = DateTime.Compare(session.StartTime, currentTime);

            if (dateComparison <= 0)
            {
                return BadRequest();
            }

            Movie? sessionMovie = _context.Movies!.AsNoTracking().FirstOrDefault(x => x.Id == session.MovieId);
            if (sessionMovie == null)
            {
                return BadRequest();
            }

            _context.Add(session);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetSession), new { Id = session.Id }, session);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSession(int id)
        {
            var session = _context.Sessions!.Find(id);
            if (session == null)
            {
                return NotFound();
            }

            _context.Remove(session);
            _context.SaveChanges();

            return Ok();
        }
    }
}
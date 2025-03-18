using ITB2203Application.Model;
using ITB2203Application.Model.FilmModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly DataContext _context;

    public SessionsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Session>> GetTicket(DateTime? periodStart = null, DateTime? periodEnd = null, string? AuditoriumName = null)
    {
        var query = _context.Sessions!.AsQueryable();
        var Session_periodStart = periodStart;
        var Session_periodEnd = periodEnd;


        if (AuditoriumName != null)
            query = query.Where(x => x.AuditoriumName != null && x.AuditoriumName.ToUpper().Contains(AuditoriumName.ToUpper()));

        if (Session_periodStart != null)
            query = query.Where(x => x.StartTime != null && x.StartTime.Equals(Session_periodStart));
        
        if (Session_periodEnd != null)
            query = query.Where(x => x.StartTime != null && x.StartTime.Equals(Session_periodEnd));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<Session> GetSession(int id, string AuditoriumName, DateTime? periodStart = null, DateTime? periodEnd = null)
    {
        var Session_Objects = _context.Sessions!.Find(id, AuditoriumName, periodStart, periodEnd);

        if (Session_Objects == null)
        {
            return NotFound();
        }

        return Ok(Session_Objects);
    }

    [HttpPut("{id}")]
    public IActionResult PutSession(int id, Session session)
    {
        var dbSession = _context.Tickets!.AsNoTracking().FirstOrDefault(x => x.Id == session.Id);
        if (id != session.Id || dbSession == null)
        {
            return NotFound();
        }

        _context.Update(session);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Session> PostSession(Session session, DateTime? periodEnd = null, DateTime? periodStart = null)
    {
        if (periodEnd != null)
        {
            return BadRequest("404 (Not Found)");
        }

        var dbSession = _context.Sessions!.Find(session.Id);
        if (dbSession == null)
        {
            var dbSessionStart = _context.Sessions.FirstOrDefault(a => a.StartTime >= periodStart);
            if (dbSessionStart != null)
            {
                return BadRequest("404 (Not Found)");
            }

            var dbSessionEnd = _context.Sessions.FirstOrDefault(a => a.StartTime <= periodEnd);
            if (dbSessionEnd != null)
            {
                return BadRequest("404 (Not Found)");
            }

            _context.Add(session);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetSession), new { Id = session.Id }, session);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSession(int id, string AuditoriumName, DateTime StartTime)
    {
        var session = _context.Sessions!.Find(id, AuditoriumName, StartTime);

        if (session == null)
        {
            return NotFound();
        }

        _context.Remove(session);
        _context.SaveChanges();

        return NoContent();
    }
}

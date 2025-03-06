using ITB2203Application.Model;
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
    public ActionResult<IEnumerable<Session>> GetTicket(DateTime StartTime, string? AuditoriumName = null)
    {
        var query = _context.Sessions!.AsQueryable();
        var Session_StartTime = _context.Sessions!.Find(StartTime);

        if (AuditoriumName != null)
            query = query.Where(x => x.AuditoriumName != null && x.AuditoriumName.ToUpper().Contains(AuditoriumName.ToUpper()));

        if (Session_StartTime != null)
            query = query.Where(x => x.StartTime != null && x.StartTime.Equals(Session_StartTime));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<Session> GetSession(int id, string AuditoriumName, DateTime StartTime)
    {
        var Session_Objects = _context.Sessions!.Find(id, AuditoriumName, StartTime);

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
    public ActionResult<Session> PostSession(Session session)
    {
        var dbSessionMarking = _context.Sessions!.Find(session.Id);
        if (dbSessionMarking == null)
        {
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

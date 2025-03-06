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
public class TicketsController : ControllerBase
{
    private readonly DataContext _context;

    public TicketsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Ticket>> GetTicket(int? Price = null, string? SeatNo = null)
    {
        var query = _context.Tickets!.AsQueryable();
        if (SeatNo != null)
            query = query.Where(x => x.SeatNo != null && x.SeatNo.ToUpper().Contains(SeatNo.ToUpper()));

        var ticket_Price = _context.Tickets!.Find(Price);
        if (ticket_Price != null)
            query = query.Where(x => x.Price != null && x.Price.Equals(ticket_Price));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<Ticket> GetTicket(int id, string SeatNo, decimal Price)
    {
        var ticket_Objects = _context.Tickets!.Find(id, SeatNo, Price);

        if (ticket_Objects == null)
        {
            return NotFound();
        }

        return Ok(ticket_Objects);
    }

    [HttpPut("{id}")]
    public IActionResult PutTicket(int id, Ticket ticket)
    {
        var dbTicket = _context.Tickets!.AsNoTracking().FirstOrDefault(x => x.Id == ticket.Id);
        if (id != ticket.Id || dbTicket == null)
        {
            return NotFound();
        }

        _context.Update(ticket);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Ticket> PostTicket(Ticket ticket)
    {
        var dbTicketMarking = _context.Tickets!.Find(ticket.Id);
        if (dbTicketMarking == null)
        {
            _context.Add(ticket);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTicket), new { Id = ticket.Id }, ticket);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTicket(int id, string SeatNo, decimal Price)
    {
        var ticket = _context.Tickets!.Find(id, SeatNo, Price);

        if (ticket == null)
        {
            return NotFound();
        }

        _context.Remove(ticket);
        _context.SaveChanges();

        return NoContent();
    }
}

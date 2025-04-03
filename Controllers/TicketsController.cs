using ITB2203Application.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ITB2203Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly DataContext _context;

        public TicketsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public ActionResult<Ticket> GetTicket(int id)
        {
            var ticket = _context.Tickets!.Find(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Ticket>> GetTickets()
        {
            var query = _context.Tickets!.AsQueryable();

            return query.ToList();
        }

        [HttpPut("{id}")]
        public IActionResult PutTicket(int id, Ticket ticket)
        {
            var dbTicket = _context.Tickets!.AsNoTracking().FirstOrDefault(x => x.Id == ticket.Id);
            if (id != ticket.Id)
            {
                return BadRequest();
            }
            if (dbTicket == null)
            {
                return NotFound();
            }
            if (_context.Tickets!.Any())
            {
                List<Ticket> tickets = _context.Tickets!.AsQueryable().ToList();

                foreach (Ticket dbTicketNew in tickets)
                {
                    if (dbTicketNew.SeatNo == ticket.SeatNo && dbTicketNew.SessionId == ticket.SessionId)
                    {
                        return BadRequest();
                    }
                }
            }

            Session? ticketSession = _context.Sessions!.AsNoTracking().FirstOrDefault(x => x.Id == ticket.SessionId);
            if (ticketSession == null)
            {
                return BadRequest();
            }

            if (ticket.Price <= 0)
            {
                return BadRequest();
            }

            _context.Update(ticket);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public ActionResult<Ticket> PostTicket(Ticket ticket)
        {
            if (_context.Tickets!.Any())
            {
                int maxTicketId = _context.Tickets!.Max(x => x.Id);
                ticket.Id = maxTicketId + 1;
                List<Ticket> tickets = _context.Tickets!.AsQueryable().ToList();

                foreach (Ticket dbTicket in tickets)
                {
                    if (dbTicket.SeatNo == ticket.SeatNo && dbTicket.SessionId == ticket.SessionId)
                    {
                        return BadRequest();
                    }
                }
            }

            if (ticket.Price <= 0)
            {
                return BadRequest();
            }

            Session? ticketSession = _context.Sessions!.AsNoTracking().FirstOrDefault(x => x.Id == ticket.SessionId);
            if (ticketSession == null)
            {
                return BadRequest();
            }

            _context.Add(ticket);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTicket), new { Id = ticket.Id }, ticket);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTicket(int id)
        {
            var ticket = _context.Tickets!.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Remove(ticket);
            _context.SaveChanges();

            return Ok();
        }
    }
}
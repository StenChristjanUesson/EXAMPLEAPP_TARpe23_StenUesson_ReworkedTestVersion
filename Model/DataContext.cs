using ITB2203Application.Model.FilmModels;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Model;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    { }

    public DbSet<Ticket>? Tickets { get; set; }
    public DbSet<Movie>? Movies { get; set; }
    public DbSet<Session>? Sessions { get; set; }
}

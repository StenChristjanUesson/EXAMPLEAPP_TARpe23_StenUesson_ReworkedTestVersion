﻿namespace ITB2203Application.Model.FilmModels
{
    public class Ticket
    {
        public int Id { get; set; }
        public int? SessionId { get; set; }
        public string? SeatNo { get; set; }
        public decimal? Price { get; set; }
    }
}

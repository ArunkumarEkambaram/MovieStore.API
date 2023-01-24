using System;

namespace MovieStore.API.Models
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string ProductionName { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }
}

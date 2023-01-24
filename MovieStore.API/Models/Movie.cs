using System;

namespace MovieStore.API.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string ProductionName { get; set; }
        public DateTime ReleaseDate { get; set; }

        //Navigation Property
        public Genre Genre { get; set; }

        //Foreign Key
        public int GenreId { get; set; }
    }
}

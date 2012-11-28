using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ARS.Models
{
    [Table("BookedFlights")]
    public class BookedFlights
    {

        public int ID { get; set; }
        public String FlightNumber { get; set; }
        public string Destination { get; set; }
        public string Origin { get; set; }
        public String Date { get; set; }
        public String DepartureTime { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public string BookedBy { get; set; }
        public int Seats { get; set; }
        public String Class { get; set; }


    }

   /* public class BookedFlightDBContext : DbContext
    {
        public DbSet<BookedFlights> BookedFlights { get; set; }
    }*/
}
using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ARS.Models
{
    [Table("Flights")]
    public class Flight
    {
        public int ID { get; set; }
        public String FlightNumber { get; set; }
        public string Destination { get; set; }
        public string Origin { get; set; }
        public String Date { get; set; }
        public String DepartureTime { get; set; }
        public string Type { get; set; }
        public decimal EconomyClassPrice { get; set; }
        public decimal BusinessClassPrice { get; set; }
        public int EconomyCapacity { get; set; }
        public int EconomySeatsBooked { get; set; }
        public int BusinessCapacity { get; set; }
        public int BusinessSeatsBooked { get; set; }


        public int EconomySeatsAvailable
        {
            get
            {
                return this.EconomyCapacity - this.EconomySeatsBooked;
            }
        }
        public int BusinessSeatsAvailable
        {
            get
            {
                return this.BusinessCapacity - this.BusinessSeatsBooked;
            }
        }
    }
   /* public class FlightDBContext : DbContext
    {
        public DbSet<Flight> Flights { get; set; }
    }*/


}
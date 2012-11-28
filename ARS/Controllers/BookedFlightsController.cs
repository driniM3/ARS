using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ARS.Models;

namespace ARS.Controllers
{
    public class BookedFlightsController : Controller
    {
        private UsersContext db = new UsersContext();
        private BookedFlights flight = new BookedFlights();
        private Flight unBooked = new Flight();

        //
        // GET: /BookedFlights/

        public ActionResult Index()
        {
            return View(db.BookedFlights.ToList());
        }

        
        //
        // GET: /BookedFlights/Details/5

        public ActionResult Details(int id = 0)
        {
            
            BookedFlights bookedflights = db.BookedFlights.Find(id);
            if (bookedflights == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != bookedflights.BookedBy && User.Identity.Name != "admin")
            {
                return RedirectToAction("Forbidden");
            }
          
            return View(bookedflights);
        }

       

        [HttpPost]
        public ActionResult Book(int id, string bookSeats, string typeClass)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Forbidden");
            }

            unBooked = db.Flights.Find(id);
            int seats;
            int.TryParse(bookSeats, out seats);

            if (seats == 0) {
                return RedirectToAction("ErrorBooking");
            }

            else if(typeClass.Equals("economy",StringComparison.CurrentCultureIgnoreCase) && seats <= unBooked.EconomySeatsAvailable){
                flight.Date = unBooked.Date;
                flight.DepartureTime = unBooked.DepartureTime;
                flight.Destination = unBooked.Destination;
                flight.FlightNumber = unBooked.FlightNumber;
                flight.Origin = unBooked.Origin;
                flight.Price = unBooked.EconomyClassPrice * seats;
                flight.Type = unBooked.Type;
                flight.Seats = seats;
                flight.Class = typeClass;
                flight.BookedBy = User.Identity.Name;

                unBooked.EconomySeatsBooked += seats;
            }

            else if (typeClass.Equals("business", StringComparison.CurrentCultureIgnoreCase) && seats <= unBooked.BusinessSeatsAvailable)
            {
                flight.Date = unBooked.Date;
                flight.DepartureTime = unBooked.DepartureTime;
                flight.Destination = unBooked.Destination;
                flight.FlightNumber = unBooked.FlightNumber;
                flight.Origin = unBooked.Origin;
                flight.Price = unBooked.BusinessClassPrice * seats;
                flight.Type = unBooked.Type;
                flight.Seats = seats;
                flight.Class = typeClass;
                flight.BookedBy = User.Identity.Name;

                unBooked.BusinessSeatsBooked += seats;
            }

            else
            {
                return RedirectToAction("ErrorBooking");
            }
            

            if (ModelState.IsValid)
            {
                db.BookedFlights.Add(flight);
                db.SaveChanges();

                db.Entry(unBooked).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }


            return RedirectToAction("ErrorBooking");
        }

        public ActionResult ErrorBooking()
        {
            return View();
        }


        //
        // GET: /BookedFlights/Delete/5

        public ActionResult Delete(int id = 0)
        {
            BookedFlights bookedflights = db.BookedFlights.Find(id);
            if (bookedflights == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != bookedflights.BookedBy && User.Identity.Name != "admin")
            {
                return RedirectToAction("Forbidden");
            }

           

            if (bookedflights == null)
            {
                return HttpNotFound();
            }
            return View(bookedflights);
        }

        //
        // POST: /BookedFlights/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            BookedFlights bookedflights = db.BookedFlights.Find(id);
            if (bookedflights == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != bookedflights.BookedBy && User.Identity.Name != "admin")
            {
                return RedirectToAction("Forbidden");
            }
            db.BookedFlights.Remove(bookedflights);
            db.SaveChanges();

            var flights = from m in db.Flights
                          select m;
            if (bookedflights.FlightNumber != null)
            {
                Flight raw = flights.Where(x => x.FlightNumber == bookedflights.FlightNumber).Single();
                if (bookedflights.Class.Equals("economy", StringComparison.CurrentCultureIgnoreCase))
                {
                    raw.EconomySeatsBooked -= bookedflights.Seats;
                }
                else
                {
                    raw.BusinessSeatsBooked -= bookedflights.Seats;
                }
                db.Entry(raw).State = EntityState.Modified;
                db.SaveChanges();

            }

            return RedirectToAction("Index");

        }

        public ActionResult Forbidden()
        {
            return View();
        }

        public ActionResult bookLog()
        {
            if (User.Identity.Name.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
            {
                return View(db.BookedFlights.ToList());
            }
            return RedirectToAction("Forbidden");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
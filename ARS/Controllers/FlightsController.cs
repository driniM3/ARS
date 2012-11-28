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
    public class FlightsController : Controller
    {
        private UsersContext db = new UsersContext();
        //private BookedFlightDBContext bdb = new BookedFlightDBContext();

        //
        // GET: /Flights/

        public ActionResult Index()
        {
            return View(db.Flights.ToList());
        }

        public ActionResult Search(string origin, string destination, string date, string type,string flightNumber)
        {

            var originLst = new List<string>();

            var originQry = from o in db.Flights
                            orderby o.Origin
                            select o.Origin;
            originLst.AddRange(originQry.Distinct());
            ViewBag.origin = new SelectList(originLst);

            var destLst = new List<string>();

            var destQry = from d in db.Flights
                          orderby d.Destination
                          select d.Destination;
            destLst.AddRange(destQry.Distinct());
            ViewBag.destination = new SelectList(destLst);

            var flights = from m in db.Flights
                          select m;

           

            return View(flights.Where(x => x.Origin.Contains(origin) &&
                                       x.FlightNumber.Contains(flightNumber) &&
                                       x.Destination.Contains(destination) &&
                                       x.Type.Contains(type) &&
                                       x.Date.Contains(date)));


        }

        //
        // GET: /Flights/Details/5

        public ActionResult Details(int id = 0)
        {
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            {
                return HttpNotFound();
            }
            return View(flight);
        }

        //
        // GET: /Flights/Create

        public ActionResult Create()
        {
            if (User.Identity.Name != "admin")
            {
                return RedirectToAction("Forbidden");
            }
            return View();
        }

        //
        // POST: /Flights/Create

        [HttpPost]
        public ActionResult Create(Flight flight)
        {
            if (User.Identity.Name != "admin")
            {
                return RedirectToAction("Forbidden");
            }
            if (ModelState.IsValid)
            {
                var flights = from m in db.Flights
                              select m;
                int count = flights.Where(x => x.FlightNumber == flight.FlightNumber).Count();
                if (count == 0)
                {
                    db.Flights.Add(flight);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("NumberConflict");
                }
            }

            return View(flight);
        }

        public ActionResult NumberConflict()
        {
            return View();
        }

        //
        // GET: /Flights/Edit/5

        public ActionResult Edit(int id = 0)
        {
            
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != "admin")
            {
                return RedirectToAction("Forbidden");
            }

            return View(flight);
        }

        //
        // POST: /Flights/Edit/5

        [HttpPost]
        public ActionResult Edit(Flight flight)
        {
            if (User.Identity.Name != "admin")
            {
                return RedirectToAction("Forbidden");
            }
            if (flight.BusinessCapacity < flight.BusinessSeatsBooked || flight.EconomyCapacity < flight.EconomySeatsBooked)
            {
                return RedirectToAction("CapacityError");
            }
            List<BookedFlights> raw = new List<BookedFlights>();

           
            var bookedFlights = from m in db.BookedFlights
                                select m;
            raw = bookedFlights.Where(x => x.FlightNumber == flight.FlightNumber).ToList();
            foreach (BookedFlights z in raw)
            {
                z.Origin = flight.Origin;
                z.Destination = flight.Destination;
                z.Date = flight.Date;
                z.DepartureTime = flight.DepartureTime;
                z.Type = flight.Type;
                if(z.Class.Equals("economy",StringComparison.CurrentCultureIgnoreCase)){
                    z.Price = z.Seats * flight.EconomyClassPrice;
                }
                else{
                    z.Price = z.Seats * flight.BusinessClassPrice;
                }
                db.Entry(z).State = EntityState.Modified;
                
            }

            if (ModelState.IsValid)
            {
                db.Entry(flight).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            return View(flight);
        }

        public ActionResult CapacityError()
        {
            return View();
        }

        public ActionResult Forbidden()
        {
            return View();
        }

        //
        // GET: /Flights/Delete/5

        public ActionResult Delete(int id = 0)
        {
            
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != "admin")
            {
                return RedirectToAction("Forbidden");
            }
            return View(flight);
        }

        //
        // POST: /Flights/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (User.Identity.Name != "admin")
            {
                return RedirectToAction("Forbidden");
            }
            
            List<BookedFlights> raw = new List<BookedFlights>();
            Flight flight = db.Flights.Find(id);
            db.Flights.Remove(flight);
            db.SaveChanges();

            var bookedFlights = from m in db.BookedFlights
                          select m;
            raw = bookedFlights.Where(x => x.FlightNumber == flight.FlightNumber &&
                                            x.Origin == flight.Origin &&
                                            x.Destination == flight.Destination &&
                                            x.Date == flight.Date &&
                                            x.DepartureTime == flight.DepartureTime).ToList();
            foreach (BookedFlights z in raw){
                db.BookedFlights.Remove(z);
            }

            db.SaveChanges();
                

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
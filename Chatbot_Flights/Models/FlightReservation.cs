 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot_Flights
{
    public class FlightReservation
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }
        public string Nick { get; set; }
        public string BirthDate { get; set; }
        public string AirPortFrom { get; set; }
        public string AirPortTo { get; set; }
        public string StartDate { get; set; }
        public string OneWayFlight { get; set; }
        public string FlightReturn { get; set; }
        public string ReturnDate { get; set; }
        public string FlightClass { get; set; }
        public string FlightCost { get; set; }
        public string ReservationNumber { get; set; }

        //CarReservation
        public string RentCar { get; set; }
        public string RentBookNr { get; set; }
        public string CarSize { get; set; }
        public string CarClass { get; set; }
        public string Insurance { get; set; }

    }

}

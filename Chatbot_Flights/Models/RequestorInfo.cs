using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot_Flights.Models
{
    public class RequestorInfo
    {
        public string RequestorName { get; set; }
        public List<FlightReservation> Resevations { get; set; }
        public int ReservationIdToShow { get; set; }

        public RequestorInfo()
        {
            this.Resevations = new List<FlightReservation>();
        }
    }
}

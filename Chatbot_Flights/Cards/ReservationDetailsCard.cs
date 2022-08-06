using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Chatbot_Flights.Cards
{
    public static class ReservationDetailsCard
    {
        

        public static Attachment ReservationDetails (string name, string surename, string nick, string birthDate, string airportFrom,
            string airpotTo, string startDate, string oneWayFlight, string flightClass, string flightCost)
        {
           

            string adaptiveCardJson = File.ReadAllText(@".\Resources\reservationDetails.json");
            
            adaptiveCardJson = adaptiveCardJson.Replace("<Name>", name +" "+surename);
            adaptiveCardJson = adaptiveCardJson.Replace("<Nick>", nick);
            adaptiveCardJson = adaptiveCardJson.Replace("<Birthdate>", birthDate);
            adaptiveCardJson = adaptiveCardJson.Replace("<AirPortFrom>", airportFrom);
            adaptiveCardJson = adaptiveCardJson.Replace("<AirPortTo>", airpotTo);
            adaptiveCardJson = adaptiveCardJson.Replace("<StartDate>", startDate);
            adaptiveCardJson = adaptiveCardJson.Replace("<OnWayFlight>", oneWayFlight);
            adaptiveCardJson = adaptiveCardJson.Replace("<FlightClass>", flightClass);
            adaptiveCardJson = adaptiveCardJson.Replace("<FlightCost>", flightCost);

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }

        public static Attachment ReservationDetailsExtended(string name, string surename, string nick, string birthDate, string airportFrom,
            string airpotTo, string startDate, string oneWayFlight, string flightReturn, string returnDate, string flightClass, string flightCost)
        {


            string adaptiveCardJson = File.ReadAllText(@".\Resources\reservationDetailsExtended.json");

            adaptiveCardJson = adaptiveCardJson.Replace("<Name>", name + " " + surename);
            adaptiveCardJson = adaptiveCardJson.Replace("<Nick>", nick);
            adaptiveCardJson = adaptiveCardJson.Replace("<Birthdate>", birthDate);
            adaptiveCardJson = adaptiveCardJson.Replace("<AirPortFrom>", airportFrom);
            adaptiveCardJson = adaptiveCardJson.Replace("<AirPortTo>", airpotTo);
            adaptiveCardJson = adaptiveCardJson.Replace("<StartDate>", startDate);
            adaptiveCardJson = adaptiveCardJson.Replace("<OnWayFlight>", oneWayFlight);
            adaptiveCardJson = adaptiveCardJson.Replace("<FlightReturn>", flightReturn);
            adaptiveCardJson = adaptiveCardJson.Replace("<ReturnDate>", returnDate);
            adaptiveCardJson = adaptiveCardJson.Replace("<FlightClass>", flightClass);
            adaptiveCardJson = adaptiveCardJson.Replace("<FlightCost>", flightCost);

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }


        public static Attachment CarDetailsCard(string rentCar, string rentBookNr, string carSize, string carClass, string carInsurance)
        {

            string adaptiveCardJson = File.ReadAllText(@".\Resources\additionalInformation.json");
            
            adaptiveCardJson = adaptiveCardJson.Replace("<RentCar>", rentCar);
            adaptiveCardJson = adaptiveCardJson.Replace("<RentNumber>", rentBookNr);
            adaptiveCardJson = adaptiveCardJson.Replace("<CarSize>", carSize);
            adaptiveCardJson = adaptiveCardJson.Replace("<CarClass>", carClass);
            adaptiveCardJson = adaptiveCardJson.Replace("<CarInsurance>", carInsurance);

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }

    }
}

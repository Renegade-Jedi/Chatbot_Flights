using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Chatbot_Flights.Cards
{
    public class HelpHeroCard
    {
        public static HeroCard GetHeroCard(string heroCardVariant)
        {
            var heroCard = new HeroCard();
            switch (heroCardVariant)
            {
                case "AdditionalInformationDialog":
                    heroCard = new HeroCard
                    {

                        Title = "Help",
                        Subtitle = "Do you have problem with providing data? or you want visit our full reservation weeb page?",
                        Text = "Find cheap flights with the innovative Skyscanner search engine. Compare millions of airline and travel agent offers in seconds and book cheap airline tickets.",
                        Images = new List<CardImage> { new CardImage("https://www.skyscanner.pl/sttc/blackbird/opengraph_solid_logo.png") },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Go to our page", value: "https://www.skyscanner.pl/") },
                    };
                    return heroCard;
                case "BasicInfoDialog":
                    heroCard = new HeroCard
                    {

                        Title = "Help",
                        Subtitle = "Do you have problem with providing data? or you want visit our full reservation weeb page?",
                        Text = "Find cheap flights with the innovative Skyscanner search engine. Compare millions of airline and travel agent offers in seconds and book cheap airline tickets.",
                        Images = new List<CardImage> { new CardImage("https://www.skyscanner.pl/sttc/blackbird/opengraph_solid_logo.png") },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Go to our page", value: "https://www.skyscanner.pl/") },
                    };
                    return heroCard;
                case "CarReservationDialog":
                    heroCard = new HeroCard
                    {

                        Title = "Help",
                        Subtitle = "You have problem with choose car? Oo you want visit our full reservation weeb page?",
                        Text = "Find cheap flights with the innovative Skyscanner search engine. Compare millions of airline and travel agent offers in seconds and book cheap airline tickets.",
                        Images = new List<CardImage> { new CardImage("https://www.skyscanner.pl/sttc/blackbird/opengraph_solid_logo.png") },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Go to our page", value: "https://www.skyscanner.pl/") },
                    };
                    return heroCard;
                case "OneWayFlightDialog":
                    heroCard = new HeroCard
                    {

                        Title = "Help",
                        Subtitle = "You don't know which option to choose? Or you want visit our full reservation weeb page?",
                        Text = "Find cheap flights with the innovative Skyscanner search engine. Compare millions of airline and travel agent offers in seconds and book cheap airline tickets.",
                        Images = new List<CardImage> { new CardImage("https://www.skyscanner.pl/sttc/blackbird/opengraph_solid_logo.png") },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Go to our page", value: "https://www.skyscanner.pl/") },
                    };
                    return heroCard;
                case "ShowReservationDialog":
                    heroCard = new HeroCard
                    {

                        Title = "Help",
                        Subtitle = "You have problem with Show Reservation? Or you want visit our full reservation weeb page?",
                        Text = "Find cheap flights with the innovative Skyscanner search engine.Compare millions of airline and travel agent offers in seconds and book cheap airline tickets.",
                        Images = new List<CardImage> { new CardImage("https://www.skyscanner.pl/sttc/blackbird/opengraph_solid_logo.png") },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Go to our page", value: "https://www.skyscanner.pl/") },
                    };
                    return heroCard;
                case "TicketClass":
                    heroCard = new HeroCard
                    {

                        Title = "More Information about flight class.",
                        Text = "Standard: The distance between the rows is generally 75 cm and the width is 50 cm. \n" +
                        "Business: The distance between the rows is generally 90 cm and the width is 60 cm. Free coffe and tea. Check in priority. \n" +
                        "Premium: The distance between the rows is generally 95 cm and the width is 65 cm. Open bar, zone of silence, check in priority \n",
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Go to our page", value: "https://www.skyscanner.pl/") },
                    };
                    return heroCard;
                case "CarClass":
                    heroCard = new HeroCard
                    {

                        Title = "More Information about car class.",
                        Text = "Standard: air conditioning, car alarm . \n" +
                        "Premnium: air conditioning, car alarm, more horse power, cruise control. \n" +
                        "Lux: air conditioning, car alarm, more power, cruise control, leather seats, wi-fi \n",
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Go to our page", value: "https://www.rentalcars.com/pl/") },
                    };
                    return heroCard;
                case "CarSize":
                    heroCard = new HeroCard
                    {

                        Title = "More Information about car type.",
                        Text = "Hatchback: Smal car with a hatch-type rear door. \n" +
                        "Sedan: 4 door classic car best performance in our offer. \n" +
                        "SUV: Big 5 door car with 4x4 powertrain \n"+
                        "Van: The biggest car in our offer the possibility to carry 9 person  \n" +
                        "PickUp: small truck that has an open back with low sides \n",
                        Images = new List<CardImage> { new CardImage("https://www.lufthansa.com/content/dam/lh/images/local_images/ancillary/lh_anc_car_pano_XL_en.jpg") },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Go to our page", value: "https://www.rentalcars.com/pl/") },
                    };
                    return heroCard;
                case "CarInsurance":
                    heroCard = new HeroCard
                    {

                        Title = "More Information about car insurance.",
                        Text = "Standard: OC only. \n" +
                        "Business: OC and AC. \n" +
                        "Premium: OC, AC, NWW \n",
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Go to our page", value: "https://www.rentalcars.com/pl/") },
                    };
                    return heroCard;
                default:
                    heroCard = new HeroCard
                    {

                        Title = "Help",
                        Subtitle = "Do you want visit our full reservation weeb page?",
                        Text = "Find cheap flights with the innovative Skyscanner search engine. Compare millions of airline and travel agent offers in seconds and book cheap airline tickets.",
                        Images = new List<CardImage> { new CardImage("https://www.skyscanner.pl/sttc/blackbird/opengraph_solid_logo.png") },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Go to our page", value: "https://www.skyscanner.pl/") },
                    };
                    return heroCard;
            }   
        }
      
    }
}

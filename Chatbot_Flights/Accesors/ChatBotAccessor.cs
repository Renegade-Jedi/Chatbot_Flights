using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatbot_Flights.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Chatbot_Flights
{
    public class ChatBotAccessor
    {
        public const string ChatbotAccesorName = nameof(ChatBotAccessor);
        public const string FlightReservationAccessName = nameof(FlightReservation);
        public const string RequestorInfoAccessorName = nameof(RequestorInfo);

        public ConversationState ConversationState;
        public IStatePropertyAccessor<DialogState> ConversationDialogStateAccesor;
        public IStatePropertyAccessor<FlightReservation> FlightReservationAccessor { get; set; }
        //Lesson 3 addon
        public IStatePropertyAccessor<RequestorInfo> RequestorInfoAccessor { get; set; }

        public ChatBotAccessor(ConversationState conversationState)
        {
            this.ConversationState = conversationState;
        }
    }
}

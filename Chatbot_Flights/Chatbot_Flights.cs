// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chatbot_Flights.Dialogs;
using Chatbot_Flights.ProjectClass;
using Chatbot_Flights.Cards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.IO;

namespace Chatbot_Flights
{
    public class Chatbot_Flights : ActivityHandler
    {
        
        private const string MainDialogId = "Main Dialog";
        private const string ShowReservationDialogID = "ShowReservationDialog";
        private readonly ChatBotAccessor _accessor;
        private readonly DialogSet _dialogSet;

        public Chatbot_Flights(ChatBotAccessor accessor)
        {
            this._accessor = accessor ?? throw new System.ArgumentException("Accesor object is empty ");


            _dialogSet = new DialogSet(this._accessor.ConversationDialogStateAccesor);
            _dialogSet.Add(new FlightReservationDialogMain(MainDialogId,_accessor));
            _dialogSet.Add(new ShowReservationDialog(ShowReservationDialogID, _accessor));

        }


        
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            DialogContext dialogContext = await _dialogSet.CreateContextAsync(turnContext);
            
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                DialogTurnResult turnResult = await dialogContext.ContinueDialogAsync();

                if (turnResult.Status == DialogTurnStatus.Complete)
                {
                    await WelcomeSecondOptions(turnContext);
                                    
                }
                else if (!dialogContext.Context.Responded)
                {
                    if (turnResult.Status == DialogTurnStatus.Empty)
                    {
                        string caseToLower = turnContext.Activity.Text;
                        switch (caseToLower.ToLower())
                        {
                            case "reservation is started":
                                await dialogContext.BeginDialogAsync(MainDialogId);
                                break;
                            case "reservation":

                                await dialogContext.BeginDialogAsync(ShowReservationDialogID);
                               
                                break;
                            case "back":
                                await WelcomeSecondOptions(turnContext);
                                break;
                            case "hi":
                                await WelcomeSecondOptions(turnContext);
                                break;
                            case "hello":
                                await WelcomeSecondOptions(turnContext);
                                break;
                            case "end":
                                await turnContext.SendActivityAsync(MessageFactory.Text($"Bye have nice day!"), cancellationToken);
                                break;
                            case "help":
                                Activity replyHeroCard = turnContext.Activity.CreateReply("Help Menu");
                                replyHeroCard.Attachments.Add(HelpHeroCard.GetHeroCard("Default").ToAttachment());
                                await turnContext.SendActivityAsync(replyHeroCard, cancellationToken: cancellationToken);
                                break;
                            default:
                                await turnContext.SendActivityAsync(MessageFactory.Text($" Say Hi to start new converation :)"), cancellationToken);
                                break;
                        }

                    }
                    else
                    {
                        await turnContext.SendActivityAsync("Please start reservation from beginning");
                    }
                }

            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                await WelcomeUsers(turnContext); 
            }

            await _accessor.ConversationState.SaveChangesAsync(turnContext, false);
        }

       

        private async Task WelcomeSecondOptions(ITurnContext turnContext)
        {
            SuggestedActions suggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() {Title = "Buy next ticket", Value = "Reservation is started", Type = ActionTypes.ImBack},
                    new CardAction() {Title = "Menage Resrvation", Value = "Reservation", Type = ActionTypes.ImBack},
                    new CardAction() {Title = "End Conversation", Value = "End", Type = ActionTypes.ImBack}
                }
            };

            Activity reply = turnContext.Activity.CreateReply("Welcome again to the Flight Reservation bot choose option:");
            reply.SuggestedActions = suggestedActions;
            await turnContext.SendActivityAsync(reply);
                
            
        }

        private async Task WelcomeUsers(ITurnContext turnContext)
        {
            foreach (ChannelAccount member in turnContext.Activity.MembersAdded)
            {
                if (turnContext.Activity.Recipient.Id != member.Id)
                {
                    SuggestedActions suggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                            new CardAction() {Title = "Buy ticket", Value = "Reservation is started", Type = ActionTypes.ImBack},
                            new CardAction() {Title = "Show Resrvation", Value = "Reservation", Type = ActionTypes.ImBack}
                        }
                    };

                    //Activity reply = turnContext.Activity.CreateReply("Welcome to the bot choose option:");
                    Activity reply = MessageFactory.Text("Welcome to the Flight Reservation bot, choose option:");
                    reply.Attachments = new List<Attachment>() { OtherAttachments.GetInlineAttachment() };
                    reply.SuggestedActions = suggestedActions;
                    await turnContext.SendActivityAsync(reply);
                }
            }
        }

       

    }
}

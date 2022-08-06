using Chatbot_Flights.ProjectClass;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatbot_Flights.Dialogs;
using Chatbot_Flights.Models;
using Chatbot_Flights.Dialogs.SubDialog;
using Microsoft.Bot.Schema;
using Chatbot_Flights.Cards;

namespace Chatbot_Flights.Dialogs
{
    public class FlightReservationDialogMain : ComponentDialog
    {
        private const string SubDialogId = "SubDialogOne";
        private const string BasicInfoDialogId = "BasicInfoDialogId";
        private const string SubDialogCarRentId = "CarReservationID";
        private const string SubDialogOneWayFlightID = "OneWayFlightDialogID";
        private const string ShowReservationDialogID = "ShowReservationDialog";
        private static bool correctDate;
        private static bool isHistory;
        private string reservationNumber;
        private readonly ChatBotAccessor _accessor;

        public FlightReservationDialogMain(string dialogId, ChatBotAccessor accessor) : base(dialogId)
        {
            this.InitialDialogId = SubDialogId;
            this._accessor = accessor ?? throw new System.ArgumentException("Accesor object is empty");

            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                BasicUserInfoAsync,
                AirportFromAsync,
                AirportToAsync,
                EnterFlightFromDateAsync,
                OneWayTicketAsync,
                FlightClassAsync,
                RentCarAsync,
                CarRentFlowAsync,
                GenerateReservationNumberAsync,
                SavingDataAsync,
                EndWaterfall 
            };

            AddDialog(new WaterfallDialog(SubDialogId, waterfallSteps));
            AddDialog(new BasicInfoDialog(BasicInfoDialogId, _accessor));
            AddDialog(new CarReservationDialog(SubDialogCarRentId, _accessor));
            AddDialog(new OneWayFlightDialog(SubDialogOneWayFlightID, _accessor));
            AddDialog(new ShowReservationDialog(ShowReservationDialogID, _accessor));
            AddDialog(new TextPrompt("DateValidator", DateValidaton));
            AddDialog(new TextPrompt("FlightFrom"));
            AddDialog(new TextPrompt("FlightTo"));
            AddDialog(new ChoicePrompt("OneWayTicketConform"));
            AddDialog(new ChoicePrompt("TripClass"));
            AddDialog(new ChoicePrompt("RentaCar"));

        }

        #region BasicUserInfo
        /// <summary>
        /// Method used to Start of Basic dialog
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> BasicUserInfoAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        { 
            return await stepContext.BeginDialogAsync(BasicInfoDialogId);
        }
        #endregion

        #region AirportFrom
        /// <summary>
        /// Method used to get info from user about flight destination
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> AirportFromAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);
            
            return await stepContext.PromptAsync("FlightFrom", new PromptOptions
            {
                Prompt = MessageFactory.Text($"Thanks {flightReservation.Name}" + " " + $"{flightReservation.Surename} Choose the airport from which you want to fly:"),
            }, cancellationToken);
        }
        #endregion

        #region AirportTo
        /// <summary>
        /// Method used for choice Airport To 
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> AirportToAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            flightReservation.AirPortFrom = stepContext.Result as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);
            
            return await stepContext.PromptAsync("FlightTo", new PromptOptions
            {
                Prompt = MessageFactory.Text("Choose the airport to which you want to fly: "),
            }, cancellationToken);
        }
        #endregion

        #region EnterFlightFromDate
        /// <summary>
        /// Method used to get info from user about flight date
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> EnterFlightFromDateAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            flightReservation.AirPortTo = stepContext.Result as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);

            return await stepContext.PromptAsync("DateValidator", new PromptOptions
            {
                Prompt = MessageFactory.Text("Can you please enter the flight date:")
            }, cancellationToken);
        }
        #endregion

        #region OneWayTicket
        /// <summary>
        /// Method used to get info from user about one way ticket
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> OneWayTicketAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            flightReservation.StartDate = stepContext.Result as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);
            return await stepContext.BeginDialogAsync(SubDialogOneWayFlightID);
        }
        #endregion

        #region FlightClass
        /// <summary>
        /// Method used for chose Flight Class
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> FlightClassAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
          
            return await stepContext.PromptAsync("TripClass", new PromptOptions
            {
                Prompt = MessageFactory.Text("Choose the trip class:"),
                RetryPrompt = MessageFactory.Text("Please choose correct option"),
                Choices = ChoiceFactory.ToChoices(new List<String> { "Standard", "Business", "Premium", "More"}),
            }, cancellationToken);
        }
        #endregion

        #region RentCarPrompt
        /// <summary>
        /// Method used for choice Car Rent option
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> RentCarAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            FoundChoice flightClass = stepContext.Result as FoundChoice;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            flightReservation.FlightClass = flightClass.Value as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);
            double fakeFlightCost = FakeDataGenerator.CalculateTripCost(flightReservation.FlightClass.ToString(), flightReservation.AirPortFrom.ToString(), flightReservation.AirPortTo.ToString());
            if(flightReservation.OneWayFlight == "false")
            {
                double fakeFlightCostTwoWay = FakeDataGenerator.CalculateTripCostTwoWay(fakeFlightCost);
                flightReservation.FlightCost = fakeFlightCostTwoWay.ToString();
            }
            else
            {
                flightReservation.FlightCost = fakeFlightCost.ToString();
            }
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);
            return await stepContext.PromptAsync("RentaCar", new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you want rent a car"),
                RetryPrompt = MessageFactory.Text("Chose Yes or No"),
                Choices = ChoiceFactory.ToChoices(new List<String> { "Yes", "No" }),
            }, cancellationToken);

        }
        #endregion

        #region RentCarFlow
        /// <summary>
        /// Method used for triger Car rent Flow 
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> CarRentFlowAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            FoundChoice rentaCar = stepContext.Result as FoundChoice;
            //var stepContextToLover = context.Activity.Text.ToLower();
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            flightReservation.RentCar = rentaCar.Value as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);            
            if (rentaCar.Value == "No")
            {
                flightReservation.RentBookNr = "-";
                await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);
                return await stepContext.NextAsync();
            }
            else
            {
                reservationNumber = FakeDataGenerator.GetUniqueKey(10);
                flightReservation.RentBookNr = reservationNumber;
                await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);
                return await stepContext.BeginDialogAsync(SubDialogCarRentId);
            }
        }
        #endregion

        #region GenerateReservationNumberAsync
        /// <summary>
        /// Method used to generate random number for Reservation ID
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GenerateReservationNumberAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            var requestorReservationAccessor = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            reservationNumber = FakeDataGenerator.GetUniqueKey(10);
            requestorReservationAccessor.ReservationNumber = reservationNumber;
            await _accessor.FlightReservationAccessor.SetAsync(context, requestorReservationAccessor);
            var requestorInfo = await _accessor.RequestorInfoAccessor.GetAsync(context, () => new RequestorInfo());

            if (requestorInfo.Resevations.Count > 0)
            {
                flightReservation.ID = requestorInfo.Resevations[requestorInfo.Resevations.Count - 1].ID + 1;
            }
            else
            {
                flightReservation.ID = 1;
            }

            return await stepContext.NextAsync();
        }
        #endregion

        #region SavingDataAsync
        /// <summary>
        /// Method used for save gathered data
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SavingDataAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var requestorInfo = await _accessor.RequestorInfoAccessor.GetAsync(stepContext.Context, () => new RequestorInfo());
            var requestorReservations = await _accessor.FlightReservationAccessor.GetAsync(stepContext.Context, () => new FlightReservation());

            requestorInfo.Resevations.Add(requestorReservations);

            return await stepContext.ContinueDialogAsync();
        }
        #endregion

        #region EndWaterfall
        /// <summary>
        /// Method used for confirm user choice
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> EndWaterfall(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var requestorInfo = await _accessor.RequestorInfoAccessor.GetAsync(context, () => new RequestorInfo());
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            if (requestorInfo.RequestorName != null)
            {
                if (flightReservation.OneWayFlight == "true")
                {
                    Activity reply = stepContext.Context.Activity.CreateReply("");
                    reply.Attachments.Add(ReservationDetailsCard.ReservationDetails(flightReservation.Name, flightReservation.Surename, flightReservation.Nick, flightReservation.BirthDate,
                        flightReservation.AirPortFrom, flightReservation.AirPortTo, flightReservation.StartDate, flightReservation.OneWayFlight, flightReservation.FlightClass, flightReservation.FlightCost));
                    await stepContext.Context.SendActivityAsync(reply, cancellationToken: cancellationToken);
                    if (flightReservation.RentCar == "Yes")
                    {
                        Activity additionalreply = stepContext.Context.Activity.CreateReply("");
                        additionalreply.Attachments.Add(ReservationDetailsCard.CarDetailsCard(flightReservation.RentCar, flightReservation.RentBookNr, flightReservation.CarSize,
                            flightReservation.CarClass, flightReservation.Insurance));
                        await stepContext.Context.SendActivityAsync(additionalreply, cancellationToken: cancellationToken);
                        await stepContext.Context.SendActivityAsync("Reservation is currently being processed");
                    }

                }
                else
                {
                    Activity reply = stepContext.Context.Activity.CreateReply("");
                    reply.Attachments.Add(ReservationDetailsCard.ReservationDetailsExtended(flightReservation.Name, flightReservation.Surename, flightReservation.Nick, flightReservation.BirthDate,
                        flightReservation.AirPortFrom, flightReservation.AirPortTo, flightReservation.StartDate, flightReservation.OneWayFlight, flightReservation.FlightReturn, flightReservation.ReturnDate, flightReservation.FlightClass, flightReservation.FlightCost));
                    await stepContext.Context.SendActivityAsync(reply, cancellationToken: cancellationToken);
                    if (flightReservation.RentCar == "Yes")
                    {
                        Activity additionalreply = stepContext.Context.Activity.CreateReply("");
                        additionalreply.Attachments.Add(ReservationDetailsCard.CarDetailsCard(flightReservation.RentCar, flightReservation.RentBookNr, flightReservation.CarSize,
                            flightReservation.CarClass, flightReservation.Insurance));
                        await stepContext.Context.SendActivityAsync(additionalreply, cancellationToken: cancellationToken);
                        await stepContext.Context.SendActivityAsync("Reservation is currently being processed");
                    }
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("There's no reservations in system.");
            }
            return await stepContext.EndDialogAsync();
            
        }
        #endregion

        #region DateValidation
        /// <summary>
        /// Method used for confirm user choice
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> DateValidaton(PromptValidatorContext<string> promptValidatorContext, CancellationToken cancellationToken)
        {

            string datestring = promptValidatorContext.Recognized.Value;

            correctDate = DataValidator.IsValidDate(datestring);
            if (correctDate == true)
            {
                isHistory = DataValidator.IsHistoricalDate(datestring);
                if (isHistory == true)
                {
                    await promptValidatorContext.Context.SendActivityAsync("Please type a corrrect date should have format dd/mm/yyy and be from future", cancellationToken: cancellationToken);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                await promptValidatorContext.Context.SendActivityAsync("Please type a corrrect date should have format dd/mm/yyy and be from future", cancellationToken: cancellationToken);
                return false;
            }
               
        }
        #endregion

        #region OnContinueDialogAsync
        /// <summary>
        /// Method used to validate if user name is not to short
        /// </summary>
        /// <param name="innerDc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            string text = innerDc.Context.Activity.Text;

            if (text.ToLower() == "help")
            {
                Activity replyHeroCard = innerDc.Context.Activity.CreateReply("Help Menu");
                replyHeroCard.Attachments.Add(HelpHeroCard.GetHeroCard("Default").ToAttachment());
                await innerDc.Context.SendActivityAsync(replyHeroCard, cancellationToken);
                await innerDc.RepromptDialogAsync();
                return new DialogTurnResult(DialogTurnStatus.Waiting);
            }
            else if (text.ToLower() == "exit")
            {
                return await innerDc.CancelAllDialogsAsync(cancellationToken);
            }
            else if(text.ToLower() == "more")
            {
                Activity replyHeroCard = innerDc.Context.Activity.CreateReply("Help Menu");
                replyHeroCard.Attachments.Add(HelpHeroCard.GetHeroCard("TicketClass").ToAttachment());
                await innerDc.Context.SendActivityAsync(replyHeroCard, cancellationToken);
                await innerDc.RepromptDialogAsync();
                return new DialogTurnResult(DialogTurnStatus.Waiting);
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}


using Chatbot_Flights.Cards;
using Chatbot_Flights.Models;
using Chatbot_Flights.ProjectClass;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Chatbot_Flights.Dialogs
{
    /// <summary>
    /// Sub Dialog Class used for show reservations details
    /// </summary>
    public class ShowReservationDialog : ComponentDialog
    {
       
        private const string ShowReservationDialogID = "ShowReservationDialog";
        private readonly ChatBotAccessor _accessor;
        private bool isNumber;

        public ShowReservationDialog(string dialogId, ChatBotAccessor accessor) : base(dialogId)
        {
            this._accessor = accessor;
            this.InitialDialogId = ShowReservationDialogID;

            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                ChooseReservationIdAsync,
                ShowUserReservationAsync,
                ConfirmationAsync,
                CancellReservationAsync,
                EndWaterfall
            };

            AddDialog(new WaterfallDialog(ShowReservationDialogID, waterfallSteps));
            AddDialog(new TextPrompt("ReservationID", ReservationIdValidatorAsync));
            AddDialog(new TextPrompt("ReservationCancellationID", ReservationCancellingValidatorAsync));
            AddDialog(new ChoicePrompt("Restarting"));
        }

        #region ChooseReservationIdAsync
        /// <summary>
        /// Method used to display to user available reservations and gather information which Resservation should be shown
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DialogTurnResult> ChooseReservationIdAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string ReservationIds = "";
            var requestorInfo = await _accessor.RequestorInfoAccessor.GetAsync(stepContext.Context, () => new RequestorInfo());
            foreach (var item in requestorInfo.Resevations)
            {
                ReservationIds += item.ID + "\n";
            }
            if (requestorInfo.Resevations.Count > 0)
            {

                if (stepContext.Context.Activity.Text.ToLower() == "exit")
                {
                    return await stepContext.EndDialogAsync();
                }
                else
                {
                    return await stepContext.PromptAsync("ReservationID", new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Please enter Reservation ID to display, available reservations: \n" + ReservationIds)
                    },
                    cancellationToken);
                }
            }
            else
            {
                return await stepContext.NextAsync();
            }
        }
        #endregion

        #region Show User Reservation
        private async Task<DialogTurnResult> ShowUserReservationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var requestedID = stepContext.Context.Activity.Text;
            var requestorInfo = await _accessor.RequestorInfoAccessor.GetAsync(context, () => new RequestorInfo());
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());

            foreach (var reservation in requestorInfo.Resevations)
            {
                if (reservation.ID == Convert.ToInt32(requestedID))
                {
                    flightReservation.Name = reservation.Name;
                    flightReservation.Surename = reservation.Surename;
                    flightReservation.Nick = reservation.Nick;
                    flightReservation.BirthDate = reservation.BirthDate;
                    flightReservation.AirPortFrom = reservation.AirPortFrom;
                    flightReservation.AirPortTo = reservation.AirPortTo;
                    flightReservation.StartDate = reservation.StartDate;
                    flightReservation.OneWayFlight = reservation.OneWayFlight;
                    flightReservation.FlightClass = reservation.FlightClass;
                    flightReservation.FlightCost = reservation.FlightCost;
                    flightReservation.RentCar = reservation.RentCar;
                    flightReservation.RentBookNr = reservation.RentBookNr;
                    flightReservation.CarSize = reservation.CarSize;
                    flightReservation.CarClass = reservation.CarClass;
                    flightReservation.Insurance = reservation.Insurance;
                }
            }

            if (requestorInfo.RequestorName != null)
            {
                if(flightReservation.OneWayFlight == "true")
                {
                    Activity reply = stepContext.Context.Activity.CreateReply("");
                    reply.Attachments.Add(ReservationDetailsCard.ReservationDetails(flightReservation.Name, flightReservation.Surename, flightReservation.Nick, flightReservation.BirthDate,
                        flightReservation.AirPortFrom,flightReservation.AirPortTo,flightReservation.StartDate, flightReservation.OneWayFlight, flightReservation.FlightClass, flightReservation.FlightCost));
                    await stepContext.Context.SendActivityAsync(reply, cancellationToken: cancellationToken);
                    if (flightReservation.RentCar == "Yes")
                    {    
                        Activity additionalreply = stepContext.Context.Activity.CreateReply("");
                        additionalreply.Attachments.Add(ReservationDetailsCard.CarDetailsCard(flightReservation.RentCar, flightReservation.RentBookNr, flightReservation.CarSize,
                            flightReservation.CarClass, flightReservation.Insurance));
                        await stepContext.Context.SendActivityAsync(additionalreply, cancellationToken: cancellationToken);
                        await stepContext.Context.SendActivityAsync("Reservation is currently being processed if you would like to know more, write Hi");
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
                        await stepContext.Context.SendActivityAsync("Reservation is currently being processed if you would like to know more, write Hi");
                    }
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("There's no reservations in system.");
            }
            return await stepContext.ContinueDialogAsync();
        }
        #endregion

        #region ConfirmationAsync
        /// <summary>
        /// Method used for confirm user choice about restarting or ending dialog
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DialogTurnResult> ConfirmationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync("Restarting", new PromptOptions
            {
                Prompt = MessageFactory.Text("What do you want to do?"),
                RetryPrompt = MessageFactory.Text("I am sorry, I do not understand, please try again."),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Cancell Reservation", "Restart", "End" })
            },
            cancellationToken);
        }
        #endregion

        #region Cancell Reservation
        /// <summary>
        /// Method used for 
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DialogTurnResult> CancellReservationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            if (context.Activity.Text.ToLower() == "cancell reservation")
            {
                string ReservationIds = "";
                var requestorInfo = await _accessor.RequestorInfoAccessor.GetAsync(context, () => new RequestorInfo());
                await _accessor.RequestorInfoAccessor.SetAsync(context, requestorInfo);
                foreach (var item in requestorInfo.Resevations)
                {
                    if (item != null)
                    {
                        ReservationIds += item.ID + "\n";
                    }
                }
                if (requestorInfo.Resevations.Count > 0)
                {
                    return await stepContext.PromptAsync("ReservationCancellationID", new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Please enter Reservation ID to cancel, available reservations: \n" + ReservationIds)
                    },
                        cancellationToken);
                }
                else
                {
                    return await stepContext.NextAsync();
                }
            }
            else
            {
                return await stepContext.NextAsync();
            }
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
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        }
        #endregion

        #region ReservationCancellingValidatorAsync
        /// <summary>
        /// Method used for cancell reservation
        /// </summary>
        /// <param name="promptContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> ReservationCancellingValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var requestorInfo = await _accessor.RequestorInfoAccessor.GetAsync(promptContext.Context, () => new RequestorInfo());
            foreach (var item in requestorInfo.Resevations)
            {
                if (item.ID == Convert.ToInt32(promptContext.Context.Activity.Text))
                {
                    requestorInfo.Resevations.Remove(item);
                    await _accessor.RequestorInfoAccessor.SetAsync(promptContext.Context, requestorInfo);
                    return true;
                }
            }
            await promptContext.Context.SendActivityAsync("There's no reservations in system.");
            return false;
        }
        #endregion

        #region ReservationIdValidatorAsync
        /// <summary>
        /// Method used for validate if provided Reservation ID is correct
        /// </summary>
        /// <param name="promptContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> ReservationIdValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var requestorInfo = await _accessor.RequestorInfoAccessor.GetAsync(promptContext.Context, () => new RequestorInfo());
            var reservationNumber = promptContext.Context.Activity.Text;
            isNumber = DataValidator.IsNumber(reservationNumber);
            if (isNumber)
            {
                foreach (var item in requestorInfo.Resevations)
                {
                    if (item.ID == Convert.ToInt32(reservationNumber))
                    {
                        requestorInfo.ReservationIdToShow = item.ID;
                        return true;
                    }
                }
                await promptContext.Context.SendActivityAsync("There's no reservations in system provide correct number.");
                return false;

            }
            
            await promptContext.Context.SendActivityAsync("Please provide correct number.");
            return false;

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

            if (text.ToLower() == "help" && innerDc.ActiveDialog.Id != ShowReservationDialogID)
            {
                Activity replyHeroCard = innerDc.Context.Activity.CreateReply("Help Menu");
                replyHeroCard.Attachments.Add(HelpHeroCard.GetHeroCard("ShowReservationDialog").ToAttachment());
                await innerDc.Context.SendActivityAsync(replyHeroCard, cancellationToken);
                await innerDc.RepromptDialogAsync();
                return new DialogTurnResult(DialogTurnStatus.Waiting);
            }
            else if (text.ToLower() == "exit" && innerDc.ActiveDialog.Id != ShowReservationDialogID)
            {
                return await innerDc.CancelAllDialogsAsync(cancellationToken);
            }
            else
            {
                return null;
            }
        }
        #endregion
    }

}

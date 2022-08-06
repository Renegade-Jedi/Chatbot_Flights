using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatbot_Flights.Models;
using Microsoft.Bot.Schema;
using Chatbot_Flights.Cards;

namespace Chatbot_Flights.Dialogs.SubDialog
{
    public class OneWayFlightDialog : ComponentDialog
    {
        private const string SubDialogId = "OneWayFlightDialogID";
        private const string SubDialogIdA = "AditionalInformationDialogId";
        private readonly ChatBotAccessor _accessor;

        public OneWayFlightDialog(string dialogId, ChatBotAccessor accessor) : base(dialogId)
        {
            _accessor = accessor;
            this.InitialDialogId = SubDialogId;

            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                CheckIfOneWayFlightAsync,
                OneWayFlightConfirmationAsync
            };

            AddDialog(new WaterfallDialog(SubDialogId, waterfallSteps));
            AddDialog(new AditionalInformationDialog(SubDialogIdA, _accessor));
            AddDialog(new ChoicePrompt("OneWayFlight"));
        }

        #region One Way Ticket
        /// <summary>
        /// Method used to gather information if ticket is for one way flight
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DialogTurnResult> CheckIfOneWayFlightAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync("OneWayFlight", new PromptOptions
            {
                Prompt = MessageFactory.Text("Is that one way flight?"),
                RetryPrompt = MessageFactory.Text("I am sorry, I do not understand, please try again."),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No"})
            },
        cancellationToken);
        }
        #endregion

        #region Confirm One Way ticket
        /// <summary>
        /// Method used to confirm user choice
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DialogTurnResult> OneWayFlightConfirmationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var requestorReservations = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());

            if (stepContext.Context.Activity.Text.ToLower() == "no")
            {
                requestorReservations.OneWayFlight = "false";
                await _accessor.FlightReservationAccessor.SetAsync(context, requestorReservations);
                return await stepContext.BeginDialogAsync(SubDialogIdA);
            }
            else
            {
                requestorReservations.OneWayFlight = "true";
                await _accessor.FlightReservationAccessor.SetAsync(context, requestorReservations);

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
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

            if (text.ToLower() == "help" && innerDc.ActiveDialog.Id != SubDialogId)
            {
                Activity replyHeroCard = innerDc.Context.Activity.CreateReply("Help Menu");
                replyHeroCard.Attachments.Add(HelpHeroCard.GetHeroCard("OneWayFlightDialog").ToAttachment());
                await innerDc.Context.SendActivityAsync(replyHeroCard, cancellationToken);
                await innerDc.RepromptDialogAsync();
                return new DialogTurnResult(DialogTurnStatus.Waiting);
            }
            else if (text.ToLower() == "exit" && innerDc.ActiveDialog.Id != SubDialogId)
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


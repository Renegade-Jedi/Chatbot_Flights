using Chatbot_Flights.Cards;
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
    public class AditionalInformationDialog : ComponentDialog
    {
        private const string SubDialogId = "AditionalInformationDialogId";
        private readonly ChatBotAccessor _accessor;
        private static bool correctDate;
        private static bool isHistory;
        private static bool isBigger;
        public AditionalInformationDialog(string dialogId, ChatBotAccessor accessor): base(dialogId)
        {
            this._accessor = accessor;
            this.InitialDialogId = SubDialogId;

            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                AirportToAsync,GetReturnDateAsync, ConfirmationAsync
            };

            AddDialog(new WaterfallDialog(SubDialogId, waterfallSteps));
            AddDialog(new TextPrompt("DateValidator", DateValidaton));
            AddDialog(new TextPrompt("AirportTo"));
        }

        #region AirportTo
        /// <summary>
        /// Method used for choice Airport To 
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> AirportToAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync("AirportTo", new PromptOptions
            {
                Prompt = MessageFactory.Text("Choose the airport to return: ")
            }, cancellationToken);
        }
        #endregion

        #region Return Date
        /// <summary>
        /// Method used to get info from user about return date
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetReturnDateAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            flightReservation.FlightReturn = stepContext.Result as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);

            return await stepContext.PromptAsync("DateValidator", new PromptOptions
            {
                Prompt = MessageFactory.Text("Can you please enter the flight date:")
            }, cancellationToken);
        }
        #endregion

        #region ConfirmationAsync
        private async Task<DialogTurnResult> ConfirmationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());

            flightReservation.ReturnDate = stepContext.Result as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);

            return await stepContext.EndDialogAsync(cancellationToken : cancellationToken);
        }
        #endregion

        #region DateValidation
        /// <summary>
        /// Method used for confirm user choice
        /// </summary>
        /// <param name="promptValidatorContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> DateValidaton(PromptValidatorContext<string> promptValidatorContext, CancellationToken cancellationToken)
        {
            var context = promptValidatorContext.Context;
            string datestring = promptValidatorContext.Recognized.Value;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());

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
                    isBigger = DataValidator.CompareTwoDates(flightReservation.StartDate, datestring);
                    if (isBigger)
                    {
                        return true;
                    }
                    else
                    {
                        await promptValidatorContext.Context.SendActivityAsync($"Please type a corrrect date should have format dd/mm/yyy and should be bigger than: {flightReservation.StartDate}", cancellationToken: cancellationToken);
                        return false;
                    }                    
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

            if (text.ToLower() == "help" && innerDc.ActiveDialog.Id != SubDialogId)
            {
                Activity replyHeroCard = innerDc.Context.Activity.CreateReply("Help Menu");
                replyHeroCard.Attachments.Add(HelpHeroCard.GetHeroCard("AdditionalInformationDialog").ToAttachment());
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

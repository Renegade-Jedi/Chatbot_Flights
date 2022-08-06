using Chatbot_Flights.Cards;
using Chatbot_Flights.Models;
using Chatbot_Flights.ProjectClass;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
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
    public class BasicInfoDialog : ComponentDialog
    {
        private const string BasicInfoDialogId = "BasicInfoDialogId";
        private readonly ChatBotAccessor _accessor;
        private static bool correctDate;
        private static bool isHistory;

        public BasicInfoDialog(string dialogId, ChatBotAccessor accessor) : base(dialogId)
        {
            this._accessor = accessor;
            this.InitialDialogId = BasicInfoDialogId;

            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                GetNameAsync,GetSurenameAsync,GetNickAsync,GetbirthDateAsync, ConfirmationAsync
            };

            AddDialog(new WaterfallDialog(BasicInfoDialogId, waterfallSteps));
            AddDialog(new TextPrompt("Name",UserNameValidator));
            AddDialog(new TextPrompt("Surename",UserNameValidator));
            AddDialog(new TextPrompt("nick"));
            AddDialog(new TextPrompt("birthDate", DateValidaton));
        }

        #region GetName
        /// <summary>
        /// Method used to Get Name From User
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync("Name", new PromptOptions
            {
                Prompt = MessageFactory.Text("Enter your Name"),
            }, cancellationToken);
        }
        #endregion

        #region GetSureName
        /// <summary>
        /// Method used to Get User Name From User
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetSurenameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            flightReservation.Name = stepContext.Result as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);

            return await stepContext.PromptAsync("Surename", new PromptOptions
            {
                Prompt = MessageFactory.Text($"Thanks {flightReservation.Name} Enter You Surename"),
            }, cancellationToken);
        }
        #endregion

        #region GetNick
        /// <summary>
        /// Method used to get user birth date
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetNickAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            flightReservation.Surename = stepContext.Result as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);

            return await stepContext.PromptAsync("nick", new PromptOptions
            {
                Prompt = MessageFactory.Text("Enter your Nick"),
            }, cancellationToken);
        }
        #endregion

        #region GetBirthDate
        /// <summary>
        /// Method used to get user birth date
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetbirthDateAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            flightReservation.Nick = stepContext.Result as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);

            return await stepContext.PromptAsync("birthDate", new PromptOptions
            {
                Prompt = MessageFactory.Text("Enter your birthDate"),
            }, cancellationToken);
        }
        #endregion

        #region Confirmation
        /// <summary>
        /// Method used to Save all Requested Data
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ConfirmationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var flightReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            var requestorInfo = await _accessor.RequestorInfoAccessor.GetAsync(context, () => new RequestorInfo());
            flightReservation.BirthDate = stepContext.Result as String;
            requestorInfo.RequestorName = flightReservation.Name as String +" "+ flightReservation.Surename as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, flightReservation);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
        #endregion

        #region UserNameValidator
        /// <summary>
        /// Method used to validate if user name is not to short
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> UserNameValidator(PromptValidatorContext<string> promptValidatorContext, CancellationToken cancellationToken)
        {
            string userName = promptValidatorContext.Recognized.Value;
            if (userName == null || userName.Length < 3)
            {
                await promptValidatorContext.Context.SendActivityAsync("Please type a corrrect name", cancellationToken: cancellationToken);
                return false;
            }
            else
            {
                return true;
            }
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

            string datestring = promptValidatorContext.Recognized.Value;

            correctDate = DataValidator.IsValidDate(datestring);
            if (correctDate == true)
            {
                isHistory = DataValidator.IsHistoricalDate(datestring);
                if (isHistory == true)
                {
                    return true;
                }
                else
                {
                    await promptValidatorContext.Context.SendActivityAsync("Please type a corrrect date should be from past and have format dd/mm/yyy", cancellationToken: cancellationToken);
                    return false;
                }
            }
            else
            {
                await promptValidatorContext.Context.SendActivityAsync("Please type a corrrect date should be from past and have format dd/mm/yyy", cancellationToken: cancellationToken);
                return false;
            }
        }
        #endregion

        #region OnContinueDialogAsync
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

            if (text.ToLower() == "help" && innerDc.ActiveDialog.Id != BasicInfoDialogId)
            {
                Activity replyHeroCard = innerDc.Context.Activity.CreateReply("Help Menu");
                replyHeroCard.Attachments.Add(HelpHeroCard.GetHeroCard("BasicInfoDialog").ToAttachment());
                await innerDc.Context.SendActivityAsync(replyHeroCard, cancellationToken);
                await innerDc.RepromptDialogAsync();
                return new DialogTurnResult(DialogTurnStatus.Waiting);
            }
            else if (text.ToLower() == "exit" && innerDc.ActiveDialog.Id != BasicInfoDialogId)
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


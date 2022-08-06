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
    public class CarReservationDialog : ComponentDialog
    {
        private const string SubDialogId = "CarReservationID";
        private readonly ChatBotAccessor _accessor;

        public CarReservationDialog(string dialogId, ChatBotAccessor accessor) : base(dialogId)
        {
            this._accessor = accessor;
            this.InitialDialogId = SubDialogId;

            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                GetCarSizeAsync,GetCarClassAsync,GetCarInsuranceAsync, ConfirmationAsync
            };

            AddDialog(new WaterfallDialog(SubDialogId, waterfallSteps));
            AddDialog(new ChoicePrompt("CarSize"));
            AddDialog(new ChoicePrompt("CarClass"));
            AddDialog(new ChoicePrompt("CarInsurance"));
        }

        #region GetCarSizeAsync
        /// <summary>
        /// Method used for confirm user choice
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetCarSizeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync("CarSize", new PromptOptions
            {
                Prompt = MessageFactory.Text("Select Car Size :"),
                RetryPrompt = MessageFactory.Text("Chose true or false"),
                Choices = ChoiceFactory.ToChoices(new List<String> { "Hatchback", "Sedan", "SUV", "Van", "PickUp", "CarSizeInfo" }),
            }, cancellationToken);
        }
        #endregion

        #region GetCarClassAsync
        /// <summary>
        /// Method used for confirm user choice
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetCarClassAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            FoundChoice carSize = stepContext.Result as FoundChoice;
            var carReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());

            carReservation.CarSize = carSize.Value as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, carReservation);

            return await stepContext.PromptAsync("CarClass", new PromptOptions
            {
                Prompt = MessageFactory.Text("Select Car Class :"),
                RetryPrompt = MessageFactory.Text("Chose One Option"),
                Choices = ChoiceFactory.ToChoices(new List<String> { "Standard", "Premnium", "Lux", "CarClasInfo" }),
            }, cancellationToken);
        }
        #endregion

        #region GetCarInsuranceAsync
        /// <summary>
        /// Method used for confirm user choice
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetCarInsuranceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            FoundChoice carClass = stepContext.Result as FoundChoice;
            var carReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());
            carReservation.CarClass = carClass.Value as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, carReservation);

            return await stepContext.PromptAsync("CarInsurance", new PromptOptions
            {
                Prompt = MessageFactory.Text("Select Car Insurance Option:"),
                RetryPrompt = MessageFactory.Text("Chose One Option"),
                Choices = ChoiceFactory.ToChoices(new List<String> { "Standard", "Premnium", "Lux", "InsuranceInfo" }),
            }, cancellationToken);
        }
        #endregion

        #region ConfirmationAsync
        /// <summary>
        /// Method used for confirm user choice
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ConfirmationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            FoundChoice insurance = stepContext.Result as FoundChoice;
            var carReservation = await _accessor.FlightReservationAccessor.GetAsync(context, () => new FlightReservation());

            carReservation.Insurance = insurance.Value as String;
            await _accessor.FlightReservationAccessor.SetAsync(context, carReservation);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
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
            Activity replyHeroCard = innerDc.Context.Activity.CreateReply("");
            if (text.ToLower() == "help" && innerDc.ActiveDialog.Id != SubDialogId)
            { 
                replyHeroCard.Attachments.Add(HelpHeroCard.GetHeroCard("CarReservationDialog").ToAttachment());
                await innerDc.Context.SendActivityAsync(replyHeroCard, cancellationToken);
                await innerDc.RepromptDialogAsync();
                return new DialogTurnResult(DialogTurnStatus.Waiting);
            }
            else if (text.ToLower() == "exit" && innerDc.ActiveDialog.Id != SubDialogId)
            {
                return await innerDc.CancelAllDialogsAsync(cancellationToken);
            }
            else if (text.ToLower() == "carclasinfo" && innerDc.ActiveDialog.Id != SubDialogId)
            {
                replyHeroCard.Attachments.Add(HelpHeroCard.GetHeroCard("CarClass").ToAttachment());
                await innerDc.Context.SendActivityAsync(replyHeroCard, cancellationToken);
                await innerDc.RepromptDialogAsync();
                return new DialogTurnResult(DialogTurnStatus.Waiting);
            }
            else if (text.ToLower() == "carsizeinfo" && innerDc.ActiveDialog.Id != SubDialogId)
            {
                replyHeroCard.Attachments.Add(HelpHeroCard.GetHeroCard("CarSize").ToAttachment());
                await innerDc.Context.SendActivityAsync(replyHeroCard, cancellationToken);
                await innerDc.RepromptDialogAsync();
                return new DialogTurnResult(DialogTurnStatus.Waiting);
            }
            else if (text.ToLower() == "insuranceinfo" && innerDc.ActiveDialog.Id != SubDialogId)
            {
                replyHeroCard.Attachments.Add(HelpHeroCard.GetHeroCard("CarInsurance").ToAttachment());
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

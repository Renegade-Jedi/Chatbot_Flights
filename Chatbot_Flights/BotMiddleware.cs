using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Chatbot_Flights
{ 
  /// <summary>
  /// Middleware for logging incoming and outgoing activities/>.
  /// </summary>
    public class BotMiddleware : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(turnContext.Activity.Type == ActivityTypes.Message)
            {
                Debug.WriteLine(turnContext.Activity.Text);
                turnContext.Activity.Text = turnContext.Activity.Text.ToUpper();
                // Logic which run before the bot logic
                await next(cancellationToken);
                // Logic which run after the bot logic
                Debug.WriteLine(turnContext.Activity.Text);
            }
            else
            {
                await next(cancellationToken);
            }

        }
    }
}


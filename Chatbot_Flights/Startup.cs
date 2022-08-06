// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.3.0

using Chatbot_Flights.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Chatbot_Flights
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IStorage dataStorage = new MemoryStorage();
            ConversationState conversationState = new ConversationState(dataStorage);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Create the Bot Framework Adapter.
            services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, Chatbot_Flights>();

            services.AddSingleton<ChatBotAccessor>(sp =>
            {
                return new ChatBotAccessor(conversationState)
                {
                    ConversationDialogStateAccesor = conversationState.CreateProperty<DialogState>(ChatBotAccessor.ChatbotAccesorName),
                    FlightReservationAccessor = conversationState.CreateProperty<FlightReservation>(ChatBotAccessor.FlightReservationAccessName),
                    RequestorInfoAccessor = conversationState.CreateProperty<RequestorInfo>(ChatBotAccessor.RequestorInfoAccessorName)
                };
            });
            var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            var botConfig = BotConfiguration.Load(@".\Chatbot_Flights.bot", secretKey);

            services.AddBot<Chatbot_Flights>(options =>
            {
                services.AddSingleton(sp => botConfig);
                options.Middleware.Add(new BotMiddleware());

                //Retrive current endpoint
                var service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == "development").FirstOrDefault();
                if(!(service is EndpointService endpointService))
                {
                    throw new InvalidOperationException($"The .bot file does not contain a development endpoint. ");
                }

                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                //Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    await context.SendActivityAsync("Sorry, it looks like something went weong");
                };

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

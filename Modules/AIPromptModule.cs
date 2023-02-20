using Discord.Interactions;
using Discord;
using DNet_V3_Tutorial.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI_API;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using OpenAI_API.Models;
using Microsoft.VisualBasic;
using System.Resources;
using System.Reflection;
using static DNet_V3_Tutorial.YourStorage;

namespace DNet_V3_Bot.Modules
{
    
        public class AIPromptModule: InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }
        private static Logger _logger;
        IConfigurationRoot? config;
        OpenAIAPI openAIApiConnection;
        int today = DateTime.Now.DayOfYear;
        public Model PondweedModel => new Model("davinci:ft-personal-2023-02-19-23-15-12") { OwnedBy = "openai" };

        public AIPromptModule(ConsoleLogger logger)
        {
            today = DateTime.Now.DayOfYear;

            _logger = logger;

            config = new ConfigurationBuilder()
            // this will be used more later on
            .SetBasePath(AppContext.BaseDirectory)
            // I chose using YML files for my config data as I am familiar with them
            .AddXmlFile("config.xml")
            .Build();

            openAIApiConnection = new OpenAIAPI(config.GetSection("tokens")["openAI"]);

        }


        // Basic slash command. [SlashCommand("name", "description")]
        // Similar to text command creation, and their respective attributes
        [SlashCommand("ask", "Ask Pondwed AI a question")]
        public async Task Ask(string prompt)
        {
            await DeferAsync();
            if(today != globalDate)
            {
                askCount = 0;
                globalDate = today;
            }
            if (askCount >= 50)
            {
                await FollowupAsync("Sorry, I've already answered the max questions today. Please try again tomorrow.");
                return;
            }
            askCount++;
            Console.WriteLine(askCount);
            // New LogMessage created to pass desired info to the console using the existing Discord.Net LogMessage parameters
            await _logger.Log(new LogMessage(LogSeverity.Info, "AIPromptModule : Ask", $"User: {Context.User.Username}, Command: ask", null));
            var result = await openAIApiConnection.Completions.CreateCompletionAsync(new OpenAI_API.Completions.CompletionRequest(prompt, model: PondweedModel, max_tokens: 150, temperature: .5));
            await FollowupAsync(result.ToString());

        }
        //detect a discord user replying to a bot message

        //https://stackoverflow.com/questions/66000000/how-to-detect-a-discord-user-replying-to-a-bot-message
    }
}

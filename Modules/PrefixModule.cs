using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI_API;
using DNet_V3_Tutorial.Log;
using Microsoft.Extensions.Configuration;
using OpenAI_API.Models;
using Discord.WebSocket;
using static DNet_V3_Tutorial.YourStorage;

namespace DNet_V3_Bot.Modules
{
    // this Module name, PrefixModule, will be called by AddModule when loading the bot with the available prefix commands
    public class PrefixModule : ModuleBase<SocketCommandContext>
    {
        OpenAIAPI openAIApiConnection;
        IConfigurationRoot config;
        private readonly int maxAsksPerDay = 1;
        private int today;
        public Model PondweedModel => new Model("davinci:ft-personal-2023-02-19-23-15-12") { OwnedBy = "openai" };
        public PrefixModule()
        {

            today = DateTime.Now.DayOfYear;
           
            config = new ConfigurationBuilder()
            // this will be used more later on
            .SetBasePath(AppContext.BaseDirectory)
            // I chose using YML files for my config data as I am familiar with them
            .AddXmlFile("config.xml")
            .Build();

            openAIApiConnection = new OpenAIAPI(config.GetSection("tokens")["openAI"]);

        }
        [Command("ping")]
        public async Task Pong()
        {
            // Reply to the user's message with the response
            await Context.Message.ReplyAsync("Pong!");
        }
        public async Task Ask(string prompt, SocketUserMessage message)
        {
            
            if (today != globalDate)
            {
                askCount = 0;
                globalDate = today;
            }
            if (askCount >= 50)
            {
                await ReplyAsync("Sorry, I've already answered the max questions today. Please try again tomorrow.");
                return;
            }
            askCount++;
            Console.WriteLine(askCount);
            var result = await openAIApiConnection.Completions.CreateCompletionAsync(new OpenAI_API.Completions.CompletionRequest(prompt, model: PondweedModel, max_tokens: 175, temperature: .7));
            await message.ReplyAsync(result.ToString());

        }
    }
}

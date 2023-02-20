using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNet_V3_Bot.Modules;

namespace DNet_V3_Tutorial
{
    public class PrefixHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly string _prefix;
        public static List<string> botMessages;


        // Retrieve client and CommandService instance via ctor
        public PrefixHandler(DiscordSocketClient client, CommandService commands, IConfigurationRoot config)
        {
            _commands = commands;
            _client = client;
            _config = config;
            _prefix = _config.GetSection("prefix").Value;
        }

        public async Task InitializeAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _client.SetActivityAsync(new Game("Dleik's Sister", ActivityType.Playing));
        }
        public void AddModule<T>()
        {
            _commands.AddModuleAsync<T>(null);
        }
        
        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 1;
            SocketGuildUser socketGuildUser = message.Author as SocketGuildUser;

            if(message.ReferencedMessage != null && message.ReferencedMessage.Interaction != null  ) 
            {
                if(message.ReferencedMessage.Interaction.Name == "ask") 
                {

                    if (!string.IsNullOrEmpty(message.Content))
                    {
                        // Initialize the array if it doesn't exist yet
                        if (botMessages is null)
                        {
                            botMessages = new List<string>();
                        }
                        string combinedText = "convo ";
                        // Add the message to the array
                        botMessages.Add(message.ReferencedMessage.Author.Username + ": " + message.ReferencedMessage.Content);
                        botMessages.Add(message.Author.Username+": "+message.Content);


                        // If the array length is greater than 10, remove the oldest message
                        if (botMessages.Count > 10)
                        {
                            botMessages.RemoveAt(0);
                        }
                        foreach (var item in botMessages)
                        {
                            combinedText += item;
                            Console.WriteLine(item);
                        }
                        PrefixModule prefixModule = new PrefixModule();
                        await prefixModule.Ask(combinedText, message);  
                    }
                }
            }
            else if(message.ReferencedMessage != null && message.ReferencedMessage.Interaction == null && botMessages != null && message.ReferencedMessage.Author.IsBot)
            {
                if (!string.IsNullOrEmpty(message.Content))
                {

                    string combinedText = "convo ";
                    // Add the message to the array
                    botMessages.Add(message.ReferencedMessage.Author.Username + ": " + message.ReferencedMessage.Content);
                    botMessages.Add(message.Author.Username + ": " + message.Content);
                    
                    

                    // If the array length is greater than 10, remove the oldest message
                    if (botMessages.Count > 10)
                    {
                        botMessages.RemoveAt(0);
                    }
                    foreach (var item in botMessages)
                    {
                        combinedText += item;
                        Console.WriteLine(item);
                    }
                    PrefixModule prefixModule = new PrefixModule();
                    await prefixModule.Ask(combinedText, message);

                }
            }

        
            //if the message has the prefix or is @'d by the user, and is not a bot message
            
            if (message.HasCharPrefix('!', ref argPos) && !message.Author.IsBot)
            {
                // Create a WebSocket-based command context based on the message
                var context = new SocketCommandContext(_client, message);

                // Execute the command with the command context we just
                // created, along with the service provider for precondition checks.
                await _commands.ExecuteAsync(
                                       context: context,
                                                          argPos: argPos,
                                                                             services: null);
            }
            else if (message.HasMentionPrefix(_client.CurrentUser, ref argPos) && !message.Author.IsBot)
            {
                // Create a WebSocket-based command context based on the message
                var context = new SocketCommandContext(_client, message);
      
                // Execute the command with the command context we just
                // created, along with the service provider for precondition checks.
                await _commands.ExecuteAsync(
                                       context: context,
                                                          argPos: argPos,
                                                                             services: null);
            }
        }
        
    }
}

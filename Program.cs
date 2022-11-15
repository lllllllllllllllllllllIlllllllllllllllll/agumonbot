using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

namespace AgumonBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            // subscribe logging handler to both the client and the CommandService
            _client.Log += Log;
            _commands.Log += Log;

            // Setup your DI container
            _services = ConfigureServices(_client, _commands);

            string token = File.ReadAllText("token.txt");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await InitCommands();

            // Block this task until the program is closed.
            await Task.Delay(Timeout.Infinite);
        }
        
        private static IServiceProvider ConfigureServices
            (DiscordSocketClient client, CommandService commands)
        {
            var map = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands);

            return map.BuildServiceProvider();
        }

        public async Task InitCommands()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;

            // bail out if system message
            if (msg == null) return;

            // do not respond to other bots or itself
            if (msg.Author.IsBot) return;

            int pos = 0;

            // ! prefix
            if (msg.HasCharPrefix('!', ref pos))
            {
                // create a command context
                var context = new SocketCommandContext(_client, msg);

                // execute command
                var result = await _commands.ExecuteAsync(context, pos, _services);

                /*
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
                }
                */
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }

        }

        private Task Log(LogMessage msg)
        {
            switch(msg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine(msg.ToString());
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Text.Json;

namespace AgumonBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        Random rnd = new Random();
        [Command("8ball")]
        public async Task YesNo([Remainder] string query)
        {
            string[] answers = File.ReadAllLines("8ball.txt");
            await ReplyAsync("*" + answers[rnd.Next(0, answers.Length)] + "*");

        }

        [Command("roll")]
        public async Task RollDie([Remainder] int num)
        {
            await ReplyAsync("You have rolled: " + rnd.Next(1, num + 1).ToString());
        }

        [Command("bandai")]
        public async Task Bandai([Remainder] string query)
        {
            string format = "https://www.bandai-tcg-plus.com/card?card_param={0}&game_title_id=2&limit=20&offset=0";
            await ReplyAsync(string.Format(format, query));
        }

        [Command("card")]
        public async Task Card([Remainder] string query)
        {
            string output = "";

            if (query.Substring(0,2).ToUpper() == "BT")
            {
                query = query.Substring(0, 2).ToUpper() + query.Substring(2);
                string format = "https://digimoncardgame.fandom.com/wiki/{0}";
                output = string.Format(format, query);
            }
            else if (query.Split('-').Length > 1)
            {
                string format = "https://digimoncardgame.fandom.com/wiki/BT{0}";
                output = string.Format(format, query);
            }
            else
            {
                var jsonFile = File.ReadAllText("database.json");
            }
            await ReplyAsync(output);
        }

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("pong");
        }

        [Command("pong")]
        public async Task Pong()
        {
            await ReplyAsync("ping");
        }

        [Command("timetable")]
        public async Task ShowTimetable()
        {
            await ReplyAsync("https://media.discordapp.net/attachments/884943555687239743/999478049387073616/BIT-2022-V4.png?width=1402&height=701");
        }

        [Command("help")]
        public async Task ListCommands()
        {
            string output = "```" +
                "digimon\n" +
                "------------\n" + 
                "!bandai cardname: looks up digimon card by name in bandai tcgplus\n" +
                "!card {set-number}: looks up digimon cards, format: !card bt6-111 or !card 6-111\n" +
                "school+\n" +
                "------------\n" +
                "!timetable: shows a picture of timetable\n" +
                "fun\n" + 
                "------------\n" +
                "!8ball {query}: consult the all knowing 8ball\n" +
                "!roll x: roll x sided die\n" +
                "!ping: pong" +
                "```";
            await ReplyAsync(output);
        }
    }
}

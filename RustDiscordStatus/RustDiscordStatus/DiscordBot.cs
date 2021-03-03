using System.Threading.Tasks;
using DSharpPlus;

namespace RustDiscordStatus
{
    public class DiscordBot
    {
        private readonly string _botToken;
        private DiscordClient _discordClient;
        
        private async Task Startup()
        {
            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = _botToken,
                TokenType = TokenType.Bot       
            });
            await _discordClient.ConnectAsync();
        }
        
        public DiscordBot(string botToken)
        {
            _botToken = botToken;
            Startup().GetAwaiter().GetResult();
        }
    }
}
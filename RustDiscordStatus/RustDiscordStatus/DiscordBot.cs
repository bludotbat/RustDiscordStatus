using System;
using System.Diagnostics.SymbolStore;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using SourceQuery;

namespace RustDiscordStatus
{
    public class DiscordBot
    {
        private DiscordClient _discordClient;
        private readonly string _botToken;
        private readonly string _serverIP;
        private readonly int _threshold;
        private readonly string _thresholdMessage;

        private bool _ready = false;

        public bool IsReady()
        {
            return _ready;
        }

        public async Task Worker(bool restart)
        {
            try
            {
                var gs = new GameServer(IPEndPoint.Parse(_serverIP));
                var playerCount = gs.Players.Count;
                bool hitThreshold = (playerCount > _threshold);
                var discordActivity = new DiscordActivity(hitThreshold ? gs.Players.Count + " players!" : _thresholdMessage);
                discordActivity.ActivityType = ActivityType.Watching;
                await _discordClient.UpdateStatusAsync(discordActivity, UserStatus.Online);
            }
            catch (Exception ex)
            {
                var discordActivity = new DiscordActivity(restart ? "Restarting..." : "Offline!");
                discordActivity.ActivityType = ActivityType.Watching;
                await _discordClient.UpdateStatusAsync(discordActivity, restart ? UserStatus.Idle : UserStatus.DoNotDisturb);
            }
        }

        private async Task Startup()
        {
            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = _botToken,
                TokenType = TokenType.Bot       
            });
            
            await _discordClient.ConnectAsync();
            
            _discordClient.Ready += async (s, e) =>
            {
                _ready = true;
            };
        }
        
        public DiscordBot(string botToken, string serverIp, int threshold, string thresholdMessage)
        {
            _botToken = botToken;
            _serverIP = serverIp;
            _threshold = threshold;
            _thresholdMessage = thresholdMessage;
            Startup().GetAwaiter().GetResult();
        }
    }
}
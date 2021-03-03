using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace RustDiscordStatus
{
    public class Server {
        public string BotToken { get; set; } 
        public string ServerIP { get; set; } 
        public int Threshold { get; set; } 
        public string ThresholdMessage { get; set; } 
    }

    public class Config {
        public int UpdateTime { get; set; } 
        public bool RestartingOverOffline { get; set; } 
        public List<Server> Servers { get; set; } 
    }
    
    class Program
    {
        private static Config _config;
        private static List<DiscordBot> _instances = new List<DiscordBot>();
        static void Main(string[] args)
        {
            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            Console.WriteLine("Starting up...");
            
            foreach (var server in _config.Servers)
            {
                try
                {
                    _instances.Add(new DiscordBot(server.BotToken, server.ServerIP, server.Threshold, server.ThresholdMessage));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to add " + server.ServerIP + "!");
                }
            }
            
            Console.WriteLine("Loaded data ");
            while (true)
            {
                Thread.Sleep(_config.UpdateTime);
                foreach (var bot in _instances)
                    if (bot.IsReady())
                    {
                        bot.Worker(_config.RestartingOverOffline).GetAwaiter().GetResult();
                        Thread.Sleep(2000);
                    }
            }
        }
    }
}
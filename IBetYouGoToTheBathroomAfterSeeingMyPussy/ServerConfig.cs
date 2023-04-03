using Newtonsoft.Json;
using NLog;
using System;
using System.IO;

public class ServerConfig
{
    public string RedisOption { get; set; } = "127.0.0.1,syncTimeout=3000";
    private Logger logger = LogManager.GetLogger("Conf");

    public void InitServerConfig()
    {
        try { File.WriteAllText("server_config_example.json", JsonConvert.SerializeObject(new ServerConfig(), Formatting.Indented)); } catch { }
        if (!File.Exists("server_config.json"))
        {
            logger.Error($"server_config.json遺失，請依照 {Path.GetFullPath("server_config_example.json")} 內的格式填入正確的數值");
            if (!Console.IsInputRedirected)
                Console.ReadKey();
            Environment.Exit(3);
        }

        try
        {
            var config = JsonConvert.DeserializeObject<ServerConfig>(File.ReadAllText("server_config.json"));
            if (config == null)
            {
                logger.Error($"server_config.json讀取失敗，請依照 {Path.GetFullPath("server_config_example.json")} 內的格式填入正確的數值");
                if (!Console.IsInputRedirected)
                    Console.ReadKey();
                Environment.Exit(3);
            }

            if (string.IsNullOrWhiteSpace(config.RedisOption))
            {
                logger.Error("RedisOption遺失，請輸入至server_config.json後重開伺服器");
                if (!Console.IsInputRedirected)
                    Console.ReadKey();
                Environment.Exit(3);
            }

            RedisOption = config.RedisOption;
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
            throw;
        }
    }
}
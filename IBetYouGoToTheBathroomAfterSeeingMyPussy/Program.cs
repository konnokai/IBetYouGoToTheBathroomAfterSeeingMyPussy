using NLog.Web;

namespace I_bet_you_go_to_the_bathroom_after_seeing_my_pussy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                Utility.ServerConfig.InitServerConfig();
                logger.Info("初始化中");

                try
                {
                    RedisConnection.Init(Utility.ServerConfig.RedisOption);
                    Utility.Redis = RedisConnection.Instance.ConnectionMultiplexer;
                    Utility.RedisDb = Utility.Redis.GetDatabase(2);
                    logger.Info("Redis已連線");
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Redis連線錯誤，請確認伺服器是否已開啟\r\n");
                    return;
                }

                var builder = WebApplication.CreateBuilder(args);
                var app = builder.Build();

                app.UseMiddleware<Middleware.LogMiddleware>();

                app.MapGet("/", (HttpRequest request, HttpResponse response) =>
                {
                    return Results.Content(html, "text/html");
                });

                app.Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception\r\n");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
                Utility.Redis.Dispose();
            }
        }

        static string html = @"<!DOCTYPE html>
<html>
    <head>
        <meta charset=""utf8"">
        <meta property=""og:type"" content=""article"">
        <meta property=""og:title"" content=""我敢打赌你会在看完我的猫咪后去洗手间"">
        <meta property=""og:description"" content=""我敢打赌你会在看完我的猫咪后去洗手间"">
        <meta property=""og:image"" content=""https://i.imgur.com/TZ7RVe6.png"">
        <meta property=""og:image:width"" content=""589"">
        <meta property=""og:image:height"" content=""332"">

        <meta name=""twitter:card"" content=""summary_large_image"">
        <meta name=""twitter:site"" content=""www.youtube.com"">
        <meta name=""twitter:title"" content=""我敢打赌你会在看完我的猫咪后去洗手间"">
        <meta name=""twitter:description"" content=""我敢打赌你会在看完我的猫咪后去洗手间"">
        <meta name=""twitter:domain"" content=""www.youtube.com"">
    </head>
    <body data-mobile-mode=""yep"" data-showing-advertisement=""none"">
        <div id=""box"">
            <h1>我敢打赌你会在看完我的猫咪后去洗手间</h1>
            <p>
                <a href=""https://youtu.be/hvL1339luv0"">去洗手间</a>
            </p>
        </div>
        <script type=""text/javascript"">
            document.getElementById(""box"").style.display = ""none"";
            location.replace('https://youtu.be/hvL1339luv0');
	
            window.addEventListener('error', function(err) {
                location.replace('https://youtu.be/hvL1339luv0');
                console.log('error', err);
            }, true);
        </script>
    </body>
</html>";
    }
}
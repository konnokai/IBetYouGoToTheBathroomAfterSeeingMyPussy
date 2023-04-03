using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using NLog;
using StackExchange.Redis;
using System.Text;

namespace I_bet_you_go_to_the_bathroom_after_seeing_my_pussy.Middleware
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private Logger logger = LogManager.GetLogger("ACCE");

        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalResponseBodyStream = context.Response.Body;

            try
            {
                var remoteIpAddress = context.GetRemoteIPAddress();
                var requestUrl = context.Request.GetDisplayUrl();
                string redisKey = $"Server.Bathroom.ClickCount";
                Microsoft.Extensions.Primitives.StringValues userAgent = "N/A";

                try
                {
                    if (context.Request.Headers.TryGetValue("User-Agent", out userAgent) && !userAgent.Contains("http"))
                    {
                        await Utility.RedisDb.StringIncrementAsync(redisKey);
                    }
                }
                catch (RedisConnectionException redisEx)
                {
                    logger.Error(redisEx, "Redis掛掉了");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Middleware錯誤");
                }

                await _next(context);

                logger.Info($"{remoteIpAddress} | {context.Request.Method} | {context.Response.StatusCode} | {requestUrl}");
                logger.Info($"User-Agent: {userAgent}");
            }
            catch (Exception e)
            {
                logger.Error(e);

                var errorMessage = JsonConvert.SerializeObject(new
                {
                    ErrorMessage = e.Message
                });
                var bytes = Encoding.UTF8.GetBytes(errorMessage);

                await originalResponseBodyStream.WriteAsync(
                    bytes, 0, bytes.Length);
            }
        }
    }
}
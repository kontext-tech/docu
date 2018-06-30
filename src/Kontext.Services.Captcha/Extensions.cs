using Microsoft.AspNetCore.Builder;

namespace Kontext.Services.Captcha
{
    public static class CaptchaExtensions
    {
        /// <summary>
        /// Use Meta Weblog
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="apiUri"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCaptcha(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CaptchaMiddleware>();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Kontext.Services.Captcha
{
    public class CaptchaMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IOptions<CaptchaOptions> options;
        private readonly CaptchaOptions captchaOptions;
        // #e9ecef
        private readonly Color bgColor = Color.FromArgb(0xe9, 0xec, 0xef);
        private readonly Color codeColor = Color.FromArgb(0x00, 0x69, 0xd9);
        //#28a745
        private readonly Color obsColor = Color.FromArgb(0x28, 0xa7, 0x45);

        public CaptchaMiddleware(RequestDelegate next, IOptions<CaptchaOptions> options)
        {
            this.next = next;
            this.options = options;
            captchaOptions = options.Value;
        }

        /// <summary>
        /// Response to the request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rpcService"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(captchaOptions.CaptchaUrl))
            {
                ProcessRequest(context);
                return;
            }
            await next.Invoke(context);
        }

        private void ProcessRequest(HttpContext context)
        {
            // Setup output format
            context.Response.ContentType = "image/gif";
            Random random = new Random();

            // Generate random characters
            StringBuilder s = new StringBuilder();

            // Create the image
            using (Bitmap bitmap = new Bitmap(captchaOptions.ImageWidth, captchaOptions.ImageHeight))
            {
                // Create the graphics 
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    // Write bg color
                    graphics.FillRectangle(new SolidBrush(bgColor), 0, 0, captchaOptions.ImageWidth, captchaOptions.ImageHeight);

                    // Font
                    using (Font font = new Font(FontFamily.GenericSerif, 32, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel))
                    {
                        for (int i = 0; i < captchaOptions.CodeLength; i++)
                        {
                            s.Append(captchaOptions.CaptchaString.Substring(random.Next(0, captchaOptions.CaptchaString.Length - 1), 1));
                            // Write char to the graphic 
                            graphics.DrawString(s[s.Length - 1].ToString(), font, new SolidBrush(codeColor), i * 32, random.Next(0, 24));
                        }
                    }

                    // Add obstructions
                    using (Pen pen = new Pen(new SolidBrush(this.obsColor), 1))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            graphics.DrawLine(pen, new Point(random.Next(0, captchaOptions.ImageWidth - 1), random.Next(0, captchaOptions.ImageHeight - 1)), new Point(random.Next(0, captchaOptions.ImageWidth - 1), random.Next(0, captchaOptions.ImageHeight - 1)));
                        }
                    }
                    for (int i = 0; i < 100; i++)
                    {
                        bitmap.SetPixel(random.Next(captchaOptions.ImageWidth), random.Next(captchaOptions.ImageHeight), Color.FromArgb(random.Next()));
                    }

                    context.Session.Set(captchaOptions.SessionName, Encoding.UTF8.GetBytes(s.ToString().ToLower()));

                    // Save image
                    bitmap.Save(context.Response.Body, System.Drawing.Imaging.ImageFormat.Gif);
                }
            }
        }
    }
}

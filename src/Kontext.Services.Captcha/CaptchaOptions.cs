namespace Kontext.Services.Captcha
{
    public class CaptchaOptions
    {
        public CaptchaOptions()
        {
            CaptchaString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            CaptchaUrl = "/api/captcha";
            CodeLength = 5;
            ImageWidth = 160;
            ImageHeight = 60;
            SessionName = "____________SessionName_Captcha________________";
        }

        public string SessionName { get; set; }

        public string CaptchaUrl { get; set; }

        public string CaptchaString { get; set; }

        public int CodeLength { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }


    }
}

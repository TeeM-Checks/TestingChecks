using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace NewBilletterie
{
    /// <summary>
    /// Summary description for GetUserCaptcha
    /// </summary>
    public class GetUserCaptcha : IHttpHandler, IRequiresSessionState, IReadOnlySessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            MemoryStream memStream = new MemoryStream();

            string phrase = Convert.ToString(context.Session["UserCaptcha"]);

            //Generate an image from the text stored in session
            Bitmap imgCapthca = GenerateImage(220, 70, phrase);
            imgCapthca.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imgBytes = memStream.GetBuffer();

            imgCapthca.Dispose();
            memStream.Close();

            //Write the image as response, so it can be displayed
            context.Response.ContentType = "image/jpeg";
            context.Response.BinaryWrite(imgBytes);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public Bitmap GenerateImage(int Width, int Height, string Phrase)
        {
            Bitmap CaptchaImg = new Bitmap(Width, Height);
            Random Randomizer = new Random();
            Graphics Graphic = Graphics.FromImage(CaptchaImg);
            Graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            //Set height and width of captcha image
            Graphic.FillRectangle(new SolidBrush(Color.Black), 0, 0, Width, Height);

            //Randomly rotate text a little bit
            Random random = new Random();
            float randomNumber = random.Next(-8, 8);

            Graphic.RotateTransform(randomNumber);
            Graphic.DrawString(Phrase, new Font("Verdana", 30), new SolidBrush(Color.White), 15, 15);

            //Add line on top of text
            Point pt1 = new Point(100);
            Point pt2 = new Point(300);
            SolidBrush myBrush = new SolidBrush(Color.Yellow);
            Pen pn = new Pen(myBrush, 4);
            Graphic.DrawLine(pn, 0, 50, 200, 30);
            Graphic.Flush();
            return CaptchaImg;
        }

    }
}
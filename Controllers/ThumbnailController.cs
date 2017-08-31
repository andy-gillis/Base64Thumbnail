using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Web.Http;

namespace Base64Thumbnail.Controllers
{
    public class ThumbnailController : ApiController
    {
        public IHttpActionResult GetThumbnail(string mh, string mw, string url)
        {
            int height = Convert.ToInt32(mh != null ? mh : "180");
            int width = Convert.ToInt32(mw != null ? mw : "240");
            string base64ThumnailData;

            try
            {
                base64ThumnailData = GetBase64ThumbnailDataFromUrl(url, width, height);
            }
            catch
            {
                return this.NotFound();
            }

            return Ok(base64ThumnailData);
        }

        private Image GetImageFromFromUrl(string strUrl)
        {
            Image webImage;

            using (WebResponse wrFileResponse = WebRequest.Create(strUrl).GetResponse())
            {
                using (Stream objWebStream = wrFileResponse.GetResponseStream())
                {
                    MemoryStream ms = new MemoryStream();
                    objWebStream.CopyTo(ms, 8192);
                    webImage = Image.FromStream(ms);
                    ms.Flush();
                    ms.Close();
                }
            }

            return webImage;
        }

        private Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            double ratio = ((double)maxWidth / (double)image.Width);
            int newHeight = (int)(image.Height * ratio);
            int newWidth = (int)(image.Width * ratio);

            if (newHeight > maxHeight)
            {
                ratio = ((double)maxHeight / (double)image.Height);
                newHeight = (int)(image.Height * ratio);
                newWidth = (int)(image.Width * ratio);
            }

            Bitmap newImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        private string GetBase64(Image image)
        {
            byte[] bytearray;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                bytearray = ms.ToArray();
            }

            return Convert.ToBase64String(bytearray);
        }

        private string GetBase64ThumbnailDataFromUrl(String strUrl, int maxWidth, int maxHeight)
        {
            string base64Thumbnail;

            using (Image targetImage = GetImageFromFromUrl(strUrl))
            {
                Image thumbnailImage = ScaleImage(targetImage, maxWidth, maxHeight);
                base64Thumbnail = GetBase64(thumbnailImage);
            }

            return base64Thumbnail;
        }
    }    
}

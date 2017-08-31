using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Base64Thumbnail.Controllers
{
    public class PrimaryImageController : ApiController
    {
        public IHttpActionResult GetPrimaryImage(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebClient client = new WebClient();

            string html = String.Empty;
            string transGif = "data:image/gif;base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs=";
            string primaryImageSrc = transGif;

            try 
            {
                html = client.DownloadString(url);
                primaryImageSrc = GetPrimaryImageSrcFromUrl(url, html, transGif);
            }
            catch 
            { 
            }

            return Ok(primaryImageSrc);
        }

        private string GetPrimaryImageSrcFromUrl(string url, string html, string transGif)
        {
            string primaryImageSrc = transGif;

            try 
            {
                HtmlDocument document = new HtmlDocument();
                IEnumerable<HtmlNode> images = null;

                int currentSize = 0;
                string[] urlParts = Regex.Split(url, "://");
                string proto = urlParts[0] + ":";
                string host = proto + "//" + Regex.Split(urlParts[1], "/")[0];
                string path = url.Substring(0, url.LastIndexOf("/")) + "/";
                string[] elementNames = { "article", "main", "section", "div", "body" };

                document.LoadHtml(html);
                images = GetImageHtmlNodeCollectionFromElements(document, elementNames);

                foreach (HtmlNode image in images)
                {
                    try 
                        {
                            string imgSource = GetAbsoluteImageSource(image, proto, host, path, transGif);
                            int contentLength = GetImageContentLength(imgSource);
                            if (contentLength > currentSize)
                        {
                            primaryImageSrc = imgSource;
                            currentSize = contentLength;
                        } 
                    }
                    catch 
                    { 
                    }
                }
            }
            catch 
            { 
            }

            return primaryImageSrc;
        }

        private int GetImageContentLength(string imgSource)
        {
            try
            {
                WebRequest req = HttpWebRequest.Create(imgSource);
                req.Method = "HEAD";
                using (WebResponse resp = req.GetResponse())
                {
                    int contentLength;
                    if (int.TryParse(resp.Headers.Get("Content-Length"), out contentLength))
                    {
                        return contentLength;
                    }
                }
            }
            catch
            {
            }

            return 0;
        }

        private string GetAbsoluteImageSource(HtmlNode image, string proto, string host, string path, string transGif)
        {
            string source = image.GetAttributeValue("src", transGif);

            if (!source.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                if (source.StartsWith("//"))
                {
                    source = proto + source;
                }
                else if (source.StartsWith("/"))
                {
                    source = host + source;
                }
                else if (!source.StartsWith(transGif, StringComparison.InvariantCultureIgnoreCase))
                {
                    source = path + source;
                }
            }

            return source;
        }

        private IEnumerable<HtmlNode> GetImageHtmlNodeCollectionFromElements(HtmlDocument document, string[] elementNames)
        {
            int i = 0;
            int max = elementNames.Length;
            IEnumerable<HtmlNode> htmlNodeCollection = null;
            string elementName;

            for (; i < max; i++)
            {
                elementName = elementNames[i];
                htmlNodeCollection = GetImageHtmlNodeCollectionFromElement(document, elementName);

                if (htmlNodeCollection.Count() > 0)
                {
                    return htmlNodeCollection;
                }
            }

            return htmlNodeCollection;
        }

        private IEnumerable<HtmlNode> GetImageHtmlNodeCollectionFromElement(HtmlDocument document, string elementName)
        {
            return (
                from d in document.DocumentNode.Descendants()
                where d.Name == "img" && d.Ancestors(elementName).Count() > 0
                select d);
        }
    }
}

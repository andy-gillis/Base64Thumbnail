using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using Base64Thumbnail.Classes;
//using System.Threading;
//using System.Security.Permissions;

namespace Base64Thumbnail.Controllers
{

    //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class PrimaryContentController : ApiController
    {
        //private string _url = String.Empty;
        //private string _html = String.Empty;
        public IHttpActionResult GetPrimaryContent(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebClient client = new WebClient() { Encoding = System.Text.Encoding.UTF8 };
            HtmlDocument document = new HtmlDocument();

            string html = String.Empty;
            string primaryImageUrl = String.Empty;
            string primaryHeading = String.Empty;
            string transGif = "data:image/gif;base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs=";
            string[] elementNames = { "heading", "article", "main", "section", "div", "body" };
            WebContent webContent;

            html = client.DownloadString(url);

            /* try loading page and executing its script */
            //_url = url;
            //Thread thread = new Thread(new ThreadStart(GetWebPageSource));
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
            //thread.Join();
            //html = _html;

            document.LoadHtml(html);

            primaryImageUrl = GetPrimaryImageSrcFromUrlOrMeta(document, url, transGif);
            primaryHeading = WebUtility.HtmlDecode(GetPrimaryHeadingFromElementsOrMeta(document, elementNames));
            webContent = new WebContent(primaryImageUrl, primaryHeading);

            return Ok(webContent);
        }

        //protected void GetWebPageSource()
        //{
        //    using (System.Windows.Forms.WebBrowser browser = new System.Windows.Forms.WebBrowser())
        //    {
        //        int numberOfMilisecondsToSleep = 12999;
        //        browser.ScriptErrorsSuppressed = true;
        //        browser.AllowNavigation = true;
        //        browser.ScrollBarsEnabled = false;
        //        browser.Navigate(_url);
        //        while (browser.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
        //        {
        //            System.Windows.Forms.Application.DoEvents();
        //            System.Threading.Thread.Sleep(1000); // allow dom changes
        //        }
        //        System.Threading.Thread.Sleep(numberOfMilisecondsToSleep); // allow dom changes
        //        browser.Stop();
        //        _html = browser.Document.GetElementsByTagName("body")[0].OuterHtml;
        //        browser.Dispose();
        //    }
        //}

        private string GetOpenGraphicsMetaContentFromDocument(HtmlDocument document, string prop)
        {
            string metaProp = String.Empty;
            IEnumerable<HtmlNode> htmlNodes = (
                from d in document.DocumentNode.Descendants()
                where d.Name == "meta" && d.Attributes["property"] != null && d.Attributes["property"].Value == prop
                select d);

            if (htmlNodes != null && htmlNodes.Count() > 0)
            {
                metaProp = htmlNodes.First().Attributes["content"].Value;
            }

            return metaProp;
        }

        private string GetPrimaryHeadingFromElementsOrMeta(HtmlDocument document, string[] elementNames)
        {
            string primaryHeading = GetOpenGraphicsMetaContentFromDocument(document, "og:title");
            if (primaryHeading != null && primaryHeading.Length > 0)
            {
                return primaryHeading;
            }

            int i = 0;
            int maxi = elementNames.Length;
            string elementName = String.Empty;
            HtmlNode htmlNode = null;

            for (; i < maxi; i++)
            {
                elementName = elementNames[i];
                htmlNode = GetPrimaryHeadingFromElement(document, elementName, "h1");

                if (htmlNode != null && htmlNode.InnerText.Length > 0)
                {
                    return htmlNode.InnerText;
                }
            }

            htmlNode = GetPrimaryHeadingFromElement(document, "html", "title");
            if (htmlNode != null && htmlNode.InnerText.Length > 0)
            {
                return htmlNode.InnerText;
            }

            return String.Empty;
        }

        private HtmlNode GetPrimaryHeadingFromElement(HtmlDocument document, string elementName, string headingName)
        {
            IEnumerable<HtmlNode> htmlNodes = (
                from d in document.DocumentNode.Descendants()
                where d.Name == headingName && d.Ancestors(elementName).Count() > 0
                select d);

            if (htmlNodes != null && htmlNodes.Count() > 0)
            {
                return htmlNodes.First();
            }

            return null;
        }

        private string GetPrimaryImageSrcFromUrlOrMeta(HtmlDocument document, string url, string transGif)
        {
            string primaryImageSrc = GetOpenGraphicsMetaContentFromDocument(document, "og:image");
            if (primaryImageSrc != null && primaryImageSrc.Length > 0)
            {
                return primaryImageSrc;
            }

            primaryImageSrc = transGif;

            try
            {
                int currentSize = 0;
                string[] urlParts = Regex.Split(url, "://");
                string proto = urlParts[0] + ":";
                string host = proto + "//" + Regex.Split(urlParts[1], "/")[0];
                string path = url.Substring(0, url.LastIndexOf("/")) + "/";
                string[] elementNames = { "article", "main", "section", "div", "body" };
                IEnumerable<HtmlNode> images = null;

                // TODO: find method for locating primary image on portal pages
                if (path == host + "/" || path == proto + "//")
                {
                    return primaryImageSrc;
                }

                images = GetImageHtmlNodeCollectionFromElements(document, elementNames);

                foreach (HtmlNode image in images)
                {
                    try
                    {
                        string imgSource = GetAbsoluteImageSource(image, proto, host, path, transGif);
                        int contentLength = GetImageContentLength(imgSource);
                        if (contentLength > currentSize * 3)
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
                else if (!source.StartsWith("data:image", StringComparison.InvariantCultureIgnoreCase))
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


namespace Base64Thumbnail.Classes
{
    public class WebContent
    {
        public string primaryImageUrl { get; set; }
        public string primaryHeading { get; set; }

        public WebContent(string imageUrl, string heading)
        {
            primaryImageUrl = imageUrl;
            primaryHeading = heading;
        }
    }
}
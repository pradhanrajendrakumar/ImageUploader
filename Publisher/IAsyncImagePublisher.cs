using ImageDirectory.Models;

namespace ImageDirectory.Publisher
{
    public interface IAsyncImagePublisher
    {
        void PublishImage(Image image);
    }
}
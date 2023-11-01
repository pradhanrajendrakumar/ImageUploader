namespace ImageDirectory.Models
{
    public class Image
    {
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public required byte[] ImageData { get; set; }
    }
}

namespace FlowerShop.Utility
{
    public class BadRequestException : Exception //400
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}

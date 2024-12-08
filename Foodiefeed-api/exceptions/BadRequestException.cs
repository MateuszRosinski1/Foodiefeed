namespace Foodiefeed_api.exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {         
        }
    }
}

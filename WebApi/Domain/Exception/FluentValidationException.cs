namespace WebApi.Domain.Exception
{
    public class FluentValidationException : System.Exception
    {
        public FluentValidationException(string[] messages)
        {
            Messages = messages;
        }

        public string[] Messages { get; set; }
    }
}

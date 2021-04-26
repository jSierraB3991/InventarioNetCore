namespace WebApi.Domain.Exceotion
{
    public class BusinessException : System.Exception
    {
        public BusinessException(string message) : base(message)
        {

        }
    }
}

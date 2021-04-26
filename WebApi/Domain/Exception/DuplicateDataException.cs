namespace WebApi.Domain.Exception
{
    public class DuplicateDataException: System.Exception
    {
        public DuplicateDataException(string message): base(message)
        {

        }
    }
}

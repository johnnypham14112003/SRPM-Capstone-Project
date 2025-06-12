namespace SRPM_Services.Extensions.Exceptions
{
    public class ConflictException : ApplicationException
    {
        public ConflictException(string message) : base(message)
        {
            //409: Cannot complete because existed
        }
    }
}

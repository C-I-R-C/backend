
namespace WebApplication1.Exceptions
{

    public class BoxNotFoundException : Exception
    {

        public BoxNotFoundException() : base("No such box") { }

        public BoxNotFoundException(string message) : base(message) { }

    }


    public class BusinessRuleException : Exception
    {

        public BusinessRuleException(string message) : base(message) { }

    }


    public class AppValidationException : Exception
    {

        public AppValidationException(string message) : base(message) { }

    }


    public class DatabaseOperationException : Exception
    {

        public DatabaseOperationException(string message) : base(message) { }

        public DatabaseOperationException(string message, Exception innerException) : base(message, innerException) { }

    }

}
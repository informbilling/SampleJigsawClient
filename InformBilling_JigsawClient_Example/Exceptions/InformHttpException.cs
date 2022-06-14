using System.Net;

namespace InformBilling_JigsawClient_Example.Exceptions
{
    public class InformHttpException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public object ExceptionObject { get; }

        public InformHttpException()
            : base() { }

        public InformHttpException(string message, Exception innerException)
            : base(message, innerException) { }

        public InformHttpException(HttpStatusCode statusCode, object exceptionObject)
            : base()
        {
            StatusCode = statusCode;
            ExceptionObject = exceptionObject;
        }

    }
}
namespace MEVSharp.Application.Exceptions
{
    public class HttpException : Exception
    {
        public HttpException(HttpResponseMessage response, string? message)
            : base(message)
        {
            Response = response;
        }

        public HttpResponseMessage Response { get; }
    }
}

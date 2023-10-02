using MEVSharp.Application.Models;

namespace MEVSharp.Application.Exceptions
{
    public class ParentHashMismatchException : Exception
    {
        public ParentHashMismatchException(EthHeader ethHeader, string? message)
            : base(message)
        {
            EthHeader = ethHeader;
        }

        public EthHeader EthHeader { get; }
    }

    public class EthHeaderBuildValidationException : Exception
    {
        public EthHeaderBuildValidationException(EthHeader ethHeader, string? message)
            : base(message)
        {
            EthHeader = ethHeader;
        }

        public EthHeader EthHeader { get; }
    }
}

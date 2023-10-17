using System.Net;

namespace MEVSharp.Application.Models.Validators
{
    public interface IRegisterValidator
    {
        bool Validate(IEnumerable<HttpStatusCode> v);
    }

    public class RegisterValidator : IRegisterValidator
    {
        public RegisterValidator() { }

        public bool Validate(IEnumerable<HttpStatusCode> v)
        {
            return v.Any(px => px == HttpStatusCode.OK);
        }
    }
}

using System.Net;

namespace MEVSharp.Application.Models
{
    public class EthStatus
    {
        public readonly HttpStatusCode DTO;
        public HttpStatusCode StatusCode { get; private set; }

        public EthStatus(HttpStatusCode dto)
        {
            this.DTO = dto;
            Build();
        }

        private void Build()
        {
            this.StatusCode = DTO;
        }
    }
}

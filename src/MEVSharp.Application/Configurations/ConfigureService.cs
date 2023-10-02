using MEVSharp.Application.Models;
using MEVSharp.Application.Models.Validators;
using MEVSharp.Application.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MEVSharp.Application.Configurations
{
    public static class ConfigureService
    {
        public static void AddApplication(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddAutoMapper(typeof(MEVSharp.Application.MapperProfile));


            services.AddScoped<IRelayProvider, RelayProvider>();
            services.AddScoped<IRegisterValidator, RegisterValidator>();
            services.AddScoped<ISignatureVerification, SignatureVerification>();
        }
    }
}

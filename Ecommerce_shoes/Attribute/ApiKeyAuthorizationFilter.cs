
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce_shoes.Attribute
{
    public class ApiKeyAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
      

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}

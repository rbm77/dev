using Buslogix.Models;
using System.Security.Claims;

namespace Buslogix.Utilities
{
    public static class HttpContextExtension
    {

        public static int GetCompanyId(this HttpContext context)
        {
            if (context.Items.TryGetValue("CompanyId", out object? value) && value is int id)
            {
                return id;
            }
            throw new InvalidOperationException("Company Id not found.");
        }

        public static int GetUserId(this HttpContext context)
        {
            return int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}

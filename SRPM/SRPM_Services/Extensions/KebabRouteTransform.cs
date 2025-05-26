using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace SRPM_Services.Extensions
{
    public class KebabRouteTransform : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            string str = value is null ? "" : value.ToString()!;
            if (string.IsNullOrEmpty(str)) { return null; }

            return Regex.Replace(str, "([a-z])([A-Z])", "$1-$2").ToLower();
        }

    }
}

using Microsoft.AspNetCore.Builder;
namespace dhc;
public static class ForwardedPrefixBasePathExtensionMethods
{
    public static IApplicationBuilder UseForwardedPrefixBasePath(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ForwardedPrefixBasePathMiddleware>();
        return builder;
    }
}

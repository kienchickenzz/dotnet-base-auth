namespace AuthService.Web.Infrastructure.Mvc;

/// <summary>
/// Extension methods for configuring MVC with Feature Folder pattern.
/// </summary>
public static class MvcServiceCollectionExtensions
{
    /// <summary>
    /// Configures MVC with Feature Folder pattern for Areas.
    ///
    /// Structure:
    /// Areas/{Area}/Features/{Feature}/Controllers/{Controller}.cs
    /// Areas/{Area}/Features/{Feature}/Views/{Action}.cshtml
    /// Areas/{Area}/Features/{Feature}/Models/{Model}.cs
    /// Areas/{Area}/Shared/{SharedViews}.cshtml
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="rootNamespace">Root namespace of the web project (default: BaseCustomMvc.Web).</param>
    public static IServiceCollection AddFeatureFoldersMvc(
        this IServiceCollection services,
        string rootNamespace = "BaseCustomMvc.Web")
    {
        services.AddControllersWithViews(options =>
        {
            // Convention to auto-detect Area from namespace
            options.Conventions.Add(
                new FeatureFolderControllerModelConvention(rootNamespace));
        })
        .AddRazorOptions(options =>
        {
            // Custom view location expander for feature folders
            options.ViewLocationExpanders.Add(new FeatureFolderViewLocationExpander());

            // Configure Area View Location Formats
            options.AreaViewLocationFormats.Clear();
            // Nested feature folder (Auth)
            options.AreaViewLocationFormats.Add("/Areas/{2}/Features/Auth/{1}/Views/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/Areas/{2}/Features/Auth/{1}/Views/Shared/{0}.cshtml");
            // Standard feature folder
            options.AreaViewLocationFormats.Add("/Areas/{2}/Features/{1}/Views/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/Areas/{2}/Features/{1}/Views/Shared/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/Areas/{2}/Shared/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
        });

        return services;
    }
}

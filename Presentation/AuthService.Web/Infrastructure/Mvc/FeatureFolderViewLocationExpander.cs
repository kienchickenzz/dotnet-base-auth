namespace AuthService.Web.Infrastructure.Mvc;

using Microsoft.AspNetCore.Mvc.Razor;

/// <summary>
/// Custom view location expander for Feature Folder pattern in Areas.
///
/// Supports finding Views with structure:
/// - Areas/{Area}/Features/{Controller}/Views/{Action}.cshtml
/// - Areas/{Area}/Features/{Controller}/Views/Shared/{View}.cshtml
/// - Areas/{Area}/Shared/{View}.cshtml
/// </summary>
public class FeatureFolderViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
        // No custom cache key needed - locations are fixed by convention
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        // {0} = Action/View name
        // {1} = Controller name
        // {2} = Area name

        // Only apply custom locations when Area is present
        if (string.IsNullOrEmpty(context.AreaName))
        {
            return viewLocations;
        }

        var featureFolderLocations = new[]
        {
            // === AREA + NESTED FEATURE FOLDER (Auth) ===
            // Nested: Areas/{Area}/Features/Auth/{Controller}/Views/{Action}.cshtml
            "/Areas/{2}/Features/Auth/{1}/Views/{0}.cshtml",

            // Nested shared: Areas/{Area}/Features/Auth/{Controller}/Views/Shared/{View}.cshtml
            "/Areas/{2}/Features/Auth/{1}/Views/Shared/{0}.cshtml",

            // === AREA + FEATURE FOLDER ===
            // Primary: Areas/{Area}/Features/{Controller}/Views/{Action}.cshtml
            "/Areas/{2}/Features/{1}/Views/{0}.cshtml",

            // Feature-level shared: Areas/{Area}/Features/{Controller}/Views/Shared/{View}.cshtml
            "/Areas/{2}/Features/{1}/Views/Shared/{0}.cshtml",

            // Area-level shared: Areas/{Area}/Shared/{View}.cshtml
            "/Areas/{2}/Shared/{0}.cshtml",

            // === FALLBACK TO GLOBAL ===
            // Global shared: Views/Shared/{View}.cshtml
            "/Views/Shared/{0}.cshtml",
        };

        return featureFolderLocations.Concat(viewLocations);
    }
}

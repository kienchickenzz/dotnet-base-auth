namespace AuthService.Web.Infrastructure.Mvc;

using Microsoft.AspNetCore.Mvc.ApplicationModels;

/// <summary>
/// Convention to automatically set Area name for Controllers in Feature Folders.
///
/// Determines Area from namespace:
/// {RootNamespace}.Areas.{Area}.Features.{Feature}.Controllers.{Controller}
/// </summary>
public class FeatureFolderControllerModelConvention : IControllerModelConvention
{
    private readonly string _rootNamespace;

    public FeatureFolderControllerModelConvention(string rootNamespace)
    {
        _rootNamespace = rootNamespace;
    }

    public void Apply(ControllerModel controller)
    {
        var controllerNamespace = controller.ControllerType.Namespace;

        if (string.IsNullOrEmpty(controllerNamespace))
            return;

        // Pattern: {RootNamespace}.Areas.{Area}.Features.{Feature}.Controllers
        var areasPrefix = $"{_rootNamespace}.Areas.";

        if (!controllerNamespace.StartsWith(areasPrefix))
            return;

        // Extract Area name from namespace
        var afterAreas = controllerNamespace.Substring(areasPrefix.Length);
        var segments = afterAreas.Split('.');

        if (segments.Length < 1)
            return;

        var areaName = segments[0]; // First segment = Area name

        // Set Area if not already set by [Area] attribute
        var hasExistingArea = controller.RouteValues.TryGetValue("area", out var existingArea);
        if (!hasExistingArea || string.IsNullOrEmpty(existingArea))
        {
            controller.RouteValues["area"] = areaName;
        }
    }
}

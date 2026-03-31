namespace AuthService.Application.Features.Identities.Roles;

using System.Collections.ObjectModel;


public static class Actions
{
    public const string View = nameof(View);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
}

public static class Resource
{
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Products = nameof(Products);
    public const string Categories = nameof(Categories);
}

public static class Permissions
{
    private static readonly Permission[] _all = new Permission[]
    {
        new("View Users", Actions.View, Resource.Users),
        new("Create Users", Actions.Create, Resource.Users),
        new("Update Users", Actions.Update, Resource.Users),
        new("Delete Users", Actions.Delete, Resource.Users),
        new("Export Users", Actions.Export, Resource.Users),
        new("View UserRoles", Actions.View, Resource.UserRoles),
        new("Update UserRoles", Actions.Update, Resource.UserRoles),
        new("View Roles", Actions.View, Resource.Roles),
        new("Create Roles", Actions.Create, Resource.Roles),
        new("Update Roles", Actions.Update, Resource.Roles),
        new("Delete Roles", Actions.Delete, Resource.Roles),
        new("View RoleClaims", Actions.View, Resource.RoleClaims),
        new("Update RoleClaims", Actions.Update, Resource.RoleClaims),
        new("View Products", Actions.View, Resource.Products, IsCustomer: true),
        new("Create Products", Actions.Create, Resource.Products),
        new("Update Products", Actions.Update, Resource.Products),
        new("Delete Products", Actions.Delete, Resource.Products),
        new("Export Products", Actions.Export, Resource.Products),
        new("View Categories", Actions.View, Resource.Categories, IsCustomer: true),
        new("Create Categories", Actions.Create, Resource.Categories),
        new("Update Categories", Actions.Update, Resource.Categories),
        new("Delete Categories", Actions.Delete, Resource.Categories),
        new("Export Categories", Actions.Export, Resource.Categories),
    };

    public static IReadOnlyList<Permission> Admin { get; } = new ReadOnlyCollection<Permission>(_all);
    public static IReadOnlyList<Permission> Customer { get; } = new ReadOnlyCollection<Permission>(_all.Where(p => p.IsCustomer).ToArray());
}

public record Permission(string Description, string Action, string Resource, bool IsCustomer = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
}

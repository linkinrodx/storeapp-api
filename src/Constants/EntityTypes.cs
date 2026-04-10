namespace StoreApp.Api.Constants;

public static class EntityTypes
{
    public const string Product = "product";
    public const string Family = "family";
    public const string Category = "category";
    public const string Brand = "brand";

    public static readonly string[] All = [Product, Family, Category, Brand];

    public static bool IsValid(string entityType) => All.Contains(entityType);
}
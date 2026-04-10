namespace StoreApp.Api.Constants;

public static class OrderStatus
{
    public const string Pending = "pending";
    public const string Processing = "processing";
    public const string Shipped = "shipped";
    public const string Delivered = "delivered";
    public const string Cancelled = "cancelled";

    public static readonly string[] All = [Pending, Processing, Shipped, Delivered, Cancelled];

    public static bool IsValid(string status) => All.Contains(status);
}
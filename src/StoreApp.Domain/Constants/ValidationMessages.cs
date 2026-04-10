namespace StoreApp.Domain.Constants;

public static class ValidationMessages
{
    public const string Required = "El campo {0} es requerido.";
    public const string InvalidFormat = "El campo {0} tiene un formato inválido.";
    public const string MinLength = "El campo {0} debe tener al menos {1} caracteres.";
    public const string MaxLength = "El campo {0} debe tener como máximo {1} caracteres.";
    public const string Range = "El campo {0} debe estar entre {1} y {2}.";
    public const string Email = "El campo {0} debe ser un correo electrónico válido.";
    public const string Guid = "El campo {0} debe ser un GUID válido.";
}
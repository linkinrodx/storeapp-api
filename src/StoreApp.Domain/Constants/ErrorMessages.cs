namespace StoreApp.Domain.Constants;

public static class ErrorMessages
{
    public const string SlugAlreadyInUse = "El slug ya está en uso.";
    public const string EntityNotFound = "Entidad no encontrada.";
    public const string InvalidEntityType = "entity_type debe ser uno de: {0}";
    public const string InvalidOrderStatus = "Estado inválido. Debe ser uno de: {0}";
    public const string CannotCancelShippedOrDelivered = "No se puede cancelar un pedido que ya fue enviado o entregado.";
    public const string GenericConflict = "Ya existe un conflicto con los datos proporcionados.";
    public const string GenericBadRequest = "Solicitud inválida.";
    public const string GenericUnauthorized = "No autorizado.";
    public const string GenericForbidden = "Prohibido.";
    public const string GenericInternalError = "Error interno del servidor.";
}
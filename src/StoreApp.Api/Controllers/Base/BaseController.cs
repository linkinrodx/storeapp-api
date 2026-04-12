using Microsoft.AspNetCore.Mvc;
using StoreApp.Domain.DTOs.Responses;

namespace StoreApp.Domain.Controllers.Base;

/// <summary>
/// Base controller providing common response helper methods.
/// </summary>
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Returns a successful OK response with data.
    /// </summary>
    protected ActionResult<T> HandleSuccess<T>(T data) => Ok(data);

    /// <summary>
    /// Returns a Created response with location header.
    /// </summary>
    protected ActionResult<T> HandleCreated<T>(T data, string actionName, object routeValues) => CreatedAtAction(actionName, routeValues, data);

    /// <summary>
    /// Returns a NotFound response.
    /// </summary>
    protected ActionResult<T> HandleNotFound<T>() => NotFound();

    /// <summary>
    /// Returns a Conflict response.
    /// </summary>
    protected ActionResult<T> HandleConflict<T>(T error) => Conflict(error);

    /// <summary>
    /// Returns a BadRequest response.
    /// </summary>
    protected ActionResult<T> HandleBadRequest<T>(T error) => BadRequest(error);

    /// <summary>
    /// Returns a NoContent response.
    /// </summary>
    protected IActionResult HandleNoContent() => NoContent();

    /// <summary>
    /// Returns a paged response.
    /// </summary>
    protected ActionResult<T> HandlePaged<T>(PagedResponse<T> response) => Ok(response);
}
using Microsoft.AspNetCore.Mvc;

namespace StoreApp.Api.Controllers.Base;

public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleSuccess<T>(T data) => Ok(data);
    protected IActionResult HandleCreated<T>(T data, string actionName, object routeValues) => CreatedAtAction(actionName, routeValues, data);
    protected IActionResult HandleNotFound() => NotFound();
    protected IActionResult HandleConflict<T>(T error) => Conflict(error);
    protected IActionResult HandleBadRequest<T>(T error) => BadRequest(error);
    protected IActionResult HandleNoContent() => NoContent();
    protected IActionResult HandlePaged<T>(StoreApp.Api.DTOs.Responses.PagedResponse<T> response) => Ok(response);
}
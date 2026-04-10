using Microsoft.AspNetCore.Mvc;

namespace StoreApp.Api.Controllers.Base;

public abstract class BaseController : ControllerBase
{
    protected ActionResult<T> HandleSuccess<T>(T data) => Ok(data);
    protected ActionResult<T> HandleCreated<T>(T data, string actionName, object routeValues) => CreatedAtAction(actionName, routeValues, data);
    protected ActionResult<T> HandleNotFound<T>() => NotFound();
    protected ActionResult<T> HandleConflict<T>(T error) => Conflict(error);
    protected ActionResult<T> HandleBadRequest<T>(T error) => BadRequest(error);
    protected IActionResult HandleNoContent() => NoContent();
    protected ActionResult<T> HandlePaged<T>(StoreApp.Api.DTOs.Responses.PagedResponse<T> response) => Ok(response);
}
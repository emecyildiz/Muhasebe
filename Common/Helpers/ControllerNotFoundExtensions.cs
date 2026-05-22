using Microsoft.AspNetCore.Mvc;
using Muhasebe.Models;

namespace Muhasebe.Common.Helpers;

public static class ControllerNotFoundExtensions
{
    public static IActionResult RecordNotFound(
        this Controller controller,
        string entityName,
        string listController,
        string listAction = "Index",
        int? requestedId = null,
        string? message = null,
        string? listLabel = null)
    {
        controller.Response.StatusCode = StatusCodes.Status404NotFound;
        return controller.View("RecordNotFound", new NotFoundViewModel
        {
            EntityName = entityName,
            Message = message,
            RequestedId = requestedId,
            ListController = listController,
            ListAction = listAction,
            ListLabel = listLabel
        });
    }
}

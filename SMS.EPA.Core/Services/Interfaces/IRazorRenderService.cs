namespace SMS.EPA.Core.Services.Interfaces
{
    using System.Web.Mvc;

    public interface IRazorRenderService
    {
        string RenderViewToString(Controller controller, string viewPath, object model);
    }
}
namespace SMS.EPA.Core.Services
{
    using System.Globalization;
    using System.IO;
    using System.Web.Mvc;

    public class RazorRenderService : IRazorRenderService
    {
        public string RenderViewToString(Controller controller, string viewPath, object model)
        {
            controller.ViewData.Model = model;

            using (var sw = new StringWriter(CultureInfo.CurrentCulture))
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewPath);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
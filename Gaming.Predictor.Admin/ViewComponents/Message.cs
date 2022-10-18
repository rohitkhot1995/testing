using Microsoft.AspNetCore.Mvc;
using System;

namespace Gaming.Predictor.Admin.ViewComponents
{
    public class MessageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(String component, Object message)
        {
            if (!String.IsNullOrEmpty(component))
                return View($"/Views/Partial/Message/{component}.cshtml", message);
            else
                return Content("");
        }
    }
}
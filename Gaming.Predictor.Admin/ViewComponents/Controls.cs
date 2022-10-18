﻿using Microsoft.AspNetCore.Mvc;
using System;

namespace Gaming.Predictor.Admin.ViewComponents
{
    public class ControlsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(String component, Object model)
        {
            return View($"/Views/Partial/{component}.cshtml", model);
        }
    }
}
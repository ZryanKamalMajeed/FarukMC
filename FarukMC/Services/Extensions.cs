using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FarukMC.Services
{
    public static class Extensions
    {

        public static string IsActive(this IHtmlHelper htmlHelper, string controller
            //, string action
        )
        {
            var routeData = htmlHelper.ViewContext.RouteData;

            //var routeAction = routeData.Values["action"]?.ToString();
            var routeController = routeData.Values["controller"]?.ToString();

            var returnActive = (controller == routeController 
                                //&& (action == routeAction || routeAction == "Details")
                                )
                ;

            return returnActive ? "active" : "";
        }

        public static string ToJson(bool inputVal)
        {
            return inputVal ? "true" : "false";
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace StoreWebSite.MVC.Helpers.HtmlHelpers
{
    //Custom html helper to greet users.
    public static class HtmlHelpers
    {
        public static string DayTimeGreeting(this IHtmlHelper helper, string name)
        {
            //custom greeting based on the hour of the day.
            var hour = DateTime.Now.Hour;
            if(hour >= 0 && hour < 12)
            {
                return $"Good morning {name}";
            }
            if(hour >= 12 && hour < 18)
            {
                return $"Good afternoon {name}";
            }
            return $"Good evening {name}";
        }
    }
}

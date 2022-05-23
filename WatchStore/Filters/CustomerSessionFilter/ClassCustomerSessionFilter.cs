using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace WatchStore.Filters.CustomerSessionFilter
{
    public class ClassCustomerSessionFilter: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["role"] != null)
            {
                string role = filterContext.HttpContext.Session["role"].ToString();
                if (role == "customer")
                {

                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                     {
                    { "area", "" },
                    { "controller", "Auth" },
                    { "action", "Login" }
                     });

                }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                     {
                    { "area", "" },
                    { "controller", "Auth" },
                    { "action", "Login" }
                     });
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {

        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {

        }
    }
}

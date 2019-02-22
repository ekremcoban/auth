using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EsahaPlusApi.Attributes
{
    //public class AjaxOnlyAttribute : ActionMethodSelectorAttribute,IFilterMetadata
    //{
    //    public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
    //    {
    //        return routeContext.HttpContext.Request?.Headers["X-Requested-With"].ToString() == "XMLHttpRequest";
    //    }
    //}

    /*
     * Filter to prevent requests except ajax. Return Not Found.
     * */
    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request?.Headers["X-Requested-With"].ToString() != "XMLHttpRequest")
            {
                filterContext.Result = new NotFoundResult();
            }

            //if (!filterContext.HttpContext.Request.IsAjaxRequest())
            //{
            //    filterContext.Result = new HttpNotFoundResult();
            //}
        }
    }
}

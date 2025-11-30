using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Query;

namespace Renoza.Backend.Attributes
{
    public class UseQueryAttribute : EnableQueryAttribute
    {
        /// <summary>
        /// Performs the query composition after action is executed. It first tries to retrieve the IQueryable from the
        /// returning response message. It then validates the query from uri based on the validation settings on
        /// <see cref="T:Microsoft.AspNet.OData.EnableQueryAttribute" />. It finally applies the query appropriately, and reset it back on
        /// the response message.
        /// </summary>
        /// <param name="actionExecutedContext">The context related to this action, including the response message,
        /// request message and HttpConfiguration etc.</param>
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            var result = actionExecutedContext.Result;
            if (result == null) return;

            var typeProperty = result.GetType().GetProperty("Value");
            var dataResult = typeProperty?.GetValue(result);
            var count = actionExecutedContext.HttpContext.Request.ODataFeature().TotalCount;

            if (actionExecutedContext.Result is ObjectResult responseContent)
            {
                if (count.HasValue && count > 0)
                {
                    responseContent.StatusCode = (int)HttpStatusCode.OK;
                    responseContent.Value = new { data = dataResult, totalCount = count };
                }
            }
        }
    }
}

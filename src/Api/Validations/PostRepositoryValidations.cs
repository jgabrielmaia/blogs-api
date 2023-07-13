using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Api.Validations
{
    public class PostExistsActionFilterAttribute : ActionFilterAttribute
    {
        // TODO: Explore possibilities at https://www.devtrends.co.uk/blog/dependency-injection-in-action-filters-in-asp.net-core
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.ContainsKey("id") &&
                context.ActionArguments["id"] is Guid postId)
            {
                var repository = context.HttpContext.RequestServices.GetService<IPostRepository>();
                if (repository == null)
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    return;
                }
                var post = repository.Get(postId);
                if (post == null)
                {
                    context.Result = new BadRequestObjectResult("Post should exist.");
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }

}
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.Options;

namespace ApiTracker
{
    public class ApiTracker : ActionFilterAttribute
    {
        private readonly ElasticClient elastic;

        private readonly string Key = "_ApiTracker_";

        public ApiTracker(IOptions<ApiTrackerSetting> config)
        {
            var url = config.Value.ElasticConnection;

            elastic = new ElasticClient(url, config.Value.DocumentName, config.Value.TimeOut);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var Descriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (Descriptor == null)
            {
                base.OnActionExecuting(context);

                return;
            }

            var log = new ApiTrackerEntity()
            {
                StartTime = DateTime.Now,
                Action = Descriptor.ActionName,
                Controller = Descriptor.ControllerName,
                Method = Descriptor.MethodInfo.Name.ToLower(),
                Params = string.Empty,
            };

            var options = Descriptor.MethodInfo.GetCustomAttribute<ApiTrackerOptions>(true) ?? new ApiTrackerOptions();

            if (options.EnableHeaders)
            {
                log.Headers = JsonConvert.SerializeObject(context.HttpContext.Request.Headers);
            }

            if (options.EnableClientIp)
            {
                log.ClientIp = context.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            if (options.EnableParams)
            {
                #region Request Url Params 
                if (context.ActionArguments != null &&
                    context.ActionArguments.Keys.Count > 0)
                {
                    log.Params = JsonConvert.SerializeObject(context.ActionArguments);
                }
                #endregion

                #region Request Body Params 
                if (context.HttpContext.Request.HasFormContentType)
                {
                    log.Params += "&RequestForm=" + JsonConvert.SerializeObject(context.HttpContext.Request.Form);
                }
                #endregion
            }

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = context.HttpContext.User;

                log.CallerName = user.Identity.Name;

                if (user.Claims.Count() > 0)
                {
                    var client = user.Claims.FirstOrDefault(x => x.Type.Equals("client_id"));

                    if (client != null)
                    {
                        log.client_id = client.Value;
                    }
                }
            }

            context.HttpContext.Items[Key] = log;

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var logObject = context.HttpContext.Items[Key];

            if (logObject == null)
            {
                base.OnActionExecuted(context);

                return;
            }

            var log = logObject as ApiTrackerEntity;

            log.EndTime = DateTime.Now;

            log.UsedSeconds = (log.EndTime - log.StartTime).TotalSeconds;

            if (context.Exception != null)
            {
                log.Error = context.Exception.Message + context.Exception.StackTrace;
            }

            elastic.Index(JsonConvert.SerializeObject(log));

            base.OnActionExecuted(context);
        }
    }
}

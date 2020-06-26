using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Extensions;

namespace Yandex.Dj.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class RootController : Controller
    {
        // GET api/methods
        [HttpGet("methods")]
        [Description("Справка по предоставляемому сервисом API")]
        public object GetMethods()
        {
            JObject apiList = new JObject();
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes().Where(t => t.Name.IsMatch("Controller")))
            {
                JArray controllerMethods = new JArray();

                RouteAttribute route = (RouteAttribute)type.GetCustomAttributes(typeof(RouteAttribute)).FirstOrDefault();
                string controllerName = type.Name.Replace("Controller", string.Empty);
                string mainRoute = route.Template.Replace("[controller]", controllerName.ToLower());

                foreach (MethodInfo method in type.GetMethods())
                    foreach (HttpMethodAttribute attr in method.GetCustomAttributes(typeof(HttpMethodAttribute)))
                    {
                        // Путь
                        JObject apiMethod = new JObject {
                            { "route", $"{string.Join(", ", attr.HttpMethods)} {string.Join("/", new string[] { mainRoute, attr.Template }.Where(s => !string.IsNullOrEmpty(s)))}" }
                        };

                        // Описание
                        DescriptionAttribute descAttr = (DescriptionAttribute)method.GetCustomAttributes(typeof(DescriptionAttribute)).FirstOrDefault();
                        if (descAttr != null)
                            apiMethod.Add("desc", descAttr.Description);

                        // Авторизация
                        AuthorizeAttribute authAttr = (AuthorizeAttribute)method.GetCustomAttributes(typeof(AuthorizeAttribute)).FirstOrDefault();
                        if (authAttr != null)
                            apiMethod.Add("auth", new JObject{
                                { "roles", authAttr.Roles },
                                { "policy", authAttr.Policy }
                            });

                        // Параметры
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length > 0)
                        {
                            JArray methodParams = new JArray();
                            foreach (ParameterInfo parameter in method.GetParameters())
                                methodParams.Add(new JObject {
                                    { "name", parameter.Name },
                                    { "type", parameter.ParameterType.Name },
                                    { "default", parameter.RawDefaultValue.ToString() }
                                });
                            apiMethod.Add("params", methodParams);
                        }

                        controllerMethods.Add(apiMethod);
                    }

                apiList.Add(controllerName, controllerMethods);
            }

            return apiList.ToString();
        }

        // GET api/ping
        [HttpGet("ping")]
        [Description("Пинг")]
        public object Ping()
        {
            return DateTime.UtcNow;
        }
    }
}
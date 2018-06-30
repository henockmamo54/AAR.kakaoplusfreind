using Microsoft.Practices.Unity;
using AAR.kakaoplusfreind.Services;
using System.Web;
using System.Web.Mvc;
using Unity.Mvc5;
using Unity;

namespace AAR.kakaoplusfreind
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            //container.RegisterType<ISessionService, DocumentDBSessionService>();
            //container.RegisterType<IDirectLineConversationService, DirectLineCoversationService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}

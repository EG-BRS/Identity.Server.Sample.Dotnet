using HybricMVC.NetFramework.Controllers;
using HybricMVC.NetFramework.Services;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;

namespace HybricMVC.NetFramework
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IXenaService, XenaService>();
            container.RegisterType<IController, XenaController>("Xena");

            return container;
        }
    }
}
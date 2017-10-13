using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace SignalRSer.App_Start
{
    // http://www.c-sharpcorner.com/article/using-autofac-with-web-api/
    public class AutofacConfig
    {
        private static IContainer _container;

        public static void Initialize(HttpConfiguration config)
        {
            Initialize(config, RegisterServices(new ContainerBuilder()));
        }


        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        public static void ConfigureSignalR(IAppBuilder app)
        {
            var signalRConfig = new HubConfiguration
            {
                Resolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(_container),
                EnableDetailedErrors = true,
                EnableJavaScriptProxies = false
            };
            GlobalHost.DependencyResolver = signalRConfig.Resolver;
            /*
            var hubPipeline = signalRConfig.Resolver.Resolve<Microsoft.AspNet.SignalR.Hubs.IHubPipeline>();
            hubPipeline.AddModule(new MyErrorModule()
            {
                Logger = SerilogLogger.Default
            });
            */
            app.MapSignalR(signalRConfig);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            //Register your Web API controllers.  
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            builder.RegisterFilterProvider();

            /*
            builder.RegisterType<DBCustomerEntities>()
                   .As<DbContext>()
                   .InstancePerRequest();

            builder.RegisterType<DbFactory>()
                   .As<IDbFactory>()
                   .InstancePerRequest();

            builder.RegisterGeneric(typeof(GenericRepository<>))
                   .As(typeof(IGenericRepository<>))
                   .InstancePerRequest();
                   */
            //Set the dependency resolver to be Autofac.  
            _container = builder.Build();

            return _container;
        }
    }
}
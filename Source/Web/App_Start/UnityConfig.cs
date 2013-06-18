// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnityConfig.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(App.Web.UnityConfig.Activator), "Start")]

namespace App.Web
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Mvc;

    using App.Data;
    using App.Security;
    using App.Services;

    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.WebApi;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    /// <summary>Specifies the Unity configuration for the main container.</summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IMembershipService, MembershipService>();
            container.RegisterType<IFormsAuthentication, FormsAuthenticationWrapper>(new ContainerControlledLifetimeManager());
        }

        /// <summary>Provides the bootstrapping for integrating Unity with Web Api when it is hosted in ASP.NET.</summary>
        public static class Activator
        {
            /// <summary>Integrates Unity when the application starts.</summary>
            public static void Start()
            {
                var container = UnityConfig.GetConfiguredContainer();

                // Use UnityHierarchicalDependencyResolver if you want to use a new child container for each IHttpController resolution.
                // var resolver = new WebApi.UnityHierarchicalDependencyResolver(container);
                var resolver = new UnityDependencyResolver(container);
                GlobalConfiguration.Configuration.DependencyResolver = resolver;
            }
        }
    }
}

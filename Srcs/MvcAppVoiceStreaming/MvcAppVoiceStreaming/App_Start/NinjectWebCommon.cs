[assembly: WebActivator.PreApplicationStartMethod(typeof(MvcAppVoiceStreaming.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(MvcAppVoiceStreaming.App_Start.NinjectWebCommon), "Stop")]

namespace MvcAppVoiceStreaming.App_Start
{
	using Microsoft.Web.Infrastructure.DynamicModuleHelper;
	using Ninject;
	using Ninject.Web.Common;
	using System;
	using System.Web;
	using VoiceStreaming.Common;
	using VoiceStreaming.Common.Infrastructure;

	public static class NinjectWebCommon
	{
		private const string _ApiControllerScope = "ApiControllerScope";
		private static readonly Bootstrapper bootstrapper = new Bootstrapper();

		/// <summary>
		/// Starts the application
		/// </summary>
		public static void Start()
		{
			System.Diagnostics.Debug.WriteLine("NinjectWebCommon.Start was invoked.");
			DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
			DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
			bootstrapper.Initialize(CreateKernel);
		}

		/// <summary>
		/// Stops the application.
		/// </summary>
		public static void Stop()
		{
			bootstrapper.ShutDown();
		}

		/// <summary>
		/// Creates the kernel that will manage your application.
		/// </summary>
		/// <returns>The created kernel.</returns>
		private static IKernel CreateKernel()
		{
			var kernel = new StandardKernel();
			kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
			kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

			RegisterServices(kernel);
			System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new Infrastructure.NinjectDependencyResolver(kernel);

			return kernel;
		}

		/// <summary>
		/// Load your modules or register your services here!
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		private static void RegisterServices(IKernel kernel)
		{
			System.Diagnostics.Debug.WriteLine("Invoked RegisterServices");
			kernel.Bind<IContentManager>().To<ContentManager>().InSingletonScope();
			kernel.Bind<IContentMapper>().To<Infrastructure.ContentMapper>().InSingletonScope();
			kernel.Bind<ILog>().To<Infrastructure.Logger>().InSingletonScope();

			//kernel.Bind(x => x.FromThisAssembly() .SelectAllClasses().InheritedFrom<System.Web.Http.ApiController>()
			//	  .BindToSelf()
			//	  .Configure(b => b.DefinesNamedScope(_ApiControllerScope)));
			//kernel.Bind<ILog>().To<Infrastructure.Logger>().InNamedScope(_ApiControllerScope);

			//kernel.Bind<IContentManager>().ToConstant<ContentManager>(new ContentManager());
			//kernel.Bind<IContentMapper>().ToConstant<Infrastructure.ContentMapper>(new Infrastructure.ContentMapper());
		}
	}
}
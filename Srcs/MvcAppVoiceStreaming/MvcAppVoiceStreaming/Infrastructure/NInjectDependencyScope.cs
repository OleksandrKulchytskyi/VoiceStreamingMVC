using Ninject;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Http.Dependencies;

namespace MvcAppVoiceStreaming.Infrastructure
{
	// Provides a Ninject implementation of IDependencyScope
	// which resolves services using the Ninject container.
	public class NinjectDependencyScope : IDependencyScope
	{
		protected IResolutionRoot resolutionRoot;

		public NinjectDependencyScope(IResolutionRoot resolver)
		{
			Contract.Assert(resolver != null);
			this.resolutionRoot = resolver;
		}

		public object GetService(Type serviceType)
		{
			IRequest request = resolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
			return resolutionRoot.Resolve(request).SingleOrDefault();
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			IRequest request = resolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
			return resolutionRoot.Resolve(request).ToList();
		}

		#region Old implementation
		//public object GetService(Type serviceType)
		//{
		//	if (resolutionRoot == null)
		//		throw new ObjectDisposedException("this", "This scope has been disposed");

		//	return resolutionRoot.TryGet(serviceType);
		//}

		//public System.Collections.Generic.IEnumerable<object> GetServices(Type serviceType)
		//{
		//	if (resolutionRoot == null)
		//		throw new ObjectDisposedException("this", "This scope has been disposed");

		//	return resolutionRoot.GetAll(serviceType);
		//} 
		#endregion

		public void Dispose()
		{
			//IDisposable disposable = resolutionRoot as IDisposable;
			//if (disposable != null)
			//	disposable.Dispose();

			//resolutionRoot = null;
		}
	}

	// This class is the resolver, but it is also the global scope
	// so we derive from NinjectScope.
	public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver
	{
		private IKernel kernel;

		public NinjectDependencyResolver(IKernel kernel)
			: base(kernel)
		{
			Contract.Assert(kernel != null);
			this.kernel = kernel;
		}

		public IDependencyScope BeginScope()
		{
			//return new NinjectDependencyScope(kernel.BeginBlock());
			return this;
		}
	}
}
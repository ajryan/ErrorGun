using System;
using System.Configuration;
using ErrorGun.Web.Services;
using Ninject;
using Ninject.Modules;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace ErrorGun.Web.Injection
{
    public class ErrorGunWebServicesModule : NinjectModule
    {
        public static readonly IKernel GlobalKernel = new StandardKernel(new ErrorGunWebServicesModule());
        private static readonly IDocumentStore _DocumentStore;

        static ErrorGunWebServicesModule()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["RavenDB"].ConnectionString;
            
            _DocumentStore = connectionString.Contains("DataDir")
                ? new EmbeddableDocumentStore { ConnectionStringName = "RavenDB" }
                : new DocumentStore { ConnectionStringName = "RavenDB" };

            _DocumentStore.Initialize();
        }

        public override void Load()
        {
            Bind<IDocumentStore>().ToMethod(_ => _DocumentStore).InSingletonScope();
            Bind<IAppService>().To<AppService>();
            Bind<IErrorService>().To<ErrorService>();
            Bind<IEmailService>().To<EmailService>();
        }
    }
}
using System;
using System.Configuration;
using ErrorGun.Web.Raven;
using ErrorGun.Web.Services;
using Ninject.Modules;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace ErrorGun.Web.Injection
{
    public class ErrorGunWebServicesModule : NinjectModule
    {
        private readonly IDocumentStore _documentStore;

        public ErrorGunWebServicesModule()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["RavenDB"].ConnectionString;
            
            _documentStore = connectionString.Contains("DataDir")
                ? new EmbeddableDocumentStore { ConnectionStringName = "RavenDB", UseEmbeddedHttpServer = true }
                : new DocumentStore { ConnectionStringName = "RavenDB" };

            _documentStore.InitializeWithDefaults();
        }

        public override void Load()
        {
            Bind<IDocumentStore>().ToMethod(_ => _documentStore).InSingletonScope();
            Bind<IAppService>().To<AppService>();
            Bind<IErrorService>().To<ErrorService>();
            Bind<IEmailService>().To<EmailService>();
        }
    }
}
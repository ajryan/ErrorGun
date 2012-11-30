using System;
using ErrorGun.Web.Raven;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Listeners;

namespace ErrorGun.Tests
{
    public abstract class RavenDbTestBase : IDisposable
    {
        private readonly object _storeLock = new object();
        private volatile IDocumentStore _documentStore;
        
        public void Dispose()
        {
            if (DocumentStore.WasDisposed)
                return;

            DocumentStore.AssertDocumentStoreErrors();
            DocumentStore.Dispose();
        }

        protected IDocumentStore DocumentStore
        {
            get
            {
                if (_documentStore == null)
                    lock (_storeLock)
                        if (_documentStore == null)
                        {
                            var documentStore = new EmbeddableDocumentStore {RunInMemory = true};
                            documentStore.InitializeWithDefaults();
                            documentStore.RegisterListener(new NoStaleQueriesListener());

                            _documentStore = documentStore;
                        }

                return _documentStore;
            }
        }

        private class NoStaleQueriesListener : IDocumentQueryListener
        {
            public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
            {
                queryCustomization.WaitForNonStaleResults();
            }
        }
    }
}

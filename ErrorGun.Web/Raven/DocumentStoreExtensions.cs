using System;
using System.Diagnostics;
using System.Text;
using ErrorGun.Web.Extensions;
using Raven.Client;
using Raven.Client.Indexes;

namespace ErrorGun.Web.Raven
{
    public static class DocumentStoreExtensions
    {
        public static void InitializeWithDefaults(this IDocumentStore documentStore)
        {
            documentStore.Initialize();
            IndexCreation.CreateIndexes(typeof(ContactEmailsByConfirmationCode).Assembly, documentStore);
            documentStore.AssertDocumentStoreErrors();
        }

        public static void AssertDocumentStoreErrors(this IDocumentStore documentStore)
        {
            var serverErrors = documentStore.DatabaseCommands.GetStatistics().Errors;
            if (serverErrors == null || serverErrors.Length <= 0)
                return;

            var errorMessages = new StringBuilder();
            foreach (var serverError in serverErrors)
            {
                string errorMessage = String.Format(
                    "Document: {0}; Index: {1}; Error: {2}",
                    serverError.Document.PlaceholderOrTrim(),
                    serverError.Index.PlaceholderOrTrim(),
                    serverError.Error.PlaceholderOrTrim());

                Debug.WriteLine(errorMessage);
                errorMessages.AppendLine(errorMessage);
            }

            throw new InvalidOperationException("DocumentStore has some errors: " + errorMessages.ToString());
        }
    }
}
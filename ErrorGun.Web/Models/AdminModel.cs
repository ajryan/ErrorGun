using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ErrorGun.Common;
using Raven.Client;

namespace ErrorGun.Web.Models
{
    public class AdminModel
    {
        private const int APP_PAGE_SIZE = 2;

        private static readonly string _AdminPassword = ConfigurationManager.AppSettings["AdminPassword"];

        public string Password { get; set; }
        public int AppPage { get; set; }

        public string Message { get; set; }
        public string ServerBitness { get; set; }
        public string ProcessBitness { get; set; }
        public List<App> Apps { get; set; }

        public AdminModel(string password, int appPage, IDocumentStore documentStore)
        {
            Password = password;
            AppPage = Math.Max(1, appPage);
            Apps = new List<App>();

            bool passwordCorrect = (password == _AdminPassword);

            Message = passwordCorrect ? "Password correct" : "Password incorrect";
            if (!passwordCorrect)
                return;

            ServerBitness = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            ProcessBitness = Environment.Is64BitProcess ? "x64" : "x86";

            using (var session = documentStore.OpenSession())
            {
                Apps = session
                    .Query<App>()
                    .Skip((AppPage - 1) * APP_PAGE_SIZE)
                    .Take(APP_PAGE_SIZE)
                    .ToList();
            }
        }
    }
}
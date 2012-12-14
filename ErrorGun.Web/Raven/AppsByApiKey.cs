using System;
using System.Linq;
using ErrorGun.Common;
using Raven.Client.Indexes;

namespace ErrorGun.Web.Raven
{
    public class AppsByApiKey : AbstractIndexCreationTask<App>
    {
        public AppsByApiKey()
        {
            Map = apps => 
                from app in apps
                select new { ApiKey = app.ApiKey };
        }
    }
}
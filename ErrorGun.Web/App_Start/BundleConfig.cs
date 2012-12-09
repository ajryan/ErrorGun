using System;
using System.Web.Optimization;

namespace ErrorGun.Web.App_Start
{
    public static class BundleConfig
    {
        public const string JQuery = "~/bundles/jquery";
        public const string Knockout = "~/bundles/knockout";

        public const string Home_Register = "~/bundles/home_register";

        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle(JQuery).Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle(Knockout).Include("~/Scripts/knockout-{version}.js"));

            bundles.Add(new ScriptBundle(Home_Register).Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/KoBindings.js",
                "~/Scripts/ErrorCodes.js",
                "~/Scripts/AppCreate.js"));
        }
    }
}
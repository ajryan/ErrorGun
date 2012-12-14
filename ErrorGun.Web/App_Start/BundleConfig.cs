using System;
using System.Web.Optimization;

namespace ErrorGun.Web.App_Start
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle(AppScript.JQuery).Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle(AppScript.Knockout).Include("~/Scripts/knockout-{version}.js"));

            bundles.Add(new ScriptBundle(AppScript.HomeRegister).Include(
                "~/Scripts/json2.js",
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/ErrorCodes.js",
                "~/Scripts/AppCreate.js"));

            bundles.Add(new ScriptBundle(AppScript.HomeView).Include(
                "~/Scripts/json2.js",
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/ErrorCodes.js",
                "~/Scripts/App.js",
                "~/Scripts/AppView.js"));

            bundles.Add(new StyleBundle(AppStyle.Skeleton).Include(
                "~/Content/Styles/Skeleton/base.css",
                "~/Content/Styles/Skeleton/skeleton.css",
                "~/Content/Styles/icons.css",
                "~/Content/Styles/layout.css"));
        }
    }

    public static class AppScript
    {
        public const string JQuery = "~/bundles/jquery";
        public const string Knockout = "~/bundles/knockout";
        public const string HomeRegister = "~/bundles/home_register";
        public const string HomeView = "~/bundles/home_view";
    }

    public static class AppStyle
    {
        public const string Skeleton = "~/bundles/skeleton";
    }
}
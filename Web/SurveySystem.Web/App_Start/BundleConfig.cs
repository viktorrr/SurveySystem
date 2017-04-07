namespace SurveySystem.Web
{
    using System.Web.Optimization;

    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterScripts(bundles);
            RegisterStyles(bundles);
        }

        private static void RegisterScripts(BundleCollection bundles)
        {
            // TODO: remove all unnecessary node_modules files

            bundles
                .Add(new ScriptBundle("~/bundles/jquery")
                .Include(FormatNodeModulePath("jquery/dist/jquery.min.js")));

            bundles
                .Add(new ScriptBundle("~/bundles/jqueryval")
                .Include(
                    FormatNodeModulePath("jquery-validation/dist/jquery.validate.min.js"),
                    FormatNodeModulePath("jquery-validation-unobtrusive/jquery.validate.unobtrusive.js")));

            bundles
                .Add(new ScriptBundle("~/bundles/bootstrap")
                .Include(FormatNodeModulePath("bootstrap/dist/js/bootstrap.min.js")));
        }

        private static void RegisterStyles(BundleCollection bundles)
        {
            bundles
                .Add(new StyleBundle("~/Content/css")
                .Include(
                    FormatNodeModulePath("bootstrap/dist/css/bootstrap.min.css"),
                    FormatNodeModulePath("bootstrap/dist/css/bootstrap-theme.min.css"),
                    "~/Content/site.css"));
        }

        private static string FormatNodeModulePath(string path)
        {
            return $"~/node_modules/{path}";
        }
    }
}

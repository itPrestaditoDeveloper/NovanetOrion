using System.Web;
using System.Web.Optimization;

namespace OrionCoreCableColor
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/styles/externalstylesheets").Include(
               "~/Content/css/vendors.bundle.css",
               "~/Content/css/app.bundle.css",
               "~/Content/css/skins/skin-master.css",
               "~/Content/css/datagrid/datatables/datatables.bundle.css",
               "~/Template/plugins/dropzone/dropzone.min.css"
               ));

            bundles.Add(
                new StyleBundle("~/styles/applicationstylesheetslogin")
                .Include(
                "~/Content/css/vendors.bundle.css",
                "~/Content/css/app.bundle.css",
                "~/Content/css/skins/skin-master.css",
                "~/Content/fa-brands.css",
                  "~/Content/js/unitegallery/css/unitegallery.min.css",
                "~/Content/js/unitegallery/css/unitegallery.css"
              
                ));


            bundles.Add(new ScriptBundle("~/scripts/external-plugins").Include(
                "~/Content/js/vendors.bundle.js",
                "~/Content/js/app.bundle.js",
                 "~/Content/js/unitegallery/js/unitegallery.min.js",
                "~/Content/js/unitegallery/js/unitegallery.js",
               
                "~/Template/js/nifty.min.js",
                
                "~/Content/js/statistics/flot/flot.bundle.js",
               "~/Content/js/statistics/ApexchartEchoaMano.js",
               "~/Content/js/datagrid/datatables/datatables.bundle.js",
               "~/Content/js/datagrid/datatables/datatables.export.js"
                ));



            bundles.Add(new ScriptBundle("~/bundles/personalized").Include(
           "~/Template/js/Personalized/accounting.min.js",
           "~/Template/js/Personalized/Alertas.js",
           "~/Template/js/Personalized/Conversion.js",
           "~/Template/js/Personalized/InputFormat.js",
           "~/Template/js/Personalized/Loading.js",
            "~/Template/js/Personalized/Layout.js",
            "~/Scripts/mascarasDeEntrada/js/jquery.inputmask.bundle.js",
            "~/Template/plugins/dropzone/dropzone.min.js",
           "~/Template/js/Personalized/Modals.js"

           ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            "~/Scripts/jquery.validate*"
            ));


            BundleTable.EnableOptimizations = true;
        }
    }
}

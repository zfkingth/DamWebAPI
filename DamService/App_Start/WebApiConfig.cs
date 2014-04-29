using DamService.Models;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Microsoft.OData.Edm;
using System.Web.OData.Batch;
using Hammergo.Data;

namespace DamService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            //config.Routes.MapHttpRoute(
            //    name: "Transactions",
            //    routeTemplate: "Transactions/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional });
            config.Routes.MapODataServiceRoute("odata", "odata", GetModel(), new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));


        }



        private static IEdmModel GetModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.ContainerName = "DamContext";

            builder.EntitySet<App>("Apps");
            builder.EntitySet<ApparatusType>("ApparatusTypes");
            builder.EntitySet<AppCollection>("AppCollections");
            builder.EntitySet<CalculateParam>("CalculateParams");
            builder.EntitySet<CalculateValue>("CalculateValues");
            builder.EntitySet<ConstantParam>("ConstantParams");
            builder.EntitySet<Formula>("Formulae");
            builder.EntitySet<MessureParam>("MessureParams");
            builder.EntitySet<MessureValue>("MessureValues");
            builder.EntitySet<ProjectPart>("ProjectParts");
            builder.EntitySet<Remark>("Remarks");
            builder.EntitySet<TaskApp>("TaskApps");
            builder.EntitySet<TaskType>("TaskTypes");



            builder.EntitySet<Transaction>("Transactions");

            var transType = builder.EntityType<Transaction>();

            transType.Collection
               .Function("First")
               .Returns<Transaction>();

   ;


            return builder.GetEdmModel();
        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using Hammergo.Data;
using System.Web.Http.OData.Routing.Conventions;
using DamServiceV3.Helper;
using System.Web.Http.OData.Batch;

namespace DamServiceV3
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {


            IList<IODataRoutingConvention> routingConventions = ODataRoutingConventions.CreateDefault();
            routingConventions.Insert(0, new CountODataRoutingConvention());


            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<ApparatusType>("ApparatusTypes");
            builder.EntitySet<App>("Apps");

            builder.EntitySet<AppParam>("AppParams");
            builder.EntitySet<ProjectPart>("ProjectParts");
            builder.EntitySet<Remark>("Remarks");
            builder.EntitySet<TaskApp>("TaskApps");
            builder.EntitySet<MessureValue>("MessureValues");
            builder.EntitySet<CalculateValue>("CalculateValues");
            builder.EntitySet<Formula>("Formulae");

            ActionConfiguration rateProduct = builder.Entity<App>().Action("RateProduct");
            rateProduct.Parameter<int>("Rating");
            rateProduct.Returns<int>();


            ActionConfiguration actionRateAll = builder.Entity<App>().Collection.Action("RateAllProducts");
            actionRateAll.Parameter<int>("Rating");
            actionRateAll.Returns<int>();


            ActionConfiguration actionCheckExistData = builder.Entity<App>().Action("CheckExistData");
            actionCheckExistData.Parameter<DateTimeOffset>("date");
            actionCheckExistData.Returns<bool>();

            ActionConfiguration actionGetCalcValues = builder.Entity<App>().Collection.Action("GetCalcValues");
            actionGetCalcValues.Parameter<int>("topNum");
            actionGetCalcValues.Parameter<DateTimeOffset?>("startDate");
            actionGetCalcValues.Parameter<DateTimeOffset?>("endDate");
            actionGetCalcValues.CollectionParameter<Guid>("appids");
            actionGetCalcValues.ReturnsCollectionFromEntitySet<CalculateValue>("CalculateValues");


            ActionConfiguration actionGetMesValues = builder.Entity<App>().Collection.Action("GetMesValues");
            actionGetMesValues.Parameter<int>("topNum");
            actionGetMesValues.Parameter<DateTimeOffset?>("startDate");
            actionGetMesValues.Parameter<DateTimeOffset?>("endDate");
            actionGetMesValues.CollectionParameter<Guid>("appids");
            actionGetMesValues.ReturnsCollectionFromEntitySet<MessureValue>("MessureValues");


            ActionConfiguration actionGetRemarks = builder.Entity<App>().Collection.Action("GetRemarks");
            actionGetRemarks.Parameter<int>("topNum");
            actionGetRemarks.Parameter<DateTimeOffset?>("startDate");
            actionGetRemarks.Parameter<DateTimeOffset?>("endDate");
            actionGetRemarks.CollectionParameter<Guid>("appids");
            actionGetRemarks.ReturnsCollectionFromEntitySet<Remark>("Remarks");





            ActionConfiguration actionSearcyAppByName = builder.Entity<App>().Collection.Action("SearcyAppByName");
            actionSearcyAppByName.Parameter<string>("match");
            actionSearcyAppByName.ReturnsCollectionFromEntitySet<App>("Apps");

            ActionConfiguration actionSearcyAppCalcName = builder.Entity<App>().Collection.Action("SearcyAppCalcName");
            actionSearcyAppCalcName.Parameter<string>("match");
            actionSearcyAppCalcName.ReturnsCollectionFromEntitySet<App>("Apps");

            ActionConfiguration actionGetChildAppCalcName = builder.Entity<App>().Collection.Action("GetChildAppCalcName");
            actionGetChildAppCalcName.Parameter<string>("appCalcName");
            actionGetChildAppCalcName.Parameter<DateTimeOffset>("date");
            actionGetChildAppCalcName.ReturnsCollection<string>();


            ActionConfiguration actionGetAllFormulaeByAppID = builder.Entity<App>().Action("GetAllFormulaeByAppID");
            actionGetAllFormulaeByAppID.ReturnsCollectionFromEntitySet<Formula>("Formulae");


            ActionConfiguration actionUpdateAppsProject = builder.Entity<ProjectPart>().Action("UpdateAppsProject");
            actionUpdateAppsProject.CollectionParameter<Guid>("appids");
            actionUpdateAppsProject.Returns<bool>();


            ActionConfiguration actionUpdateAppsProjectByNames = builder.Entity<ProjectPart>().Action("UpdateAppsProjectByNames");
            actionUpdateAppsProjectByNames.CollectionParameter<string>("names");
            actionUpdateAppsProjectByNames.Returns<bool>();

            config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel(), new CountODataPathHandler(), routingConventions,
               new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));




            config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional });

        }
    }
}

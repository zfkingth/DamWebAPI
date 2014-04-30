﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using Hammergo.Data;
using System.Web.Http.OData.Routing.Conventions;
using DamServiceV3.Helper;

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




            config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel(), new CountODataPathHandler(), routingConventions);
        }
    }
}

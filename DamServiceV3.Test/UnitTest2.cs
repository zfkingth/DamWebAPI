using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DamServiceV3.Test.DamServiceRef;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DamServiceV3.Test.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Services.Client;

namespace DamServiceV3.Test
{
    [TestClass]
    public class UnitTest2
    {



        [TestMethod]
        public void A_Params()
        {
            Uri uri = new Uri(TestConfig.serviceUrl);
            var context = new DamServiceRef.Container(uri);

            context.Format.UseJson();

            var firstApp = context.Apps.FirstOrDefault();


            var paramList = context.AppParams.Where(s => s.AppId == firstApp.Id).ToList();
         
        }


    }
}

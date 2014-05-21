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

namespace DamServiceV3.Test
{
    [TestClass]
    public class UnitTest1
    {



        [TestMethod]
        public void T_type()
        {
            Uri uri = new Uri(TestConfig.serviceUrl);
            var context = new DamServiceRef.Container(uri);

            context.Format.UseJson();

            var typeList = context.ApparatusTypes.ToList();


            int cnt1 = context.ApparatusTypes.Count();

            var newType = new ApparatusType() { Id = Guid.NewGuid(), TypeName = (new Random((int)DateTime.Now.Ticks)).Next().ToString() };

            context.AddToApparatusTypes(newType);
            context.SaveChanges();

            int cnt2 = context.ApparatusTypes.Count();

            int cnt3 = context.ApparatusTypes.Where(s => s.TypeName == "第一种类型").Count();

            Assert.IsTrue(cnt1 != cnt2, "插入 失败");

            //在单独查询

            context.Detach(newType);

            var type1 = context.ApparatusTypes.Where(s => s.Id == newType.Id).SingleOrDefault();

            Assert.AreEqual(type1.TypeName, newType.TypeName, "插入失败");

            //更新

            type1.TypeName = newType.TypeName + "modify";

            context.UpdateObject(type1);
            context.SaveChanges();

            var type2 = context.ApparatusTypes.Where(s => s.Id == newType.Id).SingleOrDefault();

            Assert.AreEqual(type2.TypeName, type1.TypeName, "更新失败");

            //删除
            context.DeleteObject(type2);

            context.SaveChanges();
            int fCnt = context.Apps.Count();

            Assert.IsTrue(cnt1 == fCnt, "删除 失败");


        }


        [TestMethod]
        public void T_app()
        {
            Uri uri = new Uri(TestConfig.serviceUrl);
            var context = new DamServiceRef.Container(uri);

            context.Format.UseJson();

            var itemList = context.Apps.ToList();


            int cnt1 = context.Apps.Count();

            var newItem = new App();
            newItem.Id = Guid.NewGuid();
            newItem.AppName = "第三支仪器";
            newItem.CalculateName = "ThirdApp";

            context.AddToApps(newItem);
            context.SaveChanges();

            int cnt2 = context.Apps.Count();


            Assert.IsTrue(cnt1 + 1 == cnt2, "插入 失败");

            //在单独查询

            context.Detach(newItem);

            var itemInDb = context.Apps.Where(s => s.Id == newItem.Id).SingleOrDefault();

            Assert.AreEqual(itemInDb.Id, newItem.Id, "插入失败");

            //更新

            itemInDb.AppName = newItem.AppName + "modify";

            context.UpdateObject(itemInDb);
            context.SaveChanges();

            context.Detach(itemInDb);

            var itemUpdated = context.Apps.Where(s => s.Id == itemInDb.Id).SingleOrDefault();

            Assert.AreEqual(itemUpdated.AppName, itemInDb.AppName, "更新失败");

            //删除
            context.DeleteObject(itemUpdated);

            context.SaveChanges();
            int fCnt = context.ApparatusTypes.Count();

            Assert.IsTrue(cnt1 == fCnt, "删除 失败");


        }


        [TestMethod]
        public void T_appNav()
        {
            Uri uri = new Uri(TestConfig.serviceUrl);
            var context = new DamServiceRef.Container(uri);

            context.Format.UseJson();

            var app = context.Apps.Where(s => s.AppName == "第一支仪器").SingleOrDefault();

            context.LoadProperty<AppParam>(app, "AppParams", null);

            int cnt = app.AppParams.Count;

            Assert.AreEqual(3, cnt, "参数数目不一致");





        }


        [TestMethod]
        public void T_projectPart()
        {
            Uri uri = new Uri(TestConfig.serviceUrl);
            var context = new DamServiceRef.Container(uri);

            context.Format.UseJson();

            var itemList = context.ProjectParts.ToList();


            int cnt1 = context.ProjectParts.Count();

            var root = context.ProjectParts.Where(s => s.ParentPart == null).SingleOrDefault();


            var newItem = new ProjectPart();
            newItem.Id = Guid.NewGuid();
            newItem.PartName = "测试部位";
            newItem.ParentPart = root.Id;

            context.AddToProjectParts(newItem);
            context.SaveChanges();

            int cnt2 = context.ProjectParts.Count();


            Assert.IsTrue(cnt1 + 1 == cnt2, "插入 失败");

            //在单独查询

            context.Detach(newItem);

            var itemInDb = context.ProjectParts.Where(s => s.Id == newItem.Id).SingleOrDefault();

            Assert.AreEqual(itemInDb.Id, newItem.Id, "插入失败");

            //更新

            itemInDb.PartName = newItem.PartName + "modify";

            context.UpdateObject(itemInDb);
            context.SaveChanges();

            context.Detach(itemInDb);

            var itemUpdated = context.ProjectParts.Where(s => s.Id == itemInDb.Id).SingleOrDefault();

            Assert.AreEqual(itemUpdated.PartName, itemInDb.PartName, "更新失败");

            //删除
            context.DeleteObject(itemUpdated);

            context.SaveChanges();
            int fCnt = context.ProjectParts.Count();

            Assert.IsTrue(cnt1 == fCnt, "删除 失败");


        }



        [TestMethod]
        public void T_Remark()
        {
            Uri uri = new Uri(TestConfig.serviceUrl);
            var context = new DamServiceRef.Container(uri);

            context.Format.UseJson();

            var itemList = context.Remarks.ToList();


            int cnt1 = context.Remarks.Count();

            var app = context.Apps.Take(1).SingleOrDefault();//top 1


            var newItem = new Remark();
            newItem.Id = Guid.NewGuid();
            newItem.RemarkText = "测试";
            newItem.Date = DateTime.Now;
            newItem.AppId = app.Id;

            context.AddToRemarks(newItem);
            context.SaveChanges();

            int cnt2 = context.Remarks.Count();


            Assert.IsTrue(cnt1 + 1 == cnt2, "插入 失败");

            //在单独查询

            context.Detach(newItem);

            var itemInDb = context.Remarks.Where(s => s.Id == newItem.Id).SingleOrDefault();

            Assert.AreEqual(itemInDb.Id, newItem.Id, "插入失败");

            //更新

            itemInDb.RemarkText = newItem.RemarkText + "modify";

            context.UpdateObject(itemInDb);
            context.SaveChanges();

            context.Detach(itemInDb);

            var itemUpdated = context.Remarks.Where(s => s.Id == itemInDb.Id).SingleOrDefault();

            Assert.AreEqual(itemUpdated.RemarkText, itemInDb.RemarkText, "更新失败");

            //删除
            context.DeleteObject(itemUpdated);

            context.SaveChanges();
            int fCnt = context.Remarks.Count();

            Assert.IsTrue(cnt1 == fCnt, "删除 失败");


        }


        [TestMethod]
        public async Task T_paramsLogic1()
        {
            using (var client = new HttpClient())
            {
                // New code:
                client.BaseAddress = new Uri(TestConfig.baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/ParamsDTOs/1");
                if (response.IsSuccessStatusCode)
                {
                    var val = await response.Content.ReadAsAsync<int>();

                }

            }

        }




        [TestMethod]
        public async Task T_paramsConst()
        {
            using (var client = new HttpClient())
            {
                //get app

                Uri uri = new Uri(TestConfig.serviceUrl);
                var context = new DamServiceRef.Container(uri);

                context.Format.UseJson();

                var appItem = context.Apps.Where(s => s.AppName == "第一支仪器").SingleOrDefault();




                // New code:
                client.BaseAddress = new Uri(TestConfig.baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                ParamsDTO dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };

                var conParam1 = new ConstantParam()
                {
                    Id = Guid.NewGuid(),
                    AppId = appItem.Id,
                    ParamName = "c2",
                    ParamSymbol = "c2",
                    PrecisionNum = 2,
                    UnitSymbol = "no",
                    Val = 1,
                    Order = 1,
                    Description = "no description",


                };

                dto.AddedParams = new List<AppParam>() { conParam1 };

                HttpResponseMessage response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);

                Assert.IsTrue(response.IsSuccessStatusCode, "add param fail");



                //modify
                dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };

                conParam1.Order += 1;

                dto.UpdatedParams = new List<AppParam>() { conParam1 };

                response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);

                Assert.IsTrue(response.IsSuccessStatusCode, "delete param fail");


                //now deleted added param
                dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };


                dto.DeletedParams = new List<AppParam>() { conParam1 };

                response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);

                Assert.IsTrue(response.IsSuccessStatusCode, "delete param fail");
            }

        }


        [TestMethod]
        public async Task T_paramsMes()
        {
            using (var client = new HttpClient())
            {
                //get app

                Uri uri = new Uri(TestConfig.serviceUrl);
                var context = new DamServiceRef.Container(uri);

                context.Format.UseJson();

                var appItem = context.Apps.Where(s => s.AppName == "第一支仪器").SingleOrDefault();




                // New code:
                client.BaseAddress = new Uri(TestConfig.baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                ParamsDTO dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };

                var param = new MessureParam()
                {
                    Id = Guid.NewGuid(),
                    AppId = appItem.Id,
                    ParamName = "mtest2",
                    ParamSymbol = "mtest2",
                    PrecisionNum = 2,
                    UnitSymbol = "no",
                    Order = 1,
                    Description = "no description",


                };

                dto.AddedParams = new List<AppParam>() { param };

                HttpResponseMessage response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);

                Assert.IsTrue(response.IsSuccessStatusCode, "add param fail");



                //modify
                dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };

                param.Order += 1;

                dto.UpdatedParams = new List<AppParam>() { param };

                response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);

                Assert.IsTrue(response.IsSuccessStatusCode, "delete param fail");


                //now deleted added param
                dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };


                dto.DeletedParams = new List<AppParam>() { param };

                response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);

                Assert.IsTrue(response.IsSuccessStatusCode, "delete param fail");
            }

        }


        [TestMethod]
        public async Task T_paramsUpdate()
        {
            using (var client = new HttpClient())
            {
                //get app

                Uri uri = new Uri(TestConfig.serviceUrl);
                var context = new DamServiceRef.Container(uri);

                context.Format.UseJson();

                var appItem = context.Apps.Expand("AppParams").Where(s => s.AppName == "第一支仪器").SingleOrDefault();




                // New code:
                client.BaseAddress = new Uri(TestConfig.baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));




                ParamsDTO dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };

                var conParam1 = appItem.AppParams.OfType<ConstantParam>().SingleOrDefault();
                var mesParam1 = appItem.AppParams.OfType<MessureParam>().SingleOrDefault();
                var calParam1 = appItem.AppParams.OfType<CalculateParam>().SingleOrDefault();


                conParam1.Order += 1;
                mesParam1.Order += 1;
                calParam1.Order += 1;


                dto.UpdatedParams = new List<AppParam>() { conParam1, mesParam1, calParam1 };

                HttpResponseMessage response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);

                Assert.IsTrue(response.IsSuccessStatusCode, " update param fail");

                //all update success

                //test  acid

                mesParam1.ParamSymbol = "modify";

                Exception ex = null;

                response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);

                context.Detach(mesParam1);


                Assert.IsFalse(response.IsSuccessStatusCode, "test formulae fail");

                //reload mesparam



                var itemInDb = context.AppParams.Where(s => s.Id == mesParam1.Id).SingleOrDefault();

                Assert.AreNotEqual(mesParam1.ParamSymbol, itemInDb.ParamSymbol, "acid test fail");



            }

        }


        [TestMethod]
        public async Task T_params_Add_Delete()
        {
            using (var client = new HttpClient())
            {
                //get app

                Uri uri = new Uri(TestConfig.serviceUrl);
                var context = new DamServiceRef.Container(uri);

                context.Format.UseJson();

                var appItem = context.Apps.Where(s => s.AppName == "第二支仪器").SingleOrDefault();


                var conParam1 = new ConstantParam()
                {
                    Id = Guid.NewGuid(),
                    AppId = appItem.Id,
                    ParamName = "sc1",
                    ParamSymbol = "sc1",
                    PrecisionNum = 2,
                    UnitSymbol = "no",
                    Val = 1,
                    Order = 1,
                    Description = "no description",


                };

                var mesParam1 = new MessureParam()
                {
                    Id = Guid.NewGuid(),
                    AppId = appItem.Id,
                    ParamName = "sm1",
                    ParamSymbol = "sm1",
                    PrecisionNum = 2,
                    UnitSymbol = "no",
                    Order = 1,
                    Description = "no description",


                };


                var calParam1 = new CalculateParam()
                {
                    Id = Guid.NewGuid(),
                    AppId = appItem.Id,
                    ParamName = "scal1",
                    ParamSymbol = "scal1",
                    PrecisionNum = 2,
                    UnitSymbol = "no",
                    Order = 1,
                    Description = "no description",


                };


                var formula = new Formula()
                {
                    Id = Guid.NewGuid(),
                    ParamId = calParam1.Id,
                    StartDate = DateTimeOffset.MinValue,
                    EndDate = DateTimeOffset.MaxValue,
                    CalculateOrder = 1,
                    FormulaExpression = "sc1+sm1"
                };

                ParamsDTO dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };

                dto.AddedParams = new List<AppParam>() { conParam1, mesParam1, calParam1 };
                dto.AddedFormulae = new List<Formula>() { formula };


                // New code:
                client.BaseAddress = new Uri(TestConfig.baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);

                Assert.IsTrue(response.IsSuccessStatusCode, " insert param fail");

                //delete all params

                dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };

                dto.DeletedParams = new List<AppParam>() { conParam1, mesParam1, calParam1 };
                dto.DeletedFormulae = new List<Formula>() { formula };


                response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);
                Assert.IsTrue(response.IsSuccessStatusCode, "delete formulae fail");

                //reload mesparam

                context.Detach(conParam1);
                context.Detach(mesParam1);
                context.Detach(calParam1);


                var cnt = context.AppParams.Where(s => s.Id == mesParam1.Id).Count();

                Assert.AreEqual(0, cnt, "delete test fail");



            }

        }


        [TestMethod]
        public async Task T_params_Composite()
        {
            using (var client = new HttpClient())
            {
                //get app

                Uri uri = new Uri(TestConfig.serviceUrl);
                var context = new DamServiceRef.Container(uri);

                context.Format.UseJson();

                var appItem = context.Apps.Where(s => s.AppName == "第二支仪器").SingleOrDefault();


                var conParam1 = new ConstantParam()
                {
                    Id = Guid.NewGuid(),
                    AppId = appItem.Id,
                    ParamName = "sc1",
                    ParamSymbol = "sc1",
                    PrecisionNum = 2,
                    UnitSymbol = "no",
                    Val = 1,
                    Order = 1,
                    Description = "no description",


                };

                var mesParam1 = new MessureParam()
                {
                    Id = Guid.NewGuid(),
                    AppId = appItem.Id,
                    ParamName = "sm1",
                    ParamSymbol = "sm1",
                    PrecisionNum = 2,
                    UnitSymbol = "no",
                    Order = 1,
                    Description = "no description",


                };


                var calParam1 = new CalculateParam()
                {
                    Id = Guid.NewGuid(),
                    AppId = appItem.Id,
                    ParamName = "scal1",
                    ParamSymbol = "scal1",
                    PrecisionNum = 2,
                    UnitSymbol = "no",
                    Order = 1,
                    Description = "no description",


                };


                var formula1 = new Formula()
                {
                    Id = Guid.NewGuid(),
                    ParamId = calParam1.Id,
                    StartDate = DateTimeOffset.MinValue,
                    EndDate = DateTimeOffset.MaxValue,
                    CalculateOrder = 1,
                    FormulaExpression = "sc1+sm1"
                };

                ParamsDTO dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };

                dto.AddedParams = new List<AppParam>() { conParam1, mesParam1, calParam1 };
                dto.AddedFormulae = new List<Formula>() { formula1 };


                // New code:
                client.BaseAddress = new Uri(TestConfig.baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);

                Assert.IsTrue(response.IsSuccessStatusCode, " insert param fail");


                //add updated ,delete 
                var mesParam2 = new MessureParam()
                {
                    Id = Guid.NewGuid(),
                    AppId = appItem.Id,
                    ParamName = "sm2",
                    ParamSymbol = "sm2",
                    PrecisionNum = 2,
                    UnitSymbol = "no",
                    Order = 1,
                    Description = "no description",


                };

                var calParam2 = new CalculateParam()
                {
                    Id = Guid.NewGuid(),
                    AppId = appItem.Id,
                    ParamName = "scal2",
                    ParamSymbol = "scal2",
                    PrecisionNum = 2,
                    UnitSymbol = "no",
                    Order = 1,
                    Description = "no description",


                };


                var formula2 = new Formula()
                {
                    Id = Guid.NewGuid(),
                    ParamId = calParam2.Id,
                    StartDate = DateTimeOffset.MinValue,
                    EndDate = DateTimeOffset.MaxValue,
                    CalculateOrder = 1,
                    FormulaExpression = "sm2+sc1"
                };

                dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };

                dto.AddedParams = new List<AppParam>() { calParam2 };
                dto.AddedFormulae = new List<Formula>() { formula2 };

                //  fail because no corresponding mesparam 
                response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);
                Assert.IsFalse(response.IsSuccessStatusCode, "constraint fail");

                //fail because delete mes1 which cal param1 use
                dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };
                dto.AddedParams = new List<AppParam>() { mesParam2, calParam2 };
                dto.AddedFormulae = new List<Formula>() { formula2 };
                dto.DeletedParams = new List<AppParam>() { mesParam1 };
                response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);
                Assert.IsFalse(response.IsSuccessStatusCode, "constraint fail");

                //delete mes1 and calc1 
                dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };

                conParam1.Val = 2;

                dto.AddedParams = new List<AppParam>() { mesParam2, calParam2 };
                dto.AddedFormulae = new List<Formula>() { formula2 };
                dto.UpdatedParams = new List<AppParam>() { conParam1 };
                dto.DeletedParams = new List<AppParam>() { mesParam1, calParam1 };
                dto.DeletedFormulae = new List<Formula>() { formula1 };
                response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);
                Assert.IsTrue(response.IsSuccessStatusCode, "constraint fail");

                //delete all params

                dto = new ParamsDTO()
                {
                    Id = appItem.Id,
                };

                dto.DeletedParams = new List<AppParam>() { conParam1, mesParam2, calParam2 };
                dto.DeletedFormulae = new List<Formula>() { formula2 };


                response = await client.PostAsJsonAsync("api/ParamsDTOs", dto);
                Assert.IsTrue(response.IsSuccessStatusCode, "delete formulae fail");

                //reload mesparam

                context = new DamServiceRef.Container(uri);


                var cnt = context.AppParams.Where(s => s.Id == mesParam1.Id).Count();

                Assert.AreEqual(0, cnt, "delete test fail");



            }

        }


        [TestMethod]
        public async Task T_app_action()
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://localhost:53317");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                int old=3;
                var data = new { Rating = 3 };

                HttpResponseMessage response = await client.PostAsJsonAsync("odata/Apps(guid'3d76ff71-dab7-4752-b640-009155bc766e')/RateProduct", data);

                Assert.IsTrue(response.IsSuccessStatusCode, "action fail");

                var result = response.Content.ReadAsAsync<JObject>().Result;

                int ret = (int)result["value"];

                Assert.AreEqual(old * 2, ret, "action operate result error");
                
               
                //serialize to an object using Newtonsoft.Json nuget package





            }




        }




        [TestMethod]
        public    void T_app_GetAllFormulaeByAppID()
        {
            Uri uri = new Uri(TestConfig.serviceUrl);

            var context = new DamServiceRef.Container(uri);

            context.Format.UseJson();

            var appItem = context.Apps.Where(s => s.AppName == "第一支仪器").SingleOrDefault();

            var formula = context.GetAllFormulaeByAppID(appItem.Id) ;

            Assert.IsTrue(formula.Count() > 0, "查询失败");



        }



        [TestMethod]
        public void T_app_RateAllProducts()
        {
            Uri uri = new Uri(TestConfig.serviceUrl);

            var context = new DamServiceRef.Container(uri);

            context.Format.UseJson();


            var val = context.RateAllProducts(2);

            Assert.IsTrue( val==4, "测试失败");



        }


        [TestMethod]
        public void T_ProjectPart_UpdateAppsProject()
        {
            Uri uri = new Uri(TestConfig.serviceUrl);

            var context = new DamServiceRef.Container(uri);

            context.Format.UseJson();

            var appItem = context.Apps.Where(s => s.AppName == "第一支仪器").SingleOrDefault();

             //get root project part
            var partRoot = context.ProjectParts.Where(s => s.ParentPart == null).SingleOrDefault();

            var part1 = context.ProjectParts.Where(s => s.ParentPart ==  partRoot.Id).FirstOrDefault();

            bool ret=  context.UpdateAppsProject(part1.Id,new List<Guid>(){ appItem.Id });

            Assert.IsTrue(ret, "更新测点的工程部位失败");



        }
    }
}

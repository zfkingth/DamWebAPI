﻿using System;
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

namespace DamServiceV3.Test
{
    [TestClass]
    public class UnitTest1
    {



        [TestMethod]
        public void TestODataV3_type()
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
        public void TestODataV3_app()
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
        public void TestODataV3_appNav()
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
        public void TestODataV3_projectPart()
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
        public void TestODataV3_Remark()
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
        public async Task TestODataV3_paramsLogic1()
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
        public async Task TestODataV3_paramsLogic2()
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

                //dto.AddedParams = new List<AppParam>() { appItem.AppParams[0]};
                //dto.UpdatedParams = new List<AppParam>() { appItem.AppParams[1]};
                //dto.DeletedParams = new List<AppParam>() { appItem.AppParams[2]};

                

                HttpResponseMessage response = await client.PostAsJsonAsync("api/ParamsDTOs",dto);
                Assert.IsTrue(response.IsSuccessStatusCode, "测试失败");
        

            }

        }



    }
}

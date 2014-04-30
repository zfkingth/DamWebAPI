using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DamServiceV3.Test.DamServiceRef;

namespace DamServiceV3.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestODataV3_OP1()
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

            Assert.IsTrue(cnt1!=cnt2, "插入仪器类型失败");

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
            int fCnt = context.ApparatusTypes.Count();

            Assert.IsTrue(cnt1==fCnt, "删除仪器类型失败");


        }
    }
}

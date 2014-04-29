using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using DamService.Test.Hammergo.Data;
using DamContext = DamService.Test.Default.DamContext;
using DamService.Test.Hammergo.Data;
using System.Collections.Generic;
using Microsoft.OData.Client;
using System.Linq;


namespace DamService.Test
{

     

    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestBasicOperation1()
        {
            DamContext context = new DamContext(new Uri(TestConfig.serviceUrl));

            IList<ApparatusType> typeList = context.ApparatusTypes.ToList();

            int cnt = typeList.Count;

            var newType = new ApparatusType() { Id = Guid.NewGuid(), TypeName = (new Random((int)DateTime.Now.Ticks)).Next().ToString() };

            context.AddToApparatusTypes(newType);
            context.SaveChanges();

            int newCnt = context.ApparatusTypes.Count();

            Assert.AreEqual(cnt, newCnt, "插入仪器类型失败");

            //在单独查询

            context.Detach(newType);

            var type1 = context.ApparatusTypes.Single(s => s.Id == newType.Id);

            Assert.AreEqual(type1.TypeName, newType.TypeName, "插入失败");

            //更新

            type1.TypeName = newType.TypeName + "modify";

            context.UpdateObject(type1);
            context.SaveChanges();

            var type2 = context.ApparatusTypes.Single(s => s.Id == newType.Id);

            Assert.AreEqual(type2.TypeName, type1.TypeName, "更新失败");

            //删除
            context.DeleteObject(type2);

            context.SaveChanges();
            int fCnt = context.ApparatusTypes.Count();

            Assert.AreEqual(cnt, fCnt, "删除仪器类型失败");


        }
    }
}

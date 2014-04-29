using DamService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace DamService.Controllers
{
    public class TransactionsController : ODataController
    {

        private static List<Transaction> list = new List<Transaction>()
        {
            new Transaction(){ID=Guid.NewGuid(),CreateTime=DateTime.Now},
            new Transaction(){ID=Guid.NewGuid(),CreateTime=DateTime.Now},
            new Transaction(){ID=Guid.NewGuid(),CreateTime=DateTime.Now},
            new Transaction(){ID=Guid.NewGuid(),CreateTime=DateTime.Now},
            new Transaction(){ID=Guid.NewGuid(),CreateTime=DateTime.Now},
            new Transaction(){ID=Guid.NewGuid(),CreateTime=DateTime.Now},
            new Transaction(){ID=Guid.NewGuid(),CreateTime=DateTime.Now},
        };

        [EnableQuery(PageSize = 10, MaxExpansionDepth = 3)]
        public IQueryable<Transaction> Get()
        {
             
            return list.AsQueryable();
        }


        [HttpGet]
        public IHttpActionResult First()
        {
            var retval = list[0];

            return Ok(retval);
        }


    }
}
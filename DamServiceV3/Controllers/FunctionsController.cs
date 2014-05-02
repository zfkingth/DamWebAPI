using Hammergo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DamServiceV3.Controllers
{
    public class FunctionsController : ApiController
    {
        // GET api/functions
        public IEnumerable<ApparatusType> Get()
        {
            var type1 = new ApparatusType();
            type1.Id = Guid.NewGuid();
            type1.TypeName = "第一种类型";



            var type2 = new ApparatusType();
            type2.Id = Guid.NewGuid();
            type2.TypeName = "第二种类型";

            return new ApparatusType[] { type1, type2 };
        }

        // GET api/functions/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/functions
        public void Post([FromBody]string value)
        {
        }

        // PUT api/functions/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/functions/5
        public void Delete(int id)
        {
        }
    }
}

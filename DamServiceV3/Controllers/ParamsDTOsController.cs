using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using Hammergo.Data;
using System.Web.Http.OData.Query;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DamServiceV3.Controllers
{
    public class ParamsDTOsController :ApiController 
    {


        // GET odata/Apps
        [Queryable]
        public SingleResult<int> Get()
        {

            return SingleResult.Create((new int[]{ 1}).AsQueryable());
        }



        private DamWCFContext db = null;
        // POST odata/Apps
        public async Task<IHttpActionResult> Post(JObject dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            dynamic jdata = dto;

            JObject jAddedParams = jdata.AddedParams;

            var addedParams = jAddedParams.ToArray<AppParam>();

            //delay load
            db = new DamWCFContext();


            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

                throw;

            }

            return Created<string>("", "");
            //return Created(dto);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(db!=null)
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}

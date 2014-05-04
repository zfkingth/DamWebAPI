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
using DamServiceV3.Models;

namespace DamServiceV3.Controllers
{
    public class ParamsDTOsController :ApiController 
    {


        // GET odata/Apps
        [Queryable]
        public IQueryable<int> Get()
        {

            return (new int[] { 1 }).AsQueryable();
        }



        private DamWCFContext db = null;
        // POST odata/Apps
        public async Task<IHttpActionResult> Post(ParamsDTO dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            return ex
            //return Created(dto);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}

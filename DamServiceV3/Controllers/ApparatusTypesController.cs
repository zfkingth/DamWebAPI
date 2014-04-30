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

namespace DamServiceV3.Controllers
{
    /*
    To add a route for this controller, merge these statements into the Register method of the WebApiConfig class. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using Hammergo.Data;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<ApparatusType>("ApparatusTypes");
    builder.EntitySet<App>("App"); 
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ApparatusTypesController : ODataController
    {
        private DamWCFContext db = new DamWCFContext();

        // GET odata/ApparatusTypes
        [Queryable]
        public IQueryable<ApparatusType> GetApparatusTypes()
        {
            return db.ApparatusTypes;
        }

        // GET odata/ApparatusTypes(5)
        [Queryable]
        public SingleResult<ApparatusType> GetApparatusType([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.ApparatusTypes.Where(apparatustype => apparatustype.Id == key));
        }


        public HttpResponseMessage GetCount(ODataQueryOptions<ApparatusType> queryOptions)
        {
            IQueryable<ApparatusType> queryResults = queryOptions.ApplyTo(GetApparatusTypes()) as IQueryable<ApparatusType>;
            int count = queryResults.Count();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(count.ToString(), Encoding.UTF8, "text/plain");
            return response;
        }


        // PUT odata/ApparatusTypes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, ApparatusType apparatustype)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != apparatustype.Id)
            {
                return BadRequest();
            }

            db.Entry(apparatustype).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApparatusTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(apparatustype);
        }

        // POST odata/ApparatusTypes
        public async Task<IHttpActionResult> Post(ApparatusType apparatustype)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ApparatusTypes.Add(apparatustype);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ApparatusTypeExists(apparatustype.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(apparatustype);
        }

        // PATCH odata/ApparatusTypes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<ApparatusType> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApparatusType apparatustype = await db.ApparatusTypes.FindAsync(key);
            if (apparatustype == null)
            {
                return NotFound();
            }

            patch.Patch(apparatustype);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApparatusTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(apparatustype);
        }

        // DELETE odata/ApparatusTypes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            ApparatusType apparatustype = await db.ApparatusTypes.FindAsync(key);
            if (apparatustype == null)
            {
                return NotFound();
            }

            db.ApparatusTypes.Remove(apparatustype);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET odata/ApparatusTypes(5)/Apps
        [Queryable]
        public IQueryable<App> GetApps([FromODataUri] Guid key)
        {
            return db.ApparatusTypes.Where(m => m.Id == key).SelectMany(m => m.Apps);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ApparatusTypeExists(Guid key)
        {
            return db.ApparatusTypes.Count(e => e.Id == key) > 0;
        }
    }
}

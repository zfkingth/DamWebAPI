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
    builder.EntitySet<Remark>("Remarks");
    builder.EntitySet<App>("Apps"); 
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class RemarksController : ODataController
    {
        private DamWCFContext db = new DamWCFContext();

        // GET odata/Remarks
        [Queryable]
        public IQueryable<Remark> GetRemarks()
        {
            return db.Remarks;
        }




        public HttpResponseMessage GetCount(ODataQueryOptions<Remark> queryOptions)
        {
            IQueryable<Remark> queryResults = queryOptions.ApplyTo(GetRemarks()) as IQueryable<Remark>;
            int count = queryResults.Count();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(count.ToString(), Encoding.UTF8, "text/plain");
            return response;
        }




        // GET odata/Remarks(5)
        [Queryable]
        public SingleResult<Remark> GetRemark([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Remarks.Where(remark => remark.Id == key));
        }

        // PUT odata/Remarks(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Remark remark)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != remark.Id)
            {
                return BadRequest();
            }

            db.Entry(remark).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RemarkExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(remark);
        }

        // POST odata/Remarks
        public async Task<IHttpActionResult> Post(Remark remark)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Remarks.Add(remark);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RemarkExists(remark.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(remark);
        }

        // PATCH odata/Remarks(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Remark> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Remark remark = await db.Remarks.FindAsync(key);
            if (remark == null)
            {
                return NotFound();
            }

            patch.Patch(remark);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RemarkExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(remark);
        }

        // DELETE odata/Remarks(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            Remark remark = await db.Remarks.FindAsync(key);
            if (remark == null)
            {
                return NotFound();
            }

            db.Remarks.Remove(remark);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET odata/Remarks(5)/App
        [Queryable]
        public SingleResult<App> GetApp([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Remarks.Where(m => m.Id == key).Select(m => m.App));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RemarkExists(Guid key)
        {
            return db.Remarks.Count(e => e.Id == key) > 0;
        }
    }
}

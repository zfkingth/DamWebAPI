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
    builder.EntitySet<MessureValue>("MessureValues");
    builder.EntitySet<MessureParam>("AppParams"); 
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class MessureValuesController : ODataController
    {
        private DamWCFContext db = new DamWCFContext();

        // GET odata/MessureValues
        [Queryable]
        public IQueryable<MessureValue> GetMessureValues()
        {
            return db.MessureValues;
        }

        // GET odata/MessureValues(5)
        [Queryable]
        public SingleResult<MessureValue> GetMessureValue([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.MessureValues.Where(messurevalue => messurevalue.Id == key));
        }



        public HttpResponseMessage GetCount(ODataQueryOptions<MessureValue> queryOptions)
        {
            IQueryable<MessureValue> queryResults = queryOptions.ApplyTo(GetMessureValues()) as IQueryable<MessureValue>;
            int count = queryResults.Count();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(count.ToString(), Encoding.UTF8, "text/plain");
            return response;
        }



        // PUT odata/MessureValues(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, MessureValue messurevalue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != messurevalue.Id)
            {
                return BadRequest();
            }

            db.Entry(messurevalue).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessureValueExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(messurevalue);
        }

        // POST odata/MessureValues
        public async Task<IHttpActionResult> Post(MessureValue messurevalue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MessureValues.Add(messurevalue);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MessureValueExists(messurevalue.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(messurevalue);
        }

        // PATCH odata/MessureValues(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<MessureValue> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MessureValue messurevalue = await db.MessureValues.FindAsync(key);
            if (messurevalue == null)
            {
                return NotFound();
            }

            patch.Patch(messurevalue);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessureValueExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(messurevalue);
        }

        // DELETE odata/MessureValues(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            MessureValue messurevalue = await db.MessureValues.FindAsync(key);
            if (messurevalue == null)
            {
                return NotFound();
            }

            db.MessureValues.Remove(messurevalue);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET odata/MessureValues(5)/MessureParam
        [Queryable]
        public SingleResult<MessureParam> GetMessureParam([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.MessureValues.Where(m => m.Id == key).Select(m => m.MessureParam));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MessureValueExists(Guid key)
        {
            return db.MessureValues.Count(e => e.Id == key) > 0;
        }
    }
}

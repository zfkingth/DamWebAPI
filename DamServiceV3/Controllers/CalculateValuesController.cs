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
    builder.EntitySet<CalculateValue>("CalculateValues");
    builder.EntitySet<CalculateParam>("AppParams"); 
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CalculateValuesController : ODataController
    {
        private DamWCFContext db = new DamWCFContext();

        // GET odata/CalculateValues
        [Queryable]
        public IQueryable<CalculateValue> GetCalculateValues()
        {
            return db.CalculateValues;
        }

        // GET odata/CalculateValues(5)
        [Queryable]
        public SingleResult<CalculateValue> GetCalculateValue([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.CalculateValues.Where(calculatevalue => calculatevalue.Id == key));
        }



        public HttpResponseMessage GetCount(ODataQueryOptions<CalculateValue> queryOptions)
        {
            IQueryable<CalculateValue> queryResults = queryOptions.ApplyTo(GetCalculateValues()) as IQueryable<CalculateValue>;
            int count = queryResults.Count();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(count.ToString(), Encoding.UTF8, "text/plain");
            return response;
        }



        // PUT odata/CalculateValues(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, CalculateValue calculatevalue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != calculatevalue.Id)
            {
                return BadRequest();
            }

            db.Entry(calculatevalue).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CalculateValueExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(calculatevalue);
        }

        // POST odata/CalculateValues
        public async Task<IHttpActionResult> Post(CalculateValue calculatevalue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CalculateValues.Add(calculatevalue);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CalculateValueExists(calculatevalue.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(calculatevalue);
        }

        // PATCH odata/CalculateValues(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<CalculateValue> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CalculateValue calculatevalue = await db.CalculateValues.FindAsync(key);
            if (calculatevalue == null)
            {
                return NotFound();
            }

            patch.Patch(calculatevalue);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CalculateValueExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(calculatevalue);
        }

        // DELETE odata/CalculateValues(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            CalculateValue calculatevalue = await db.CalculateValues.FindAsync(key);
            if (calculatevalue == null)
            {
                return NotFound();
            }

            db.CalculateValues.Remove(calculatevalue);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET odata/CalculateValues(5)/CalculateParam
        [Queryable]
        public SingleResult<CalculateParam> GetCalculateParam([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.CalculateValues.Where(m => m.Id == key).Select(m => m.CalculateParam));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CalculateValueExists(Guid key)
        {
            return db.CalculateValues.Count(e => e.Id == key) > 0;
        }
    }
}

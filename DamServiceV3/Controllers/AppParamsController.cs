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
    builder.EntitySet<AppParam>("AppParams");
    builder.EntitySet<App>("Apps"); 
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class AppParamsController : ODataController
    {
        private DamWCFContext db = new DamWCFContext();

        // GET odata/AppParams
        [Queryable(PageSize=50)]
        public IQueryable<AppParam> GetAppParams()
        {
            return db.AppParams;
        }

        // GET odata/AppParams(5)
        [Queryable]
        public SingleResult<AppParam> GetAppParam([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.AppParams.Where(appparam => appparam.Id == key));
        }

        public HttpResponseMessage GetCount(ODataQueryOptions<AppParam> queryOptions)
        {
            IQueryable<AppParam> queryResults = queryOptions.ApplyTo(GetAppParams()) as IQueryable<AppParam>;
            int count = queryResults.Count();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(count.ToString(), Encoding.UTF8, "text/plain");
            return response;
        }




        // PUT odata/AppParams(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, AppParam appparam)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != appparam.Id)
            {
                return BadRequest();
            }

            db.Entry(appparam).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppParamExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(appparam);
        }

        // POST odata/AppParams
        public async Task<IHttpActionResult> Post(AppParam appparam)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AppParams.Add(appparam);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AppParamExists(appparam.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(appparam);
        }

        // PATCH odata/AppParams(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<AppParam> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AppParam appparam = await db.AppParams.FindAsync(key);
            if (appparam == null)
            {
                return NotFound();
            }

            patch.Patch(appparam);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppParamExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(appparam);
        }

        // DELETE odata/AppParams(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            AppParam appparam = await db.AppParams.FindAsync(key);
            if (appparam == null)
            {
                return NotFound();
            }

            db.AppParams.Remove(appparam);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET odata/AppParams(5)/App
        [Queryable]
        public SingleResult<App> GetApp([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.AppParams.Where(m => m.Id == key).Select(m => m.App));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AppParamExists(Guid key)
        {
            return db.AppParams.Count(e => e.Id == key) > 0;
        }
    }
}

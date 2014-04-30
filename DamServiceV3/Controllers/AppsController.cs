﻿using System;
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
    builder.EntitySet<App>("Apps");
    builder.EntitySet<ApparatusType>("ApparatusTypes"); 
    builder.EntitySet<AppParam>("AppParam"); 
    builder.EntitySet<ProjectPart>("ProjectParts"); 
    builder.EntitySet<Remark>("Remark"); 
    builder.EntitySet<TaskApp>("TaskApp"); 
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class AppsController : ODataController
    {
        private DamWCFContext db = new DamWCFContext();

        // GET odata/Apps
        [Queryable]
        public IQueryable<App> GetApps()
        {
            return db.Apps;
        }

        // GET odata/Apps(5)
        [Queryable]
        public SingleResult<App> GetApp([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Apps.Where(app => app.Id == key));
        }


        public HttpResponseMessage GetCount(ODataQueryOptions<ApparatusType> queryOptions)
        {
            IQueryable<ApparatusType> queryResults = queryOptions.ApplyTo(GetApps()) as IQueryable<ApparatusType>;
            int count = queryResults.Count();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(count.ToString(), Encoding.UTF8, "text/plain");
            return response;
        }



        // PUT odata/Apps(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, App app)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != app.Id)
            {
                return BadRequest();
            }

            db.Entry(app).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(app);
        }

        // POST odata/Apps
        public async Task<IHttpActionResult> Post(App app)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Apps.Add(app);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AppExists(app.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(app);
        }

        // PATCH odata/Apps(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<App> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            App app = await db.Apps.FindAsync(key);
            if (app == null)
            {
                return NotFound();
            }

            patch.Patch(app);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(app);
        }

        // DELETE odata/Apps(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            App app = await db.Apps.FindAsync(key);
            if (app == null)
            {
                return NotFound();
            }

            db.Apps.Remove(app);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET odata/Apps(5)/ApparatusType
        [Queryable]
        public SingleResult<ApparatusType> GetApparatusType([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Apps.Where(m => m.Id == key).Select(m => m.ApparatusType));
        }

        // GET odata/Apps(5)/AppParams
        [Queryable]
        public IQueryable<AppParam> GetAppParams([FromODataUri] Guid key)
        {
            return db.Apps.Where(m => m.Id == key).SelectMany(m => m.AppParams);
        }

        // GET odata/Apps(5)/ProjectPart
        [Queryable]
        public SingleResult<ProjectPart> GetProjectPart([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Apps.Where(m => m.Id == key).Select(m => m.ProjectPart));
        }

        // GET odata/Apps(5)/Remarks
        [Queryable]
        public IQueryable<Remark> GetRemarks([FromODataUri] Guid key)
        {
            return db.Apps.Where(m => m.Id == key).SelectMany(m => m.Remarks);
        }

        // GET odata/Apps(5)/TaskApps
        [Queryable]
        public IQueryable<TaskApp> GetTaskApps([FromODataUri] Guid key)
        {
            return db.Apps.Where(m => m.Id == key).SelectMany(m => m.TaskApps);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AppExists(Guid key)
        {
            return db.Apps.Count(e => e.Id == key) > 0;
        }
    }
}
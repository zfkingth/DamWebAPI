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
    builder.EntitySet<ProjectPart>("ProjectParts");
    builder.EntitySet<App>("App"); 
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ProjectPartsController : ODataController
    {
        private DamWCFContext db = new DamWCFContext();

        // GET odata/ProjectParts
        [Queryable]
        public IQueryable<ProjectPart> GetProjectParts()
        {
            return db.ProjectParts;
        }

        // GET odata/ProjectParts(5)
        [Queryable]
        public SingleResult<ProjectPart> GetProjectPart([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.ProjectParts.Where(projectpart => projectpart.Id == key));
        }


        /// <summary>
        /// 更新一系列测点的工程部位
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateAppsProject([FromODataUri] Guid key, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var appids = (IEnumerable<Guid>)parameters["appids"];

             bool exist =  db.ProjectParts.Any(s => s.Id == key);

            if (exist == false)
            {
                throw new Exception("projectPartID,参数错误");
            }

            exist = db.ProjectParts.Any(s => s.ParentPart == key);

            if (exist)
            {
                throw new Exception("所关联的部位不能有子结点");
            }

            var qapps = from i in db.Apps
                        where appids.Contains(i.Id)
                        select i;

            foreach (var item in qapps)
            {
                item.ProjectPartID = key;
            }

            db.SaveChanges();

            return Ok(true);
        
 
        }

        public HttpResponseMessage GetCount(ODataQueryOptions<ProjectPart> queryOptions)
        {
            IQueryable<ProjectPart> queryResults = queryOptions.ApplyTo(GetProjectParts()) as IQueryable<ProjectPart>;
            int count = queryResults.Count();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(count.ToString(), Encoding.UTF8, "text/plain");
            return response;
        }


        // PUT odata/ProjectParts(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, ProjectPart projectpart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != projectpart.Id)
            {
                return BadRequest();
            }

            db.Entry(projectpart).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectPartExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(projectpart);
        }

        // POST odata/ProjectParts
        public async Task<IHttpActionResult> Post(ProjectPart projectpart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProjectParts.Add(projectpart);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProjectPartExists(projectpart.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(projectpart);
        }

        // PATCH odata/ProjectParts(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<ProjectPart> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProjectPart projectpart = await db.ProjectParts.FindAsync(key);
            if (projectpart == null)
            {
                return NotFound();
            }

            patch.Patch(projectpart);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectPartExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(projectpart);
        }

        // DELETE odata/ProjectParts(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            ProjectPart projectpart = await db.ProjectParts.FindAsync(key);
            if (projectpart == null)
            {
                return NotFound();
            }

            db.ProjectParts.Remove(projectpart);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET odata/ProjectParts(5)/Apps
        [Queryable]
        public IQueryable<App> GetApps([FromODataUri] Guid key)
        {
            return db.ProjectParts.Where(m => m.Id == key).SelectMany(m => m.Apps);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProjectPartExists(Guid key)
        {
            return db.ProjectParts.Count(e => e.Id == key) > 0;
        }
    }
}

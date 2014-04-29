using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Hammergo.Data;

namespace DamService.Controllers
{
    public class ApparatusTypesController : ODataController
    {
        private DamWCFContext context = new DamWCFContext();

        [EnableQuery( MaxExpansionDepth = 2)]
        public IHttpActionResult Get()
        {
            return Ok(context.ApparatusTypes);
        }

        public async Task<IHttpActionResult> Post([FromBody] ApparatusType entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            context.ApparatusTypes.Add(entity);
            await context.SaveChangesAsync();
            return Created(entity);
        }

        public async Task<IHttpActionResult> Put([FromODataUri] int key, [FromBody] ApparatusType entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (key != entity.Id)
            {
                return BadRequest("The key from the url must match the key of the entity in the body");
            }
            var originalCustomer = await context.ApparatusTypes.FindAsync(key);
            if (originalCustomer == null)
            {
                return NotFound();
            }
            else
            {
                context.Entry(originalCustomer).CurrentValues.SetValues(entity);
                await context.SaveChangesAsync();
            }
            return Updated(entity);
        }

        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ApparatusType> patch)
        {
            object id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (patch.TryGetPropertyValue("Id", out id) && (int)id != key)
            {
                return BadRequest("The key from the url must match the key of the entity in the body");
            }
            ApparatusType originalEntity = await context.ApparatusTypes.FindAsync(key);
            if (originalEntity == null)
            {
                return NotFound();
            }
            else
            {
                patch.Patch(originalEntity);
                await context.SaveChangesAsync();
            }
            return Updated(originalEntity);
        }


        public async Task<IHttpActionResult> Delete([FromODataUri]int key)
        {
            ApparatusType entity = await context.ApparatusTypes.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            else
            {
                context.ApparatusTypes.Remove(entity);
                await context.SaveChangesAsync();
                return StatusCode(HttpStatusCode.NoContent);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }
            }
        } 
    }
}
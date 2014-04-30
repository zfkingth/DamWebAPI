using DamService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using System.Collections.Concurrent;
using System.Net;

namespace DamService.Controllers
{
    public class TransactionsController : ODataController
    {

        private static ConcurrentDictionary<Guid, Transaction> _data;

        static TransactionsController()
        {
            _data = new ConcurrentDictionary<Guid, Transaction>();
            var rand = new Random();

           for(int i=0;i<10;i++)
           {
               var item = new Transaction() { Id = Guid.NewGuid(), CreateTime = DateTime.Now };

               _data.TryAdd(item.Id, item);
           }
        }



        [EnableQuery(MaxExpansionDepth = 2)]
        public IQueryable<Transaction> Get()
        {
            return _data.Values.AsQueryable();
        }


        public IHttpActionResult Get(Guid key)
        {
                  Transaction retval;
                  if (_data.TryGetValue(key, out retval))
                  {
                      return Ok(retval);
                  }
                  else
                  {
                      return NotFound();
                  }
            
        }


        public  IHttpActionResult Post([FromBody] Transaction entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _data.TryAdd(entity.Id, entity);
            return Created(entity);
        }

        public IHttpActionResult Put([FromODataUri] Guid key, [FromBody] Transaction entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (key != entity.Id)
            {
                return BadRequest("The key from the url must match the key of the entity in the body");
            }
            Transaction originalEntity;
            _data.TryGetValue(key, out originalEntity);
            if (originalEntity == null)
            {
                return NotFound();
            }
            else
            {
                //context.Entry(originalEntity).CurrentValues.SetValues(entity);
                //await context.SaveChangesAsync();
                originalEntity.CreateTime = entity.CreateTime;
            }
            return Updated(entity);
        }

        [AcceptVerbs("PATCH", "MERGE")]
        public   IHttpActionResult Patch([FromODataUri] Guid key, Delta<Transaction> patch)
        {
            object id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (patch.TryGetPropertyValue("Id", out id) && (Guid) id != key)
            {
                return BadRequest("The key from the url must match the key of the entity in the body");
            }
            Transaction originalEntity;
            _data.TryGetValue(key, out originalEntity); 

            if (originalEntity == null)
            {
                return NotFound();
            }
            else
            {
                patch.Patch(originalEntity);
            }
            return Updated(originalEntity);
        }


        public  IHttpActionResult Delete([FromODataUri]Guid key)
        {
            Transaction entity;
            _data.TryGetValue(key, out entity); ;
            if (entity == null)
            {
                return NotFound();
            }
            else
            {
                _data.TryRemove(key, out entity);
                return StatusCode(HttpStatusCode.NoContent);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //if (context != null)
                //{
                //    context.Dispose();
                //    context = null;
                //}
            }
        }


        [HttpGet]
        public IHttpActionResult First()
        {
            var retval = _data.First().Value;

            return Ok(retval);
        }


    }
}
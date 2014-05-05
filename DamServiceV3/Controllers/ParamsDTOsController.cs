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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DamServiceV3.Models;
using System.Data.Entity.Core.Objects;

namespace DamServiceV3.Controllers
{
    public class ParamsDTOsController : ApiController
    {


        // GET odata/Apps
        [Queryable]
        public SingleResult<int> Get()
        {

            return SingleResult.Create((new int[] { 1 }).AsQueryable());
        }



        private DamWCFContext db = null;
        // POST odata/Apps
        public async Task<IHttpActionResult> Post(JObject rdto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var converter = new Helper.AppParamConverter();
            var dto = JsonConvert.DeserializeObject<ParamsDTO>(rdto.ToString(), converter);



            //delay load
            //check logic in entity context
            db = new DamWCFContext(true);

            if (dto.AddedParams!=null)
            db.AppParams.AddRange(dto.AddedParams);

            if (dto.UpdatedParams != null)
            {
                foreach (var item in dto.UpdatedParams)
                {
                    db.AppParams.Attach(item);

                    db.Entry<AppParam>(item).State = EntityState.Modified;
                }
            }

            if (dto.DeletedParams != null)
            {
                foreach (var item in dto.DeletedParams)
                {
                    db.AppParams.Attach(item);

                    db.Entry<AppParam>(item).State = EntityState.Deleted;
                }
            };

            if (dto.AddedFormulae != null)
            db.Formulae.AddRange(dto.AddedFormulae);

            if (dto.UpdatedFormulae != null)
            {
                foreach (var item in dto.UpdatedFormulae)
                {
                    db.Formulae.Attach(item);

                    db.Entry<Formula>(item).State = EntityState.Modified;
                }
            }

            if (dto.DeletedFormulae != null)
            {
                foreach (var item in dto.DeletedFormulae)
                {
                    db.Formulae.Attach(item);

                    db.Entry<Formula>(item).State = EntityState.Deleted;
                }
            }


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {

                throw;

            }

            return Created< string>(this.Url.Request.RequestUri, "ok");
             //return Created(dto);
        }

    

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}

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
using System.Text;
using System.Web.Http.OData.Query;

namespace DamServiceV3.Controllers
{
    /*
    To add a route for this controller, merge these statements into the Register method of the WebApiConfig class. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using Hammergo.Data;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Formula>("Formulae");
    builder.EntitySet<CalculateParam>("AppParams"); 
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class FormulaeController : ODataController
    {
        private DamWCFContext db = new DamWCFContext();

        // GET odata/Formulae
        [Queryable]
        public IQueryable<Formula> GetFormulae()
        {
            return db.Formulae;
        }

        // GET odata/Formulae(5)
        [Queryable]
        public SingleResult<Formula> GetFormula([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Formulae.Where(formula => formula.Id == key));
        }


        public HttpResponseMessage GetCount(ODataQueryOptions<Formula> queryOptions)
        {
            IQueryable<Formula> queryResults = queryOptions.ApplyTo(GetFormulae()) as IQueryable<Formula>;
            int count = queryResults.Count();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(count.ToString(), Encoding.UTF8, "text/plain");
            return response;
        }

        // GET odata/Formulae(5)/CalculateParam
        [Queryable]
        public SingleResult<CalculateParam> GetCalculateParam([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Formulae.Where(m => m.Id == key).Select(m => m.CalculateParam));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FormulaExists(Guid key)
        {
            return db.Formulae.Count(e => e.Id == key) > 0;
        }
    }
}

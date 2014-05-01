using Hammergo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;

namespace DamServiceV3.Controllers
{
    public   class MyEntitySetController<TEntity, TKey> : EntitySetController<TEntity, TKey>where TEntity : class 
    {
        protected DamWCFContext db = new DamWCFContext();

        public HttpResponseMessage GetCount(ODataQueryOptions<TEntity> queryOptions)
        {
            IQueryable<TEntity> queryResults = queryOptions.ApplyTo(Get()) as IQueryable<TEntity>;
            int count = queryResults.Count();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(count.ToString(), Encoding.UTF8, "text/plain");
            return response;
        }
    }
}
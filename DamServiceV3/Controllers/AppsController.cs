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
using System.Data.Entity.Core.Objects;

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


        public HttpResponseMessage GetCount(ODataQueryOptions<App> queryOptions)
        {
            IQueryable<App> queryResults = queryOptions.ApplyTo(GetApps()) as IQueryable<App>;
            int count = queryResults.Count();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(count.ToString(), Encoding.UTF8, "text/plain");
            return response;
        }


        [HttpPost]
        [Queryable]
        public IQueryable<Formula> GetAllFormulaeByAppID([FromODataUri] Guid key)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var qf = (from p in db.AppParams
                      where p.AppId == key
                      join f in db.Formulae
                      on p.Id equals f.ParamId
                      select f).AsNoTracking();

            return qf;
        }

        [HttpPost]
        public IQueryable<App> SearcyAppByName(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            string match = (string)parameters["match"];

            string entitySQL =
    @"SELECT VALUE c FROM Apps AS c WHERE c.AppName like @match;";
            ObjectParameter[] ps = { new ObjectParameter("match", match) };
            var query = ((IObjectContextAdapter)db).ObjectContext.CreateQuery<App>(entitySQL, ps);


            return query;


        }

        [HttpPost]
        public IQueryable<App> SearcyAppCalcName(ODataActionParameters parameters)
        {

            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            string match = (string)parameters["match"];

            string entitySQL =
    @"SELECT VALUE c FROM Apps AS c WHERE c.CalculateName like @match;";
            ObjectParameter[] ps = { new ObjectParameter("match", match) };
            var query = ((IObjectContextAdapter)db).ObjectContext.CreateQuery<App>(entitySQL, ps);


            return query;


        }

        /// <summary>
        /// 获取应用该测点的子测点的计算名称
        /// </summary>
        /// <param name="appCalcName"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        public IQueryable<string> GetChildAppCalcName(ODataActionParameters parameters)
        {

            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            string appCalcName = (string)parameters["appCalcName"];
            DateTimeOffset date = (DateTimeOffset)parameters["date"];

            IQueryable<string> names = null;

            names = from i in db.Apps
                    join p in db.AppParams.OfType<CalculateParam>()
                    on i.Id equals p.AppId
                    join f in db.Formulae
                    on p.Id equals f.ParamId
                    where f.FormulaExpression.Contains(appCalcName + ".") && f.StartDate <= date && date < f.EndDate
                    select i.CalculateName;



            return names;

        }

        [HttpPost]
        public IQueryable<CalculateValue> GetCalcValues(ODataActionParameters parameters)
        {

            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }



            var appids = (IEnumerable<Guid>)parameters["appids"];
            int topNum = (int)parameters["topNum"];
            DateTimeOffset? startDate = (DateTimeOffset?)parameters["startDate"];

            DateTimeOffset? endDate = (DateTimeOffset?)parameters["endDate"];

            IQueryable<CalculateValue> values = null;


            foreach (var id in appids)
            {
                var calcParamsCount = (from i in db.AppParams
                                       where i.AppId == id && i is CalculateParam
                                       select i).Count();


                var ret = GetCalcValues(calcParamsCount * topNum, startDate, endDate, id);
                if (values == null)
                {
                    values = ret;
                }
                else
                {
                    values = values.Union(ret);
                }

            }

            return values;


        }



        private IQueryable<CalculateValue> GetCalcValues(int topNum, DateTimeOffset? startDate, DateTimeOffset? endDate, Guid appId)
        {
            var values = from i in db.CalculateValues
                         join p in db.AppParams.OfType<CalculateParam>()
                         on i.ParamId equals p.Id
                         where p.AppId == appId
                         select i;

            if (startDate.HasValue)
            {

                if (endDate.HasValue)
                {
                    values = values.Where(i => i.Date >= startDate && i.Date <= endDate);

                    // sql = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} and CalculateValue.Date >= @startDate and CalculateValue.Date <= @endDate ", snCondition, SQL_Field);
                }
                else if (topNum > 0)
                {
                    values = values.Where(i => i.Date >= startDate).OrderBy(i => i.Date).Take(topNum);

                    //   sql = string.Format("select top {0}  {2} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {1} and  CalculateValue.Date>= @startDate order by CalculateValue.Date  asc", topNum, snCondition, SQL_Field);
                }
                else
                {
                    values = values.Where(i => i.Date >= startDate);
                    //  sql = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} and  CalculateValue.Date>= @startDate ", snCondition, SQL_Field);
                }
            }
            else if (endDate.HasValue)
            {

                if (topNum > 0)
                {
                    values = values.Where(i => i.Date <= endDate).OrderByDescending(i => i.Date).Take(topNum);
                    // sql = string.Format("select top {0}  {2} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {1} and  CalculateValue.Date<= @endDate order by CalculateValue.Date  desc", topNum, snCondition, SQL_Field);
                }
                else
                {
                    values = values.Where(i => i.Date <= endDate);
                    // s = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} and  CalculateValue.Date<= @endDate ", snCondition, SQL_Field);
                }
            }
            else
            {
                if (topNum > 0)
                {
                    values = values.OrderByDescending(i => i.Date).Take(topNum);

                    //   sql = string.Format("select top {0}  {2} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {1} order by CalculateValue.Date desc", topNum, snCondition, SQL_Field);
                }
                else
                {

                    //   sql = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} ", snCondition, SQL_Field);
                }

            }

            return values;
        }

        [HttpPost]
        public IQueryable<MessureValue> GetMesValues(ODataActionParameters parameters)
        {

            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }


            var appids = (IEnumerable<Guid>)parameters["appids"];
            int topNum = (int)parameters["topNum"];
            DateTimeOffset? startDate = (DateTimeOffset?)parameters["startDate"];

            DateTimeOffset? endDate = (DateTimeOffset?)parameters["endDate"]; ;

            IQueryable<MessureValue> values = null;


            foreach (var id in appids)
            {
                var mesParamsCount = (from i in db.AppParams
                                      where i.AppId == id && i is MessureParam
                                      select i).Count();

                var ret = GetMesValues(mesParamsCount * topNum, startDate, endDate, id);
                if (values == null)
                {
                    values = ret;
                }
                else
                {
                    values = values.Union(ret);
                }

            }

            return values;

        }



        private IQueryable<MessureValue> GetMesValues(int topNum, DateTimeOffset? startDate, DateTimeOffset? endDate, Guid appId)
        {

            var values = from i in db.MessureValues
                         join p in db.AppParams.OfType<MessureParam>()
                         on i.ParamId equals p.Id
                         where p.AppId == appId
                         select i;

            if (startDate.HasValue)
            {

                if (endDate.HasValue)
                {
                    values = values.Where(i => i.Date >= startDate && i.Date <= endDate);

                    // sql = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} and CalculateValue.Date >= @startDate and CalculateValue.Date <= @endDate ", snCondition, SQL_Field);
                }
                else if (topNum > 0)
                {
                    values = values.Where(i => i.Date >= startDate).OrderBy(i => i.Date).Take(topNum);

                    //   sql = string.Format("select top {0}  {2} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {1} and  CalculateValue.Date>= @startDate order by CalculateValue.Date  asc", topNum, snCondition, SQL_Field);
                }
                else
                {
                    values = values.Where(i => i.Date >= startDate);
                    //  sql = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} and  CalculateValue.Date>= @startDate ", snCondition, SQL_Field);
                }
            }
            else if (endDate.HasValue)
            {

                if (topNum > 0)
                {
                    values = values.Where(i => i.Date <= endDate).OrderByDescending(i => i.Date).Take(topNum);
                    // sql = string.Format("select top {0}  {2} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {1} and  CalculateValue.Date<= @endDate order by CalculateValue.Date  desc", topNum, snCondition, SQL_Field);
                }
                else
                {
                    values = values.Where(i => i.Date <= endDate);
                    // s = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} and  CalculateValue.Date<= @endDate ", snCondition, SQL_Field);
                }
            }
            else
            {
                if (topNum > 0)
                {
                    values = values.OrderByDescending(i => i.Date).Take(topNum);

                    //   sql = string.Format("select top {0}  {2} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {1} order by CalculateValue.Date desc", topNum, snCondition, SQL_Field);
                }
                else
                {

                    //   sql = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} ", snCondition, SQL_Field);
                }

            }

            return values;
        }


        [HttpPost]
        public IQueryable<Remark> GetRemarks(ODataActionParameters parameters)
        {

            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }



            var appids = (IEnumerable<Guid>)parameters["appids"];
            int topNum = (int)parameters["topNum"];
            DateTimeOffset? startDate = (DateTimeOffset?)parameters["startDate"];

            DateTimeOffset? endDate = (DateTimeOffset?)parameters["endDate"]; ; ;

            IQueryable<Remark> values = null;

            foreach (var id in appids)
            {
                var ret = GetRemarks(topNum, startDate, endDate, id);
                if (values == null)
                {
                    values = ret;
                }
                else
                {
                    values = values.Union(ret);
                }

            }

            return values;


        }

        private IQueryable<Remark> GetRemarks(int topNum, DateTimeOffset? startDate, DateTimeOffset? endDate, Guid appId)
        {
            var values = from i in db.Remarks
                         where i.AppId == appId
                         select i;

            if (startDate.HasValue)
            {

                if (endDate.HasValue)
                {
                    values = values.Where(i => i.Date >= startDate && i.Date <= endDate);

                    // sql = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} and CalculateValue.Date >= @startDate and CalculateValue.Date <= @endDate ", snCondition, SQL_Field);
                }
                else if (topNum > 0)
                {
                    values = values.Where(i => i.Date >= startDate).OrderBy(i => i.Date).Take(topNum);

                    //   sql = string.Format("select top {0}  {2} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {1} and  CalculateValue.Date>= @startDate order by CalculateValue.Date  asc", topNum, snCondition, SQL_Field);
                }
                else
                {
                    values = values.Where(i => i.Date >= startDate);
                    //  sql = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} and  CalculateValue.Date>= @startDate ", snCondition, SQL_Field);
                }
            }
            else if (endDate.HasValue)
            {

                if (topNum > 0)
                {
                    values = values.Where(i => i.Date <= endDate).OrderByDescending(i => i.Date).Take(topNum);
                    // sql = string.Format("select top {0}  {2} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {1} and  CalculateValue.Date<= @endDate order by CalculateValue.Date  desc", topNum, snCondition, SQL_Field);
                }
                else
                {
                    values = values.Where(i => i.Date <= endDate);
                    // s = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} and  CalculateValue.Date<= @endDate ", snCondition, SQL_Field);
                }
            }
            else
            {
                if (topNum > 0)
                {
                    values = values.OrderByDescending(i => i.Date).Take(topNum);

                    //   sql = string.Format("select top {0}  {2} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {1} order by CalculateValue.Date desc", topNum, snCondition, SQL_Field);
                }
                else
                {

                    //   sql = string.Format("select  {1} from CalculateParam INNER JOIN CalculateValue ON CalculateParam.CalculateParamID = CalculateValue.calculateParamID where {0} ", snCondition, SQL_Field);
                }

            }

            return values;
        }

        /// <summary>
        /// 判断测点在某一时刻的数据是否存在
        /// </summary>

        public IHttpActionResult CheckExistData([FromODataUri] Guid key, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            bool exist = false;

            DateTimeOffset date = (DateTimeOffset)parameters["date"];

            var cnt1 = (from p in db.AppParams.OfType<MessureParam>()
                        where p.AppId == key
                        join val in db.MessureValues
                         on p.Id equals val.ParamId
                        where val.Date == date
                        select val).Count();

            if (cnt1 > 0)
            {
                //有数据
                exist = true;
            }
            else
            {
                //没有测量数据，判断是否有计算数据，因为存在一种可能，测点只有计算数据
                var cnt2 = (from p in db.AppParams.OfType<CalculateParam>()
                            where p.AppId == key
                            join val in db.CalculateValues
                             on p.Id equals val.ParamId
                            where val.Date == date
                            select val).Count();
                exist = cnt2 > 0 ? true : false;
            }



            return Ok(exist);

        }






        [HttpPost]
        public IHttpActionResult RateAllProducts(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int rating = (int)parameters["Rating"];

            int average = rating * 2;

            return Ok(average);
        }


        [HttpPost]
        public IHttpActionResult RateProduct([FromODataUri] Guid key, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int rating = (int)parameters["Rating"];



            int average = rating * 2;

            return Ok(average);
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

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Hammergo.Data.Mapping;
using System.Linq;
using System.Collections.Generic;
using Hammergo.Data.Logic;
using System;
using System.Data;
using System.Data.Entity.Core.Objects;

namespace Hammergo.Data
{
    public partial class DamWCFContext : DbContext
    {
        static DamWCFContext()
        {
            Database.SetInitializer<DamWCFContext>(null);
        }

        public DamWCFContext()
            : base("Data Source=.;Initial Catalog=DamWebApi;Integrated Security=True;MultipleActiveResultSets=True")
        {
            var objCtx = ((IObjectContextAdapter)this).ObjectContext;
            objCtx.SavingChanges += objCtx_SavingChanges;
        }



        #region Business Logic
        void objCtx_SavingChanges(object sender, System.EventArgs e)
        {
            //  throw new System.NotImplementedException();

            checkAppCalcInfo((ObjectContext)sender);

        }

        /// <summary>
        /// 检查测点计算信息是否有效
        /// </summary>
        /// <param name="objContext"></param>
        void checkAppCalcInfo(ObjectContext objContext)
        {
            var paramsEntries = new List<ObjectStateEntry>();
            var formulaEntries = new List<ObjectStateEntry>();
            foreach (var entry in (objContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted)))
            {
                if (entry.Entity is AppParam)
                {
                    paramsEntries.Add(entry);
                }
                else if (entry.Entity is Formula)
                {
                    formulaEntries.Add(entry);
                }
            }

            if (paramsEntries.Count + formulaEntries.Count > 0)
            {
                IEnumerable<Guid> ids1 = null;
                IEnumerable<Guid> ids2 = null;
                IEnumerable<Guid> ids = null;

                //有更改，需要检查
                //获取更改参数的app
                if (paramsEntries.Count > 0)
                {
                    //参数变化引起的
                    //var ids = (from i in paramsEntries
                    //              select (i.Entity as AppParam).AppId).Distinct();

                    ids1 = (from p in paramsEntries
                            select (p.Entity as AppParam).AppId
                              ).Distinct();

                }

                //获取更改公式的app
                if (formulaEntries.Count > 0)
                {
                    //公式变化引起的更改
                    var pids = (from f in formulaEntries
                                select (f.Entity as Formula).ParamId
                             ).Distinct();
                    ids2 = (from p in this.AppParams
                            where pids.Contains(p.ParamId)
                            select p.AppId).Distinct();
                }

                if (ids1 == null)
                {
                    //测点参数引起的变化
                    ids = ids2;
                }
                else if (ids2 == null)
                {
                    //公式引起的变化
                    ids = ids1;
                }
                else
                {
                    //both not null
                    ids = ids1.Union(ids2);
                }

                var appIds = ids.ToList();

                //在添加测点时从数据库中查不到app
                //首先从local查询
                var appList = (from i in this.Apps.Local
                               where appIds.Contains(i.AppId)
                               select i).ToList();

                var idsInLocal = (from i in appList
                                  select i.AppId).ToList();

                var idsInDb = appIds.Except(idsInLocal).ToList();

                if (idsInDb.Count > 0)
                {
                    //有测点数据在数据库中,//没有查询到的从数据库中查询
                    var appIndb = (from i in this.Apps
                                   where idsInDb.Contains(i.AppId)
                                   select i).AsNoTracking().ToList();
                    appList = appList.Union(appIndb).ToList();
                }

                foreach (App item in appList)
                {
                    //var paramQuery = (from i in paramsEntries
                    //                  where (i.Entity as AppParam).AppId == item.AppId
                    //                  select i).ToList();

                    //var formulaQuery = (from i in this.AppParams.OfType<CalculateParam>()
                    //                    where i.AppId == item.AppId
                    //                    select i.ParamId).Distinct();
                    var validation = new ParamsValidatation(this, item, paramsEntries, formulaEntries);
                    validation.Validate();
                }
            }
        }
        #endregion

        public DbSet<App> Apps { get; set; }
        public DbSet<ApparatusType> ApparatusTypes { get; set; }
        public DbSet<AppCollection> AppCollections { get; set; }
        public DbSet<CalculateParam> CalculateParams { get; set; }
        public DbSet<CalculateValue> CalculateValues { get; set; }
        public DbSet<ConstantParam> ConstantParams { get; set; }
        public DbSet<Formula> Formulae { get; set; }
        public DbSet<MessureParam> MessureParams { get; set; }
        public DbSet<MessureValue> MessureValues { get; set; }
        public DbSet<ProjectPart> ProjectParts { get; set; }
        public DbSet<Remark> Remarks { get; set; }
        public DbSet<TaskApp> TaskApps { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<AppParam> AppParams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AppMap());
            modelBuilder.Configurations.Add(new ApparatusTypeMap());
            modelBuilder.Configurations.Add(new AppCollectionMap());
            modelBuilder.Configurations.Add(new CalculateValueMap());
            modelBuilder.Configurations.Add(new FormulaMap());
            modelBuilder.Configurations.Add(new MessureValueMap());
            modelBuilder.Configurations.Add(new ProjectPartMap());
            modelBuilder.Configurations.Add(new RemarkMap());
            modelBuilder.Configurations.Add(new TaskAppMap());
            modelBuilder.Configurations.Add(new TaskTypeMap());
            modelBuilder.Configurations.Add(new AppParamMap());
        }
    }
}

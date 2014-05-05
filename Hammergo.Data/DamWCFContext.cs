using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Hammergo.Data.Mapping;
using System.Linq;
using System.Collections.Generic;
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

        public DamWCFContext():this(true)
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkLogic">在数据库操作中是否检查应用逻辑</param>
        public DamWCFContext(bool checkLogic)
            : base("Data Source=.;Initial Catalog=DamWebApi;Integrated Security=True;MultipleActiveResultSets=True")
        {
            //在业务逻辑中检查
            //if (checkLogic)
            //{
            //    var objCtx = ((IObjectContextAdapter)this).ObjectContext;
            //    objCtx.SavingChanges += objCtx_SavingChanges;
            //}
        }

 
      
   

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

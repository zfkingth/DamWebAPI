using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EF5x.Models.Mapping;

namespace EF5x.Models
{
    public partial class DamDBContext : DbContext
    {
        static DamDBContext()
        {
            Database.SetInitializer<DamDBContext>(null);
        }

        public DamDBContext()
            : base("Name=DamDBContext")
        {
        }

        public DbSet<Apparatus> Apparatus { get; set; }
        public DbSet<ApparatusType> ApparatusTypes { get; set; }
        public DbSet<AppCollection> AppCollections { get; set; }
        public DbSet<CalculateParam> CalculateParams { get; set; }
        public DbSet<CalculateValue> CalculateValues { get; set; }
        public DbSet<ConstantParam> ConstantParams { get; set; }
        public DbSet<MessureParam> MessureParams { get; set; }
        public DbSet<MessureValue> MessureValues { get; set; }
        public DbSet<ProjectPart> ProjectParts { get; set; }
        public DbSet<Remark> Remarks { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SysUser> SysUsers { get; set; }
        public DbSet<TaskAppratu> TaskAppratus { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ApparatusMap());
            modelBuilder.Configurations.Add(new ApparatusTypeMap());
            modelBuilder.Configurations.Add(new AppCollectionMap());
            modelBuilder.Configurations.Add(new CalculateParamMap());
            modelBuilder.Configurations.Add(new CalculateValueMap());
            modelBuilder.Configurations.Add(new ConstantParamMap());
            modelBuilder.Configurations.Add(new MessureParamMap());
            modelBuilder.Configurations.Add(new MessureValueMap());
            modelBuilder.Configurations.Add(new ProjectPartMap());
            modelBuilder.Configurations.Add(new RemarkMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new SysUserMap());
            modelBuilder.Configurations.Add(new TaskAppratuMap());
            modelBuilder.Configurations.Add(new TaskTypeMap());
        }
    }
}

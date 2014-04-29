namespace DamService2.Migrations
{
    using Hammergo.Data;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Hammergo.Data.DamWCFContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Hammergo.Data.DamWCFContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var type = new ApparatusType();
            type.Id = Guid.NewGuid();
            type.TypeName = "第一种类型";

            context.ApparatusTypes.Add(type);

            type = new ApparatusType();
            type.Id = Guid.NewGuid();
            type.TypeName = "第二种类型";
            context.ApparatusTypes.Add(type);
        }
    }
}

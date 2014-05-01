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

            var type1 = new ApparatusType();
            type1.Id = Guid.NewGuid();
            type1.TypeName = "��һ������";

           

            var type2 = new ApparatusType();
            type2.Id = Guid.NewGuid();
            type2.TypeName = "�ڶ�������";

            context.ApparatusTypes.AddOrUpdate(type1, type2);



            //�������
            var app1 = new App();
            app1.Id = Guid.NewGuid();
            app1.AppName = "��һ֧����";
            app1.CalculateName = "FirstApp";
            app1.AppTypeID = type1.Id;

            var app2 = new App();
            app2.Id = Guid.NewGuid();
            app2.AppName = "�ڶ�֧����";
            app2.CalculateName = "SecondApp";
            app2.AppTypeID = type2.Id;
 
            context.Apps.AddOrUpdate(app1, app2);
            //��ӹ��̲�λ
            var root = new ProjectPart()
            {
                Id = Guid.NewGuid(),
                PartName = "root",
                ParentPart = null,

            };

            var p1 = new ProjectPart()
            {
                Id = Guid.NewGuid(),
                PartName = "p1",
                ParentPart = root.Id,
            };

            var p2 = new ProjectPart()
            {
                Id = Guid.NewGuid(),
                PartName = "p2",
                ParentPart = root.Id,
            };


            context.ProjectParts.AddOrUpdate(root, p1, p2);

        }
    }
}

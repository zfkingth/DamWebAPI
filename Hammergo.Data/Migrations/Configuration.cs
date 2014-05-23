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


            //�������
            var app1 = new App();
            app1.Id = Guid.NewGuid();
            app1.AppName = "��һ֧����";
            app1.CalculateName = "FirstApp";
            app1.AppTypeID = type1.Id;
            app1.ProjectPartID = p1.Id;

            var app2 = new App();
            app2.Id = Guid.NewGuid();
            app2.AppName = "�ڶ�֧����";
            app2.CalculateName = "SecondApp";
            app2.AppTypeID = type2.Id;
            app2.ProjectPartID = p2.Id;


            var app3 = new App();
            app3.Id = Guid.NewGuid();
            app3.AppName = "����֧����";
            app3.CalculateName = "ThirdApp";
            app3.AppTypeID = type2.Id;
            app3.ProjectPartID = p2.Id;

            context.Apps.AddOrUpdate(app1, app2, app3);


         
        
            var remark3 = new Remark()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now,
                RemarkText = "remark",
                AppId = app2.Id,
            };

            context.Remarks.AddOrUpdate( remark3);

            var conParam1 = new ConstantParam()
            {
                Id = Guid.NewGuid(),
                AppId = app1.Id,
                ParamName = "c1",
                ParamSymbol = "c1",
                PrecisionNum = 2,
                UnitSymbol = "no",
                Val = 1,
                Order = 1,
                Description = "no description",


            };

            var mesParam1 = new MessureParam()
            {
                Id = Guid.NewGuid(),
                AppId = app1.Id,
                ParamName = "m1",
                ParamSymbol = "m1",
                PrecisionNum = 2,
                UnitSymbol = "no",
                Order = 1,
                Description = "no description",


            };


            var calcParam1 = new CalculateParam()
            {
                Id = Guid.NewGuid(),
                AppId = app1.Id,
                ParamName = "cal1",
                ParamSymbol = "cal1",
                PrecisionNum = 2,
                UnitSymbol = "no",
                Order = 1,
                Description = "no description",


            };

            context.AppParams.AddOrUpdate(conParam1, mesParam1, calcParam1);
            //��ӵ�һ֧����������
            DateTimeOffset date = DateTimeOffset.Now;
            int count = 20;
            for (int i = 0; i < count; i++)
            {
                MessureValue mv = new MessureValue()
                {
                    Id = Guid.NewGuid(),
                    ParamId = mesParam1.Id,
                    Date = date.AddDays(-i),
                    Val = i
                };

                CalculateValue cv = new CalculateValue()
                {
                    Id = Guid.NewGuid(),
                    ParamId = calcParam1.Id,
                    Date = date.AddDays(-i),
                    Val = i
                };

                var remark = new Remark()
                {
                    Id = Guid.NewGuid(),
                    Date = date.AddDays(-i),
                    RemarkText = "remark"+i,
                    AppId = app1.Id,
                };


                context.MessureValues.AddOrUpdate(mv);
                context.CalculateValues.AddOrUpdate(cv);
                context.Remarks.AddOrUpdate(remark);
            }

            //��ӵ���֧�����Ĳ���
            var conParam1_third = new ConstantParam()
            {
                Id = Guid.NewGuid(),
                AppId = app3.Id,
                ParamName = "c1",
                ParamSymbol = "c1",
                PrecisionNum = 2,
                UnitSymbol = "no",
                Val = 1,
                Order = 1,
                Description = "no description",


            };

            var mesParam1_third = new MessureParam()
            {
                Id = Guid.NewGuid(),
                AppId = app3.Id,
                ParamName = "m1",
                ParamSymbol = "m1",
                PrecisionNum = 2,
                UnitSymbol = "no",
                Order = 1,
                Description = "no description",


            };


            var calcParam1_third = new CalculateParam()
            {
                Id = Guid.NewGuid(),
                AppId = app3.Id,
                ParamName = "cal1",
                ParamSymbol = "cal1",
                PrecisionNum = 2,
                UnitSymbol = "no",
                Order = 1,
                Description = "no description",


            };

            context.AppParams.AddOrUpdate(conParam1_third, mesParam1_third, calcParam1_third);

            ///first app fomula

            var formula = new Formula()
            {
                Id = Guid.NewGuid(),
                ParamId = calcParam1.Id,
                StartDate = DateTimeOffset.MinValue,
                EndDate = DateTimeOffset.MaxValue,
                CalculateOrder = 1,
                FormulaExpression = "c1+m1"
            };

            context.Formulae.AddOrUpdate(formula);


            var formula_third = new Formula()
            {
                Id = Guid.NewGuid(),
                ParamId = calcParam1_third.Id,
                StartDate = DateTimeOffset.MinValue,
                EndDate = DateTimeOffset.MaxValue,
                CalculateOrder = 1,
                FormulaExpression = "c1+m1+FirstApp.cal1"
            };

            context.Formulae.AddOrUpdate(formula_third);

        }
    }
}

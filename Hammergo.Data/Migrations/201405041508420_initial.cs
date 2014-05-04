namespace DamService2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApparatusType",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TypeName = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.App",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AppName = c.String(nullable: false, maxLength: 20),
                        CalculateName = c.String(nullable: false, maxLength: 20),
                        ProjectPartID = c.Guid(),
                        AppTypeID = c.Guid(),
                        X = c.String(maxLength: 50),
                        Y = c.String(maxLength: 50),
                        Z = c.String(maxLength: 50),
                        BuriedTime = c.DateTimeOffset(precision: 7),
                        OtherInfo = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApparatusType", t => t.AppTypeID)
                .ForeignKey("dbo.ProjectPart", t => t.ProjectPartID)
                .Index(t => t.ProjectPartID)
                .Index(t => t.AppTypeID);
            
            CreateTable(
                "dbo.AppParam",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AppId = c.Guid(nullable: false),
                        ParamName = c.String(nullable: false, maxLength: 20),
                        ParamSymbol = c.String(nullable: false, maxLength: 10),
                        UnitSymbol = c.String(maxLength: 10),
                        PrecisionNum = c.Byte(nullable: false),
                        Order = c.Byte(nullable: false),
                        Description = c.String(),
                        CalcOrder = c.Byte(),
                        Val = c.Double(),
                        TypeNum = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.App", t => t.AppId, cascadeDelete: true)
                .Index(t => t.AppId);
            
            CreateTable(
                "dbo.CalculateValue",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ParamId = c.Guid(nullable: false),
                        Date = c.DateTimeOffset(nullable: false, precision: 7),
                        Val = c.Double(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppParam", t => t.ParamId, cascadeDelete: true)
                .Index(t => t.ParamId);
            
            CreateTable(
                "dbo.Formula",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ParamId = c.Guid(nullable: false),
                        StartDate = c.DateTimeOffset(nullable: false, precision: 7),
                        EndDate = c.DateTimeOffset(nullable: false, precision: 7),
                        FormulaExpression = c.String(nullable: false, maxLength: 200),
                        CalculateOrder = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppParam", t => t.ParamId, cascadeDelete: true)
                .Index(t => t.ParamId);
            
            CreateTable(
                "dbo.MessureValue",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ParamId = c.Guid(nullable: false),
                        Date = c.DateTimeOffset(nullable: false, precision: 7),
                        Val = c.Double(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppParam", t => t.ParamId, cascadeDelete: true)
                .Index(t => t.ParamId);
            
            CreateTable(
                "dbo.ProjectPart",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PartName = c.String(nullable: false, maxLength: 50),
                        ParentPart = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Remark",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AppId = c.Guid(nullable: false),
                        Date = c.DateTimeOffset(nullable: false, precision: 7),
                        RemarkText = c.String(maxLength: 80),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.App", t => t.AppId, cascadeDelete: true)
                .Index(t => t.AppId);
            
            CreateTable(
                "dbo.TaskApp",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AppCollectionID = c.Guid(nullable: false),
                        AppId = c.Guid(nullable: false),
                        Order = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.App", t => t.AppId, cascadeDelete: true)
                .ForeignKey("dbo.AppCollection", t => t.AppCollectionID, cascadeDelete: true)
                .Index(t => t.AppCollectionID)
                .Index(t => t.AppId);
            
            CreateTable(
                "dbo.AppCollection",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TaskTypeID = c.Int(nullable: false),
                        CollectionName = c.String(nullable: false, maxLength: 30),
                        Description = c.String(maxLength: 50),
                        Order = c.Int(),
                        ParentCollection = c.Guid(),
                        SUM = c.Double(),
                        MAX = c.Double(),
                        MIN = c.Double(),
                        AVG = c.Double(),
                        CNT = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TaskType", t => t.TaskTypeID, cascadeDelete: true)
                .Index(t => t.TaskTypeID);
            
            CreateTable(
                "dbo.TaskType",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        TypeName = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TaskApp", "AppCollectionID", "dbo.AppCollection");
            DropForeignKey("dbo.AppCollection", "TaskTypeID", "dbo.TaskType");
            DropForeignKey("dbo.TaskApp", "AppId", "dbo.App");
            DropForeignKey("dbo.Remark", "AppId", "dbo.App");
            DropForeignKey("dbo.App", "ProjectPartID", "dbo.ProjectPart");
            DropForeignKey("dbo.MessureValue", "ParamId", "dbo.AppParam");
            DropForeignKey("dbo.Formula", "ParamId", "dbo.AppParam");
            DropForeignKey("dbo.CalculateValue", "ParamId", "dbo.AppParam");
            DropForeignKey("dbo.AppParam", "AppId", "dbo.App");
            DropForeignKey("dbo.App", "AppTypeID", "dbo.ApparatusType");
            DropIndex("dbo.AppCollection", new[] { "TaskTypeID" });
            DropIndex("dbo.TaskApp", new[] { "AppId" });
            DropIndex("dbo.TaskApp", new[] { "AppCollectionID" });
            DropIndex("dbo.Remark", new[] { "AppId" });
            DropIndex("dbo.MessureValue", new[] { "ParamId" });
            DropIndex("dbo.Formula", new[] { "ParamId" });
            DropIndex("dbo.CalculateValue", new[] { "ParamId" });
            DropIndex("dbo.AppParam", new[] { "AppId" });
            DropIndex("dbo.App", new[] { "AppTypeID" });
            DropIndex("dbo.App", new[] { "ProjectPartID" });
            DropTable("dbo.TaskType");
            DropTable("dbo.AppCollection");
            DropTable("dbo.TaskApp");
            DropTable("dbo.Remark");
            DropTable("dbo.ProjectPart");
            DropTable("dbo.MessureValue");
            DropTable("dbo.Formula");
            DropTable("dbo.CalculateValue");
            DropTable("dbo.AppParam");
            DropTable("dbo.App");
            DropTable("dbo.ApparatusType");
        }
    }
}

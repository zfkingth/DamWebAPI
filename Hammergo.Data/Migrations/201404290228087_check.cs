namespace DamService2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class check : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.App", "CalculateName", unique: true);
            CreateIndex("dbo.App", "AppName", unique: true);

            CreateIndex("dbo.ApparatusType", "TypeName", unique: true);

            CreateIndex("dbo.TaskType", "TypeName", unique: true);

            CreateIndex("dbo.AppParam", new string[] { "AppId", "ParamName" }, unique: true);

            CreateIndex("dbo.AppParam", new string[] { "AppId", "ParamSymbol" }, unique: true);
            CreateIndex("dbo.CalculateValue", new string[] { "ParamId", "Date" }, unique: true);
            CreateIndex("dbo.MessureValue", new string[] { "ParamId", "Date" }, unique: true);
            CreateIndex("dbo.Remark", new string[] { "AppId", "Date" }, unique: true);
            CreateIndex("dbo.TaskApp", new string[] { "AppCollectionID", "AppId" }, unique: true);
        }
        
        public override void Down()
        {
        }
    }
}

namespace DamService2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateToOffset : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.App", "BuriedTime", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.CalculateValue", "Date", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Formula", "StartDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Formula", "EndDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.MessureValue", "Date", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Remark", "Date", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Remark", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MessureValue", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Formula", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Formula", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CalculateValue", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.App", "BuriedTime", c => c.DateTime());
        }
    }
}

namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBukkenNewFloorAreaProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bukkens", "FloorArea1", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Bukkens", "FloorTubo", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Bukkens", "FloorAreaMeasuringMethod", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bukkens", "FloorAreaMeasuringMethod");
            DropColumn("dbo.Bukkens", "FloorTubo");
            DropColumn("dbo.Bukkens", "FloorArea1");
        }
    }
}

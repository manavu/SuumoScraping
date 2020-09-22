namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBukkenNewPriceProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bukkens", "Price1", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Bukkens", "Price2", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bukkens", "Price2");
            DropColumn("dbo.Bukkens", "Price1");
        }
    }
}

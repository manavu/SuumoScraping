namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBukenNewAccess2Property : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bukkens", "Access2", c => c.String(maxLength: 100, storeType: "nvarchar"));
            AddColumn("dbo.Bukkens", "Access3", c => c.String(maxLength: 100, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bukkens", "Access3");
            DropColumn("dbo.Bukkens", "Access2");
        }
    }
}

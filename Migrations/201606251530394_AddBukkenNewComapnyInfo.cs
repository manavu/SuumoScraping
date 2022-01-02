namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBukkenNewComapnyInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bukkens", "Company_Name", c => c.String(maxLength: 100, storeType: "nvarchar"));
            AddColumn("dbo.Bukkens", "Company_Address", c => c.String(maxLength: 100, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bukkens", "Company_Address");
            DropColumn("dbo.Bukkens", "Company_Name");
        }
    }
}

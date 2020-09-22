namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeBukkenEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bukkens", "UseDistrict", c => c.String(maxLength: 20));
            AddColumn("dbo.Bukkens", "Structure", c => c.String(maxLength: 20));
            AddColumn("dbo.Bukkens", "RightsStyle", c => c.String(maxLength: 20));
            AddColumn("dbo.Bukkens", "MoveInTime", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bukkens", "MoveInTime");
            DropColumn("dbo.Bukkens", "RightsStyle");
            DropColumn("dbo.Bukkens", "Structure");
            DropColumn("dbo.Bukkens", "UseDistrict");
        }
    }
}

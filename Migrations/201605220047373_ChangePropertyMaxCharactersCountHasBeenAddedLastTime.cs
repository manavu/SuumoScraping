namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePropertyMaxCharactersCountHasBeenAddedLastTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bukkens", "UseDistrict", c => c.String(maxLength: 50));
            AlterColumn("dbo.Bukkens", "Structure", c => c.String(maxLength: 50));
            AlterColumn("dbo.Bukkens", "RightsStyle", c => c.String(maxLength: 100));
            AlterColumn("dbo.Bukkens", "MoveInTime", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bukkens", "MoveInTime", c => c.String(maxLength: 20));
            AlterColumn("dbo.Bukkens", "RightsStyle", c => c.String(maxLength: 20));
            AlterColumn("dbo.Bukkens", "Structure", c => c.String(maxLength: 20));
            AlterColumn("dbo.Bukkens", "UseDistrict", c => c.String(maxLength: 20));
        }
    }
}

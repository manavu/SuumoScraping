namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeRightsStylePropertyMaxCharactersCount : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bukkens", "RightsStyle", c => c.String(maxLength: 300));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bukkens", "RightsStyle", c => c.String(maxLength: 100));
        }
    }
}

namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingRestrictionPropertyToBukkenEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bukkens", "Restriction", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bukkens", "Restriction");
        }
    }
}

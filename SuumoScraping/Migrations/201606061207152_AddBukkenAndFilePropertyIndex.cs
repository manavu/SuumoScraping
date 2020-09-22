namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBukkenAndFilePropertyIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Bukkens", new[] { "ImportedDate", "DetailUrl" }, name: "IX_Bukkens_DetailUrl_ImportedDate");
            CreateIndex("dbo.Files", "Url", name: "IX_Files_Url");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Files", "IX_Files_Url");
            DropIndex("dbo.Bukkens", "IX_Bukkens_DetailUrl_ImportedDate");
        }
    }
}

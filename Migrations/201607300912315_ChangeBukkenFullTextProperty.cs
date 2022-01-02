namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeBukkenFullTextProperty : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE Bukkens DROP INDEX IX_Bukkens_AddressBigram;");
            Sql("ALTER TABLE Bukkens DROP INDEX IX_Bukkens_AccessBigram;");

            CreateTable(
                "dbo.BukkenFulltexts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccessBigram = c.String(nullable: false, unicode: false),
                        AddressBigram = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Bukkens", "FullText_Id", c => c.Int());
            CreateIndex("dbo.Bukkens", "FullText_Id");
            AddForeignKey("dbo.Bukkens", "FullText_Id", "dbo.BukkenFulltexts", "Id");
            DropColumn("dbo.Bukkens", "AccessBigram");
            DropColumn("dbo.Bukkens", "AddressBigram");

            Sql("CREATE FULLTEXT INDEX IX_BukkenFulltexts_AccessBigram ON BukkenFulltexts(AccessBigram);");
            Sql("CREATE FULLTEXT INDEX IX_BukkenFulltexts_AddressBigram ON BukkenFulltexts(AddressBigram);");
        }

        public override void Down()
        {
            AddColumn("dbo.Bukkens", "AddressBigram", c => c.String(unicode: false));
            AddColumn("dbo.Bukkens", "AccessBigram", c => c.String(unicode: false));
            DropForeignKey("dbo.Bukkens", "FullText_Id", "dbo.BukkenFulltexts");
            DropIndex("dbo.Bukkens", new[] { "FullText_Id" });
            DropColumn("dbo.Bukkens", "FullText_Id");
            DropTable("dbo.BukkenFulltexts");

            Sql("CREATE FULLTEXT INDEX IX_Bukkens_AccessBigram ON Bukkens(AccessBigram);");
            Sql("CREATE FULLTEXT INDEX IX_Bukkens_AddressBigram ON Bukkens(AddressBigram);");
        }
    }
}

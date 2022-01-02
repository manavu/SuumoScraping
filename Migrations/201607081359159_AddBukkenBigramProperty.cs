namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBukkenBigramProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bukkens", "AccessBigram", c => c.String(unicode: false));
            AddColumn("dbo.Bukkens", "AddressBigram", c => c.String(unicode: false));

            Sql("CREATE FULLTEXT INDEX IX_Bukkens_AccessBigram ON Bukkens(AccessBigram);");
            Sql("CREATE FULLTEXT INDEX IX_Bukkens_AddressBigram ON Bukkens(AddressBigram);");

            // my.ini‚É‰º‹L‚ð’Ç‰Á
            // innodb_ft_min_token_size=1
        }

        public override void Down()
        {
            Sql("ALTER TABLE Bukkens DROP INDEX IX_Bukkens_AddressBigram;");
            Sql("ALTER TABLE Bukkens DROP INDEX IX_Bukkens_AccessBigram;");

            DropColumn("dbo.Bukkens", "AddressBigram");
            DropColumn("dbo.Bukkens", "AccessBigram");
        }
    }
}

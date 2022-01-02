namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BukkenFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false, maxLength: 50),
                        Bukken_Id = c.Int(),
                        File_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bukkens", t => t.Bukken_Id)
                .ForeignKey("dbo.Files", t => t.File_Id)
                .Index(t => t.Bukken_Id)
                .Index(t => t.File_Id);
            
            CreateTable(
                "dbo.Bukkens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Price = c.String(nullable: false, maxLength: 50),
                        DetailUrl = c.String(nullable: false, maxLength: 200),
                        Address = c.String(nullable: false, maxLength: 100),
                        Access = c.String(nullable: false, maxLength: 100),
                        Layout = c.String(nullable: false, maxLength: 100),
                        FloorArea = c.String(nullable: false, maxLength: 100),
                        Balcony = c.String(nullable: false, maxLength: 50),
                        BuiltYears = c.String(nullable: false, maxLength: 20),
                        ImportedDate = c.DateTime(nullable: false, storeType: "date"),
                        ManagementFee = c.String(maxLength: 50),
                        RepairingDeposit = c.String(maxLength: 50),
                        RepairingFund = c.String(maxLength: 50),
                        Floor = c.String(maxLength: 20),
                        Direction = c.String(maxLength: 5),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileData = c.Binary(nullable: false),
                        ContentType = c.String(nullable: false, maxLength: 50),
                        Hash = c.String(nullable: false, maxLength: 64, fixedLength: true),
                        Url = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BukkenFiles", "File_Id", "dbo.Files");
            DropForeignKey("dbo.BukkenFiles", "Bukken_Id", "dbo.Bukkens");
            DropIndex("dbo.BukkenFiles", new[] { "File_Id" });
            DropIndex("dbo.BukkenFiles", new[] { "Bukken_Id" });
            DropTable("dbo.Files");
            DropTable("dbo.Bukkens");
            DropTable("dbo.BukkenFiles");
        }
    }
}

namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeNewEf6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BukkenFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
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
                        Title = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Price = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        Price1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price2 = c.Decimal(precision: 18, scale: 2),
                        DetailUrl = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Address = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Access = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Access2 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Access3 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Layout = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        FloorArea = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        FloorArea1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FloorTubo = c.Decimal(precision: 18, scale: 2),
                        FloorAreaMeasuringMethod = c.String(unicode: false),
                        Balcony = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        BuiltYears = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                        ImportedDate = c.DateTime(nullable: false, storeType: "date"),
                        ManagementFee = c.String(maxLength: 50, storeType: "nvarchar"),
                        RepairingDeposit = c.String(maxLength: 50, storeType: "nvarchar"),
                        RepairingFund = c.String(maxLength: 50, storeType: "nvarchar"),
                        Floor = c.String(maxLength: 20, storeType: "nvarchar"),
                        Direction = c.String(maxLength: 5, storeType: "nvarchar"),
                        UseDistrict = c.String(maxLength: 50, storeType: "nvarchar"),
                        Structure = c.String(maxLength: 50, storeType: "nvarchar"),
                        RightsStyle = c.String(maxLength: 300, storeType: "nvarchar"),
                        MoveInTime = c.String(maxLength: 50, storeType: "nvarchar"),
                        Restriction = c.String(maxLength: 500, storeType: "nvarchar"),
                        Company_Name = c.String(maxLength: 100, storeType: "nvarchar"),
                        Company_Address = c.String(maxLength: 100, storeType: "nvarchar"),
                        Company_TakkenLicense = c.String(maxLength: 80, storeType: "nvarchar"),
                        Company_TransactionAspect = c.String(maxLength: 80, storeType: "nvarchar"),
                        FullText_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BukkenFulltexts", t => t.FullText_Id)
                .Index(t => new { t.ImportedDate, t.DetailUrl }, name: "IX_Bukkens_DetailUrl_ImportedDate")
                .Index(t => t.FullText_Id);
            
            CreateTable(
                "dbo.BukkenFulltexts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccessBigram = c.String(nullable: false, unicode: false),
                        AddressBigram = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileData = c.Binary(nullable: false),
                        ContentType = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        Hash = c.String(nullable: false, maxLength: 64, fixedLength: true, storeType: "nchar"),
                        Url = c.String(nullable: false, maxLength: 500, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Url, name: "IX_Files_Url");
            
            CreateTable(
                "dbo.NewBukkens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        DetailUrl = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Address = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Access1 = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Access2 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Access3 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Layout = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        FloorArea = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        FloorArea1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FloorTubo = c.Decimal(precision: 18, scale: 2),
                        FloorAreaMeasuringMethod = c.String(unicode: false),
                        Balcony = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        BuiltYears = c.DateTime(precision: 0),
                        ManagementFee = c.String(maxLength: 50, storeType: "nvarchar"),
                        RepairingDeposit = c.String(maxLength: 50, storeType: "nvarchar"),
                        RepairingFund = c.String(maxLength: 50, storeType: "nvarchar"),
                        Floor = c.String(maxLength: 20, storeType: "nvarchar"),
                        Direction = c.String(maxLength: 5, storeType: "nvarchar"),
                        UseDistrict = c.String(maxLength: 50, storeType: "nvarchar"),
                        Structure = c.String(maxLength: 50, storeType: "nvarchar"),
                        RightsStyle = c.String(maxLength: 300, storeType: "nvarchar"),
                        MoveInTime = c.String(maxLength: 50, storeType: "nvarchar"),
                        Restriction = c.String(maxLength: 500, storeType: "nvarchar"),
                        Company_Name = c.String(maxLength: 100, storeType: "nvarchar"),
                        Company_Address = c.String(maxLength: 100, storeType: "nvarchar"),
                        Company_TakkenLicense = c.String(maxLength: 80, storeType: "nvarchar"),
                        Company_TransactionAspect = c.String(maxLength: 80, storeType: "nvarchar"),
                        ImportCount = c.Int(nullable: false),
                        ImportedAt = c.DateTime(nullable: false, precision: 0),
                        CreatedAt = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.DetailUrl, unique: true, name: "IX_Bukkens_DetailUrl");
            
            CreateTable(
                "dbo.NewBukkenFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        File_Id = c.Int(),
                        NewBukken_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.File_Id)
                .ForeignKey("dbo.NewBukkens", t => t.NewBukken_Id)
                .Index(t => t.File_Id)
                .Index(t => t.NewBukken_Id);
            
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChangedAt = c.DateTime(nullable: false, precision: 0),
                        Min = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Max = c.Decimal(precision: 18, scale: 2),
                        Text = c.String(unicode: false),
                        NewBukken_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NewBukkens", t => t.NewBukken_Id)
                .Index(t => t.NewBukken_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prices", "NewBukken_Id", "dbo.NewBukkens");
            DropForeignKey("dbo.NewBukkenFiles", "NewBukken_Id", "dbo.NewBukkens");
            DropForeignKey("dbo.NewBukkenFiles", "File_Id", "dbo.Files");
            DropForeignKey("dbo.BukkenFiles", "File_Id", "dbo.Files");
            DropForeignKey("dbo.Bukkens", "FullText_Id", "dbo.BukkenFulltexts");
            DropForeignKey("dbo.BukkenFiles", "Bukken_Id", "dbo.Bukkens");
            DropIndex("dbo.Prices", new[] { "NewBukken_Id" });
            DropIndex("dbo.NewBukkenFiles", new[] { "NewBukken_Id" });
            DropIndex("dbo.NewBukkenFiles", new[] { "File_Id" });
            DropIndex("dbo.NewBukkens", "IX_Bukkens_DetailUrl");
            DropIndex("dbo.Files", "IX_Files_Url");
            DropIndex("dbo.Bukkens", new[] { "FullText_Id" });
            DropIndex("dbo.Bukkens", "IX_Bukkens_DetailUrl_ImportedDate");
            DropIndex("dbo.BukkenFiles", new[] { "File_Id" });
            DropIndex("dbo.BukkenFiles", new[] { "Bukken_Id" });
            DropTable("dbo.Prices");
            DropTable("dbo.NewBukkenFiles");
            DropTable("dbo.NewBukkens");
            DropTable("dbo.Files");
            DropTable("dbo.BukkenFulltexts");
            DropTable("dbo.Bukkens");
            DropTable("dbo.BukkenFiles");
        }
    }
}

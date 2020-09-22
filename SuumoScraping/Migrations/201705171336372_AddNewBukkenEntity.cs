namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewBukkenEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "NewBukkens",
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
                "NewBukkenFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        File_Id = c.Int(),
                        NewBukken_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Files", t => t.File_Id)
                .ForeignKey("NewBukkens", t => t.NewBukken_Id)
                .Index(t => t.File_Id)
                .Index(t => t.NewBukken_Id);
            
            CreateTable(
                "Prices",
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
                .ForeignKey("NewBukkens", t => t.NewBukken_Id)
                .Index(t => t.NewBukken_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Prices", "NewBukken_Id", "NewBukkens");
            DropForeignKey("NewBukkenFiles", "NewBukken_Id", "NewBukkens");
            DropForeignKey("NewBukkenFiles", "File_Id", "Files");
            DropIndex("Prices", new[] { "NewBukken_Id" });
            DropIndex("NewBukkenFiles", new[] { "NewBukken_Id" });
            DropIndex("NewBukkenFiles", new[] { "File_Id" });
            DropIndex("NewBukkens", "IX_Bukkens_DetailUrl");
            DropTable("Prices");
            DropTable("NewBukkenFiles");
            DropTable("NewBukkens");
        }
    }
}

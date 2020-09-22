namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBukenNewComapnyInfo2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bukkens", "Company_TakkenLicense", c => c.String(maxLength: 80, storeType: "nvarchar"));
            AddColumn("dbo.Bukkens", "Company_TransactionAspect", c => c.String(maxLength: 20, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bukkens", "Company_TransactionAspect");
            DropColumn("dbo.Bukkens", "Company_TakkenLicense");
        }
    }
}

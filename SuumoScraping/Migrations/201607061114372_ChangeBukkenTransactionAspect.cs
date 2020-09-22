namespace SuumoScraping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeBukkenTransactionAspect : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bukkens", "Company_TransactionAspect", c => c.String(maxLength: 80, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bukkens", "Company_TransactionAspect", c => c.String(maxLength: 20, storeType: "nvarchar"));
        }
    }
}

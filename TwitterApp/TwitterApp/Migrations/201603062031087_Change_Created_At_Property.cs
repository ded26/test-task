namespace TwitterApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Created_At_Property : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tweets", "CreatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tweets", "CreatedAt", c => c.String());
        }
    }
}

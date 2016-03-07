namespace TwitterApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tweets",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserName = c.String(),
                        ProfileImage = c.String(),
                        Text = c.String(),
                        CreatedAt = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Tweets");
        }
    }
}

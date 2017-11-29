namespace Reporter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cls : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ErrorCommentConns",
                c => new
                    {
                        Batch = c.String(nullable: false, maxLength: 4000),
                        Task = c.String(nullable: false, maxLength: 4000),
                        ErrorMessage = c.String(nullable: false, maxLength: 4000),
                        Comments = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => new { t.Batch, t.Task, t.ErrorMessage });
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        EmailAddress = c.String(nullable: false, maxLength: 4000),
                        Name = c.String(maxLength: 4000),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EmailAddress);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.People");
            DropTable("dbo.ErrorCommentConns");
        }
    }
}

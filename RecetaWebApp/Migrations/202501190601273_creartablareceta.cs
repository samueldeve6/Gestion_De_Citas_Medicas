namespace RecetaWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creartablareceta : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Recetas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        PacienteId = c.Int(nullable: false),
                        Estado = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Recetas");
        }
    }
}

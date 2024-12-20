using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    public partial class PopulaCategorias : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Categorias(Nome, ImageUrl) VALUES('Bebida', 'bebidas.jpg')");
            mb.Sql("Insert into Categorias(Nome, ImageUrl) VALUES('Lanches', 'lanches.jpg')");
            mb.Sql("Insert into Categorias(Nome, ImageUrl) VALUES('Sobremeas', 'sobremesas.jpg')");

        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Categorias");
        }
    }
}

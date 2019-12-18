using Microsoft.EntityFrameworkCore.Migrations;

namespace Cirice.Migrations
{
    public partial class compositionname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Compositions_CompositionId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CompositionId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CompositionId",
                table: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Compositions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Compositions");

            migrationBuilder.AddColumn<long>(
                name: "CompositionId",
                table: "Tags",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CompositionId",
                table: "Tags",
                column: "CompositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Compositions_CompositionId",
                table: "Tags",
                column: "CompositionId",
                principalTable: "Compositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

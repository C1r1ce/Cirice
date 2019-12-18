using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cirice.Migrations
{
    public partial class create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
//            migrationBuilder.CreateTable(
//                name: "Chapters",
//                columns: table => new
//                {
//                    Id = table.Column<long>(nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    CompositionId = table.Column<long>(nullable: false),
//                    Number = table.Column<int>(nullable: false),
//                    Text = table.Column<string>(nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Chapters", x => x.Id);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "Comments",
//                columns: table => new
//                {
//                    Id = table.Column<long>(nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    UserId = table.Column<long>(nullable: false),
//                    CompositionId = table.Column<long>(nullable: false),
//                    Text = table.Column<string>(nullable: true),
//                    DateTime = table.Column<DateTime>(nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Comments", x => x.Id);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "Compositions",
//                columns: table => new
//                {
//                    Id = table.Column<long>(nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    UserId = table.Column<long>(nullable: false),
//                    GenreId = table.Column<byte>(nullable: false),
//                    FirstPublication = table.Column<DateTime>(nullable: false),
//                    LastPublication = table.Column<DateTime>(nullable: false),
//                    Annotation = table.Column<string>(nullable: true),
//                    ImgSource = table.Column<string>(nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Compositions", x => x.Id);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "CompositionTags",
//                columns: table => new
//                {
//                    Id = table.Column<long>(nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    CompositionId = table.Column<long>(nullable: false),
//                    TagId = table.Column<int>(nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_CompositionTags", x => x.Id);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "Genres",
//                columns: table => new
//                {
//                    Id = table.Column<byte>(nullable: false),
//                    GenreString = table.Column<string>(nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Genres", x => x.Id);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "Likes",
//                columns: table => new
//                {
//                    Id = table.Column<long>(nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    UserId = table.Column<long>(nullable: false),
//                    CompositionId = table.Column<long>(nullable: false),
//                    DateTime = table.Column<DateTime>(nullable: false),
//                    ChapterId = table.Column<long>(nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Likes", x => x.Id);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "Ratings",
//                columns: table => new
//                {
//                    Id = table.Column<long>(nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    CompositionId = table.Column<long>(nullable: false),
//                    UserId = table.Column<long>(nullable: false),
//                    Mark = table.Column<byte>(nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Ratings", x => x.Id);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "Users",
//                columns: table => new
//                {
//                    Id = table.Column<long>(nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    Email = table.Column<string>(nullable: true),
//                    EmailVerified = table.Column<bool>(nullable: false),
//                    Name = table.Column<string>(nullable: true),
//                    RoleId = table.Column<byte>(nullable: false),
//                    FirstLogin = table.Column<DateTime>(nullable: false),
//                    LastLogin = table.Column<DateTime>(nullable: false),
//                    ImgSource = table.Column<string>(nullable: true),
//                    About = table.Column<string>(nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Users", x => x.Id);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "Tags",
//                columns: table => new
//                {
//                    Id = table.Column<int>(nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    TagString = table.Column<string>(nullable: true),
//                    CompositionId = table.Column<long>(nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Tags", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Tags_Compositions_CompositionId",
//                        column: x => x.CompositionId,
//                        principalTable: "Compositions",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Restrict);
//                });
//
//            migrationBuilder.CreateIndex(
//                name: "IX_Tags_CompositionId",
//                table: "Tags",
//                column: "CompositionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "CompositionTags");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Compositions");
        }
    }
}

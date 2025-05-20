using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreeJournalApi.Migrations
{
    /// <inheritdoc />
    public partial class SetCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreeNodes_TreeNodes_ParentId",
                table: "TreeNodes");

            migrationBuilder.AddForeignKey(
                name: "FK_TreeNodes_TreeNodes_ParentId",
                table: "TreeNodes",
                column: "ParentId",
                principalTable: "TreeNodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreeNodes_TreeNodes_ParentId",
                table: "TreeNodes");

            migrationBuilder.AddForeignKey(
                name: "FK_TreeNodes_TreeNodes_ParentId",
                table: "TreeNodes",
                column: "ParentId",
                principalTable: "TreeNodes",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocalCRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedInteractionLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InteractionLinks_Interactions_InteractionId1",
                table: "InteractionLinks");

            migrationBuilder.DropIndex(
                name: "IX_InteractionLinks_InteractionId1",
                table: "InteractionLinks");

            migrationBuilder.DropColumn(
                name: "InteractionId1",
                table: "InteractionLinks");

            migrationBuilder.AlterColumn<int>(
                name: "InteractionId",
                table: "InteractionLinks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddForeignKey(
                name: "FK_InteractionLinks_Interactions_InteractionId",
                table: "InteractionLinks",
                column: "InteractionId",
                principalTable: "Interactions",
                principalColumn: "InteractionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InteractionLinks_Interactions_InteractionId",
                table: "InteractionLinks");

            migrationBuilder.AlterColumn<int>(
                name: "InteractionId",
                table: "InteractionLinks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "InteractionId1",
                table: "InteractionLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InteractionLinks_InteractionId1",
                table: "InteractionLinks",
                column: "InteractionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InteractionLinks_Interactions_InteractionId1",
                table: "InteractionLinks",
                column: "InteractionId1",
                principalTable: "Interactions",
                principalColumn: "InteractionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

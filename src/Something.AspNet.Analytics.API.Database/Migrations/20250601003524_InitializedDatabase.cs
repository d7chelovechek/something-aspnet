using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Something.AspNet.Analytics.API.Database.Migrations;

/// <inheritdoc />
public partial class InitializedDatabase : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "OutboxEvents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OutboxEvents", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SessionsUpdates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SessionsUpdates", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_SessionsUpdates_SessionId",
            table: "SessionsUpdates",
            column: "SessionId");

        migrationBuilder.CreateIndex(
            name: "IX_SessionsUpdates_UserId",
            table: "SessionsUpdates",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OutboxEvents");

        migrationBuilder.DropTable(
            name: "SessionsUpdates");
    }
}

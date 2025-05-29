using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Something.AspNet.Database.Migrations;

/// <inheritdoc />
public partial class AddedOutboxEventTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "OutboxEvents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Payload = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Sent = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OutboxEvents", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_OutboxEvents_Sent",
            table: "OutboxEvents",
            column: "Sent");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OutboxEvents");
    }
}

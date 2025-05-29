using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Something.AspNet.Database.Migrations;

/// <inheritdoc />
public partial class RenamedColumnsInSessionsTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Sessions");

        migrationBuilder.RenameColumn(
            name: "UpdatableTo",
            table: "Sessions",
            newName: "TokensUpdatedAt");

        migrationBuilder.RenameColumn(
            name: "ExpiresAt",
            table: "Sessions",
            newName: "SessionExpiresAt");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "TokensUpdatedAt",
            table: "Sessions",
            newName: "UpdatableTo");

        migrationBuilder.RenameColumn(
            name: "SessionExpiresAt",
            table: "Sessions",
            newName: "ExpiresAt");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CreatedAt",
            table: "Sessions",
            type: "datetimeoffset",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Something.AspNet.Database.Migrations;

/// <inheritdoc />
public partial class AddedExpiresAtColumnsToSessionsTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "AccessTokenExpiresAt",
            table: "Sessions",
            type: "datetimeoffset",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "RefreshTokenExpiresAt",
            table: "Sessions",
            type: "datetimeoffset",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AccessTokenExpiresAt",
            table: "Sessions");

        migrationBuilder.DropColumn(
            name: "RefreshTokenExpiresAt",
            table: "Sessions");
    }
}

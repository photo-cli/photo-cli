using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoCli.Migrations
{
    /// <inheritdoc />
    public partial class photo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateTaken = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReverseGeocodeFormatted = table.Column<string>(type: "TEXT", nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: true),
                    Month = table.Column<int>(type: "INTEGER", nullable: true),
                    Day = table.Column<int>(type: "INTEGER", nullable: true),
                    Hour = table.Column<int>(type: "INTEGER", nullable: true),
                    Minute = table.Column<int>(type: "INTEGER", nullable: true),
                    Seconds = table.Column<int>(type: "INTEGER", nullable: true),
                    Address1 = table.Column<string>(type: "TEXT", nullable: true),
                    Address2 = table.Column<string>(type: "TEXT", nullable: true),
                    Address3 = table.Column<string>(type: "TEXT", nullable: true),
                    Address4 = table.Column<string>(type: "TEXT", nullable: true),
                    Address5 = table.Column<string>(type: "TEXT", nullable: true),
                    Address6 = table.Column<string>(type: "TEXT", nullable: true),
                    Address7 = table.Column<string>(type: "TEXT", nullable: true),
                    Address8 = table.Column<string>(type: "TEXT", nullable: true),
                    Sha1Hash = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_DateTaken",
                table: "Photos",
                column: "DateTaken");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_ReverseGeocodeFormatted",
                table: "Photos",
                column: "ReverseGeocodeFormatted");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_Year",
                table: "Photos",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_Year_Month",
                table: "Photos",
                columns: new[] { "Year", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_Year_Month_Day",
                table: "Photos",
                columns: new[] { "Year", "Month", "Day" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Photos");
        }
    }
}

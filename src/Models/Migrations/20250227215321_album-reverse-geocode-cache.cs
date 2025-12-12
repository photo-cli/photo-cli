using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoCli.Migrations
{
	/// <inheritdoc />
	public partial class albumreversegeocodecache : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Photos",
				type: "INTEGER",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<DateTime>(
				name: "ModifiedAt",
				table: "Photos",
				type: "TEXT",
				nullable: true);

			migrationBuilder.CreateTable(
				name: "Albums",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false),
					Type = table.Column<byte>(type: "INTEGER", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
					Configuration = table.Column<string>(type: "TEXT", nullable: false),
					IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
					ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Albums", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "ReverseGeocodeCache",
				columns: table => new
				{
					Id = table.Column<long>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Provider = table.Column<byte>(type: "INTEGER", nullable: false),
					Latitude = table.Column<double>(type: "REAL", nullable: false),
					Longitude = table.Column<double>(type: "REAL", nullable: false),
					Response = table.Column<byte[]>(type: "BLOB", nullable: false),
					Precision = table.Column<byte>(type: "INTEGER", nullable: false),
					Language = table.Column<string>(type: "TEXT", nullable: true),
					CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ReverseGeocodeCache", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AlbumHistories",
				columns: table => new
				{
					Id = table.Column<long>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Configuration = table.Column<string>(type: "TEXT", nullable: false),
					SnapshotAt = table.Column<DateTime>(type: "TEXT", nullable: false),
					AlbumId = table.Column<int>(type: "INTEGER", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AlbumHistories", x => x.Id);
					table.ForeignKey(
						name: "FK_AlbumHistories_Albums_AlbumId",
						column: x => x.AlbumId,
						principalTable: "Albums",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "IX_AlbumHistories_AlbumId",
				table: "AlbumHistories",
				column: "AlbumId");

			migrationBuilder.CreateIndex(
				name: "IX_Albums_Name",
				table: "Albums",
				column: "Name");

			migrationBuilder.CreateIndex(
				name: "IX_ReverseGeocodeCache_Latitude_Longitude_Provider_Precision",
				table: "ReverseGeocodeCache",
				columns: new[] { "Latitude", "Longitude", "Provider", "Precision" });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AlbumHistories");

			migrationBuilder.DropTable(
				name: "ReverseGeocodeCache");

			migrationBuilder.DropTable(
				name: "Albums");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Photos");

			migrationBuilder.DropColumn(
				name: "ModifiedAt",
				table: "Photos");
		}
	}
}

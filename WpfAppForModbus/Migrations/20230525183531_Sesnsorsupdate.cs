using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WpfAppForModbus.Migrations
{
    /// <inheritdoc />
    public partial class Sesnsorsupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    SensorId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SensorName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.SensorId);
                });

            migrationBuilder.CreateTable(
                name: "SensorsData",
                columns: table => new
                {
                    RowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SensorId = table.Column<int>(type: "INTEGER", nullable: false),
                    SensorData = table.Column<string>(type: "TEXT", nullable: false),
                    RowDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorsData", x => x.RowId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "SensorsData");
        }
    }
}

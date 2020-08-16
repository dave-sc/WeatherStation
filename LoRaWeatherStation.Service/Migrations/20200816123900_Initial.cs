using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LoRaWeatherStation.Service.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Longitude = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SupportedValues = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forecasts",
                columns: table => new
                {
                    Time = table.Column<DateTime>(nullable: false),
                    LocationId = table.Column<long>(nullable: true),
                    LoadTime = table.Column<DateTime>(nullable: false),
                    Temperature = table.Column<double>(nullable: false),
                    TemperatureError = table.Column<double>(nullable: false),
                    WindSpeed = table.Column<double>(nullable: false),
                    WindSpeedError = table.Column<double>(nullable: false),
                    Pressure = table.Column<double>(nullable: false),
                    PressureError = table.Column<double>(nullable: false),
                    CloudCover = table.Column<double>(nullable: false),
                    Weather = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forecasts", x => x.Time);
                    table.ForeignKey(
                        name: "FK_Forecasts_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SensorId = table.Column<long>(nullable: true),
                    RecordTime = table.Column<DateTime>(nullable: false),
                    Temperature = table.Column<float>(nullable: false),
                    Humidity = table.Column<float>(nullable: false),
                    Pressure = table.Column<float>(nullable: false),
                    AirQualityIndex = table.Column<float>(nullable: false),
                    Luminance = table.Column<float>(nullable: false),
                    WindSpeed = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Records_Sensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Forecasts_LocationId",
                table: "Forecasts",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_SensorId",
                table: "Records",
                column: "SensorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DropTable(
                name: "Forecasts");

            migrationBuilder.DropTable(
                name: "Records");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Sensors");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LoRaWeatherStation.Service.Migrations
{
    public partial class AddPrecipitationAndWindDir : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Precipitation",
                table: "Forecasts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PrecipitationProbability",
                table: "Forecasts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WindDirection",
                table: "Forecasts",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Precipitation",
                table: "Forecasts");

            migrationBuilder.DropColumn(
                name: "PrecipitationProbability",
                table: "Forecasts");

            migrationBuilder.DropColumn(
                name: "WindDirection",
                table: "Forecasts");
        }
    }
}

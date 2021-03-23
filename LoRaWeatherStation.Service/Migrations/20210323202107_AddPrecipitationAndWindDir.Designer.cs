﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using LoRaWeatherStation.Service;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LoRaWeatherStation.Service.Migrations
{
    [DbContext(typeof(WeatherStationContext))]
    [Migration("20210323202107_AddPrecipitationAndWindDir")]
    partial class AddPrecipitationAndWindDir
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("LoRaWeatherStation.Service.ForecastData", b =>
                {
                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("CloudCover")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("LoadTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long?>("LocationId")
                        .HasColumnType("bigint");

                    b.Property<double>("Precipitation")
                        .HasColumnType("double precision");

                    b.Property<double>("PrecipitationProbability")
                        .HasColumnType("double precision");

                    b.Property<double>("Pressure")
                        .HasColumnType("double precision");

                    b.Property<double>("PressureError")
                        .HasColumnType("double precision");

                    b.Property<double>("Temperature")
                        .HasColumnType("double precision");

                    b.Property<double>("TemperatureError")
                        .HasColumnType("double precision");

                    b.Property<int>("Weather")
                        .HasColumnType("integer");

                    b.Property<double>("WindDirection")
                        .HasColumnType("double precision");

                    b.Property<double>("WindSpeed")
                        .HasColumnType("double precision");

                    b.Property<double>("WindSpeedError")
                        .HasColumnType("double precision");

                    b.HasKey("Time");

                    b.HasIndex("LocationId");

                    b.ToTable("Forecasts");
                });

            modelBuilder.Entity("LoRaWeatherStation.Service.ForecastLocation", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("LoRaWeatherStation.Service.Option", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Name");

                    b.ToTable("Config");
                });

            modelBuilder.Entity("LoRaWeatherStation.Service.Sensor", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("SupportedValues")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("LoRaWeatherStation.Service.SensorRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<float>("AirQualityIndex")
                        .HasColumnType("real");

                    b.Property<float>("Humidity")
                        .HasColumnType("real");

                    b.Property<float>("Luminance")
                        .HasColumnType("real");

                    b.Property<float>("Pressure")
                        .HasColumnType("real");

                    b.Property<DateTime>("RecordTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long?>("SensorId")
                        .HasColumnType("bigint");

                    b.Property<float>("Temperature")
                        .HasColumnType("real");

                    b.Property<float>("WindSpeed")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("SensorId");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("LoRaWeatherStation.Service.ForecastData", b =>
                {
                    b.HasOne("LoRaWeatherStation.Service.ForecastLocation", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");
                });

            modelBuilder.Entity("LoRaWeatherStation.Service.SensorRecord", b =>
                {
                    b.HasOne("LoRaWeatherStation.Service.Sensor", "Sensor")
                        .WithMany()
                        .HasForeignKey("SensorId");
                });
#pragma warning restore 612, 618
        }
    }
}

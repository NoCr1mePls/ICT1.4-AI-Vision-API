using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICT1._4_AI_Vision_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Litter",
                columns: table => new
                {
                    litter_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    litter_classification = table.Column<int>(type: "int", nullable: false),
                    confidence = table.Column<double>(type: "float", nullable: false),
                    location_latitude = table.Column<double>(type: "float", nullable: false),
                    location_longitude = table.Column<double>(type: "float", nullable: false),
                    detection_time = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Litter", x => x.litter_id);
                });

            migrationBuilder.CreateTable(
                name: "Weather",
                columns: table => new
                {
                    weather_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    temperature_celsius = table.Column<double>(type: "float", nullable: false),
                    humidity = table.Column<double>(type: "float", nullable: false),
                    conditions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weather", x => x.weather_id);
                    table.ForeignKey(
                        name: "FK_Weather_Litter_weather_id",
                        column: x => x.weather_id,
                        principalTable: "Litter",
                        principalColumn: "litter_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weather");

            migrationBuilder.DropTable(
                name: "Litter");
        }
    }
}

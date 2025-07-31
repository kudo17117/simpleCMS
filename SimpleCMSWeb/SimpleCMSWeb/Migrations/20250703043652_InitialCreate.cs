using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleCMSWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "patientusers",
                columns: table => new
                {
                    PatientUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GoogleAccountId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FacebookAccountId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOnlineAccountActivated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserProfile = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patientusers", x => x.PatientUserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patientusers");
        }
    }
}

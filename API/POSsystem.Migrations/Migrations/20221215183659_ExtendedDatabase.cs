using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POSsystem.Migrations.Migrations
{
    public partial class ExtendedDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_BranchWorkingDay_WorkingDaysId",
                table: "Branches");

            migrationBuilder.DropTable(
                name: "BranchWorkingDay");

            migrationBuilder.RenameColumn(
                name: "WorkingDaysId",
                table: "Branches",
                newName: "BranchWorkingDaysId");

            migrationBuilder.RenameIndex(
                name: "IX_Branches_WorkingDaysId",
                table: "Branches",
                newName: "IX_Branches_BranchWorkingDaysId");

            migrationBuilder.CreateTable(
                name: "BranchWorkingDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkingDay = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchWorkingDays", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_BranchWorkingDays_BranchWorkingDaysId",
                table: "Branches",
                column: "BranchWorkingDaysId",
                principalTable: "BranchWorkingDays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_BranchWorkingDays_BranchWorkingDaysId",
                table: "Branches");

            migrationBuilder.DropTable(
                name: "BranchWorkingDays");

            migrationBuilder.RenameColumn(
                name: "BranchWorkingDaysId",
                table: "Branches",
                newName: "WorkingDaysId");

            migrationBuilder.RenameIndex(
                name: "IX_Branches_BranchWorkingDaysId",
                table: "Branches",
                newName: "IX_Branches_WorkingDaysId");

            migrationBuilder.CreateTable(
                name: "BranchWorkingDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    WorkingDays = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchWorkingDay", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_BranchWorkingDay_WorkingDaysId",
                table: "Branches",
                column: "WorkingDaysId",
                principalTable: "BranchWorkingDay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

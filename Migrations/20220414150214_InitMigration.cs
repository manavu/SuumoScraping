using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuumoScraping.Migrations
{
    public partial class InitMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "bukkenfulltexts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccessBigram = table.Column<string>(type: "longtext", nullable: false),
                    AddressBigram = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bukkenfulltexts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileData = table.Column<byte[]>(type: "longblob", nullable: false),
                    ContentType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Hash = table.Column<string>(type: "nchar(64)", maxLength: 64, nullable: false),
                    Url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "newbukkens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    DetailUrl = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Access1 = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Access2 = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Access3 = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Layout = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    FloorArea = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    FloorArea1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FloorTubo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FloorAreaMeasuringMethod = table.Column<string>(type: "longtext", nullable: true),
                    Balcony = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    BuiltYears = table.Column<DateTime>(type: "datetime", nullable: true),
                    ManagementFee = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RepairingDeposit = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RepairingFund = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Floor = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Direction = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true),
                    UseDistrict = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Structure = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RightsStyle = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    MoveInTime = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Restriction = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Company_Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Company_Address = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Company_TakkenLicense = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true),
                    Company_TransactionAspect = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true),
                    ImportCount = table.Column<int>(type: "int", nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_newbukkens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "bukkens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Price1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DetailUrl = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Access = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Access2 = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Access3 = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Layout = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    FloorArea = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    FloorArea1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FloorTubo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FloorAreaMeasuringMethod = table.Column<string>(type: "longtext", nullable: true),
                    Balcony = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    BuiltYears = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ImportedDate = table.Column<DateTime>(type: "date", nullable: false),
                    ManagementFee = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RepairingDeposit = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RepairingFund = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Floor = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Direction = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true),
                    UseDistrict = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Structure = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RightsStyle = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    MoveInTime = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Restriction = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Company_Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Company_Address = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Company_TakkenLicense = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true),
                    Company_TransactionAspect = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true),
                    FullText_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bukkens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bukkens_bukkenfulltexts_FullText_Id",
                        column: x => x.FullText_Id,
                        principalTable: "bukkenfulltexts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "newbukkenfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    File_Id = table.Column<int>(type: "int", nullable: true),
                    NewBukken_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_newbukkenfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_newbukkenfiles_files_File_Id",
                        column: x => x.File_Id,
                        principalTable: "files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_newbukkenfiles_newbukkens_NewBukken_Id",
                        column: x => x.NewBukken_Id,
                        principalTable: "newbukkens",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "prices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChangedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Min = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Max = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Text = table.Column<string>(type: "longtext", nullable: true),
                    NewBukken_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prices_newbukkens_NewBukken_Id",
                        column: x => x.NewBukken_Id,
                        principalTable: "newbukkens",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "bukkenfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Bukken_Id = table.Column<int>(type: "int", nullable: true),
                    File_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bukkenfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bukkenfiles_bukkens_Bukken_Id",
                        column: x => x.Bukken_Id,
                        principalTable: "bukkens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_bukkenfiles_files_File_Id",
                        column: x => x.File_Id,
                        principalTable: "files",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_bukkenfiles_Bukken_Id",
                table: "bukkenfiles",
                column: "Bukken_Id");

            migrationBuilder.CreateIndex(
                name: "IX_bukkenfiles_File_Id",
                table: "bukkenfiles",
                column: "File_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Bukkens_DetailUrl_ImportedDate",
                table: "bukkens",
                columns: new[] { "DetailUrl", "ImportedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_bukkens_FullText_Id",
                table: "bukkens",
                column: "FullText_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Bukkens_ImportedDate_DetailUrl",
                table: "bukkens",
                columns: new[] { "ImportedDate", "DetailUrl" });

            migrationBuilder.CreateIndex(
                name: "IX_Files_Url",
                table: "files",
                column: "Url");

            migrationBuilder.CreateIndex(
                name: "IX_newbukkenfiles_File_Id",
                table: "newbukkenfiles",
                column: "File_Id");

            migrationBuilder.CreateIndex(
                name: "IX_newbukkenfiles_NewBukken_Id",
                table: "newbukkenfiles",
                column: "NewBukken_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Bukkens_DetailUrl",
                table: "newbukkens",
                column: "DetailUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prices_NewBukken_Id",
                table: "prices",
                column: "NewBukken_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bukkenfiles");

            migrationBuilder.DropTable(
                name: "newbukkenfiles");

            migrationBuilder.DropTable(
                name: "prices");

            migrationBuilder.DropTable(
                name: "bukkens");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "newbukkens");

            migrationBuilder.DropTable(
                name: "bukkenfulltexts");
        }
    }
}

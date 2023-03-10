using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Building",
                columns: table => new
                {
                    BuildingID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeName = table.Column<string>(nullable: true),
                    FloorsN = table.Column<int>(nullable: false),
                    DemolitionPerspective = table.Column<string>(nullable: true),
                    CeilingH = table.Column<double>(nullable: false),
                    Photo = table.Column<string>(nullable: true),
                    Review = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Building", x => x.BuildingID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UName = table.Column<string>(nullable: true),
                    USurame = table.Column<string>(nullable: true),
                    UEmail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Apartment",
                columns: table => new
                {
                    ApartmentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InBldngID = table.Column<int>(nullable: false),
                    RoomsN = table.Column<int>(nullable: false),
                    GeneralArea = table.Column<double>(nullable: false),
                    LivingArea = table.Column<double>(nullable: false),
                    KitchenArea = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartment", x => x.ApartmentID);
                    table.ForeignKey(
                        name: "FK_Apartment_Building_InBldngID",
                        column: x => x.InBldngID,
                        principalTable: "Building",
                        principalColumn: "BuildingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DesignProject",
                columns: table => new
                {
                    DesignProjectID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AptID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignProject", x => x.DesignProjectID);
                    table.ForeignKey(
                        name: "FK_DesignProject_Apartment_AptID",
                        column: x => x.AptID,
                        principalTable: "Apartment",
                        principalColumn: "ApartmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DesignProject_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apartment_InBldngID",
                table: "Apartment",
                column: "InBldngID");

            migrationBuilder.CreateIndex(
                name: "IX_DesignProject_AptID",
                table: "DesignProject",
                column: "AptID");

            migrationBuilder.CreateIndex(
                name: "IX_DesignProject_UserID",
                table: "DesignProject",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DesignProject");

            migrationBuilder.DropTable(
                name: "Apartment");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Building");
        }
    }
}

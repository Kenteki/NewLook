using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NewLook.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProviderID = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    isBlocked = table.Column<bool>(type: "boolean", nullable: false),
                    UI_Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    UI_Theme = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "light")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    CustomString1Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomString1Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomString1Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomString1ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomString2Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomString2Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomString2Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomString2ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomString3Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomString3Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomString3Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomString3ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomText1Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomText1Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomText1Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomText1ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomText2Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomText2Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomText2Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomText2ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomText3Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomText3Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomText3Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomText3ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomNumber1Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomNumber1Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomNumber1Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomNumber1ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomNumber2Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomNumber2Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomNumber2Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomNumber2ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomNumber3Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomNumber3Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomNumber3Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomNumber3ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomLink1Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomLink1Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomLink1Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomLink1ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomLink2Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomLink2Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomLink2Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomLink2ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomLink3Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomLink3Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomLink3Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomLink3ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomBool1Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomBool1Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomBool1Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomBool1ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomBool2Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomBool2Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomBool2Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomBool2ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CustomBool3Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CustomBool3Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomBool3Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomBool3ShowInTable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Inventories_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryId = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomIdElements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    ElementType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomIdElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomIdElements_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryAccesses_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryAccesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTags",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTags", x => new { x.InventoryId, x.TagId });
                    table.ForeignKey(
                        name: "FK_InventoryTags_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryId = table.Column<int>(type: "integer", nullable: false),
                    CustomId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomString1Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomString2Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomString3Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CustomText1Value = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CustomText2Value = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CustomText3Value = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CustomNumber1Value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    CustomNumber2Value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    CustomNumber3Value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    CustomLink1Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CustomLink2Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CustomLink3Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CustomBool1Value = table.Column<bool>(type: "boolean", nullable: true),
                    CustomBool2Value = table.Column<bool>(type: "boolean", nullable: true),
                    CustomBool3Value = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Items_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Equipment" },
                    { 2, "Devices" },
                    { 3, "Books" },
                    { 4, "Documents" },
                    { 5, "Employees" },
                    { 6, "Other" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedAt",
                table: "Comments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_InventoryId",
                table: "Comments",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomIdElements_InventoryId_Order",
                table: "CustomIdElements",
                columns: new[] { "InventoryId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_CategoryId",
                table: "Inventories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_CreatedAt",
                table: "Inventories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_CreatorId",
                table: "Inventories",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_IsPublic",
                table: "Inventories",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAccesses_InventoryId_UserId",
                table: "InventoryAccesses",
                columns: new[] { "InventoryId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAccesses_UserId",
                table: "InventoryAccesses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTags_TagId",
                table: "InventoryTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedAt",
                table: "Items",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedById",
                table: "Items",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Items_InventoryId_CustomId",
                table: "Items",
                columns: new[] { "InventoryId", "CustomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ItemId_UserId",
                table: "Likes",
                columns: new[] { "ItemId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Provider_ProviderID",
                table: "Users",
                columns: new[] { "Provider", "ProviderID" },
                unique: true,
                filter: "\"Provider\" IS NOT NULL AND \"ProviderID\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "CustomIdElements");

            migrationBuilder.DropTable(
                name: "InventoryAccesses");

            migrationBuilder.DropTable(
                name: "InventoryTags");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

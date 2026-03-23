using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutuphaneProjesi.Migrations
{
    /// <inheritdoc />
    public partial class _10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stok",
                table: "Kitaplar",
                newName: "StokAdedi");

            migrationBuilder.RenameColumn(
                name: "KitapAdi",
                table: "Kitaplar",
                newName: "Ad");

            migrationBuilder.AlterColumn<string>(
                name: "Yazar",
                table: "Kitaplar",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "SayfaSayisi",
                table: "Kitaplar",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SayfaSayisi",
                table: "Kitaplar");

            migrationBuilder.RenameColumn(
                name: "StokAdedi",
                table: "Kitaplar",
                newName: "Stok");

            migrationBuilder.RenameColumn(
                name: "Ad",
                table: "Kitaplar",
                newName: "KitapAdi");

            migrationBuilder.AlterColumn<string>(
                name: "Yazar",
                table: "Kitaplar",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}

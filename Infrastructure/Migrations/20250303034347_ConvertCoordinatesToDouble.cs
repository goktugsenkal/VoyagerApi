using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertCoordinatesToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Using raw SQL with USING clause as suggested by PostgreSQL error message
            migrationBuilder.Sql("ALTER TABLE \"Stops\" ALTER COLUMN \"Longitude\" TYPE double precision USING \"Longitude\"::double precision;");
            
            migrationBuilder.Sql("ALTER TABLE \"Stops\" ALTER COLUMN \"Latitude\" TYPE double precision USING \"Latitude\"::double precision;");
            
            // Commented out original code that doesn't include USING clause
            // migrationBuilder.AlterColumn<double>(
            //     name: "Longitude",
            //     table: "Stops",
            //     type: "double precision",
            //     nullable: false,
            //     oldClrType: typeof(string),
            //     oldType: "text");
            // 
            // migrationBuilder.AlterColumn<double>(
            //     name: "Latitude",
            //     table: "Stops",
            //     type: "double precision",
            //     nullable: false,
            //     oldClrType: typeof(string),
            //     oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Longitude",
                table: "Stops",
                type: "text",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "Stops",
                type: "text",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}

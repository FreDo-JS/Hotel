using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hotel.Migrations
{
    /// <inheritdoc />
    public partial class UsersAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Floor", "ResidentId", "RoomNumber", "Status" },
                values: new object[,]
                {
                    { 1, 1, null, 101, "wolny" },
                    { 2, 1, null, 102, "wolny" },
                    { 3, 1, null, 103, "wolny" },
                    { 4, 1, null, 104, "wolny" },
                    { 5, 1, null, 105, "wolny" },
                    { 6, 2, null, 201, "wolny" },
                    { 7, 2, null, 202, "wolny" },
                    { 8, 2, null, 203, "wolny" },
                    { 9, 2, null, 204, "wolny" },
                    { 10, 3, null, 301, "wolny" },
                    { 11, 3, null, 302, "wolny" },
                    { 12, 3, null, 303, "wolny" },
                    { 13, 3, null, 304, "wolny" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirebaseUid", "Name", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 5, 2, 35, 24, 744, DateTimeKind.Local).AddTicks(815), "j4n.k0wal4kisac1k@gmail.com", "kasd782m2akd8922s", "Jan Kowalski", "rezydent" },
                    { 2, new DateTime(2025, 1, 5, 2, 35, 24, 744, DateTimeKind.Local).AddTicks(859), "4nn4l3wa9s1lsp9smki@gmail.com", "d7j89s12mdm12", "Anna Lewandowska", "rezydent" }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "CheckInDate", "CheckOutDate", "CreatedAt", "LastName", "QRCode", "RoomId", "Status", "UserId" },
                values: new object[] { 1, new DateTime(2025, 1, 2, 2, 35, 24, 744, DateTimeKind.Local).AddTicks(1086), new DateTime(2025, 1, 9, 2, 35, 24, 744, DateTimeKind.Local).AddTicks(1089), new DateTime(2025, 1, 5, 2, 35, 24, 744, DateTimeKind.Local).AddTicks(1080), "Kowalski", "821928da892d", 3, "potwierdzona", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealDeal.Bet.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchs2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BettorId",
                table: "Bets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "MatchId",
                table: "Bets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BettorId",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "Bets");
        }
    }
}

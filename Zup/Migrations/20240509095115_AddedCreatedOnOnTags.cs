﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zup.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreatedOnOnTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "TaskEntryTags",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "TaskEntryTags");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GatewayApi.Migrations
{
    /// <inheritdoc />
    public partial class HardenDomainInvariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Runs_RegimenId",
                table: "Runs");

            migrationBuilder.DropIndex(
                name: "IX_ColorMetricSamples_ScanRunId_SequenceIndex",
                table: "ColorMetricSamples");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "Runs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "AlgorithmVersion",
                table: "Runs",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BaselineScanRunId",
                table: "Runs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeltaEFormula",
                table: "Runs",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "Runs",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Regimens",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Patients",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ShadeGuideValue",
                table: "ColorMetricSamples",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Frames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScanRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceIndex = table.Column<int>(type: "integer", nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ByteSize = table.Column<long>(type: "bigint", nullable: false),
                    CapturedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frames", x => x.Id);
                    table.CheckConstraint("CK_Frames_ByteSize", "\"ByteSize\" > 0");
                    table.CheckConstraint("CK_Frames_SequenceIndex", "\"SequenceIndex\" >= 0");
                    table.ForeignKey(
                        name: "FK_Frames_Runs_ScanRunId",
                        column: x => x.ScanRunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Runs_BaselineScanRunId",
                table: "Runs",
                column: "BaselineScanRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Runs_RegimenId_CapturedAtUtc",
                table: "Runs",
                columns: new[] { "RegimenId", "CapturedAtUtc" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Regimens_DurationDays",
                table: "Regimens",
                sql: "\"DurationDays\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Regimens_ScheduledIntervalHours",
                table: "Regimens",
                sql: "\"ScheduledIntervalHours\" > 0");

            migrationBuilder.CreateIndex(
                name: "IX_ColorMetricSamples_ScanRunId_SequenceIndex",
                table: "ColorMetricSamples",
                columns: new[] { "ScanRunId", "SequenceIndex" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ColorMetricSamples_A",
                table: "ColorMetricSamples",
                sql: "\"A\" >= -128 AND \"A\" <= 128");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ColorMetricSamples_B",
                table: "ColorMetricSamples",
                sql: "\"B\" >= -128 AND \"B\" <= 128");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ColorMetricSamples_ConfidenceScore",
                table: "ColorMetricSamples",
                sql: "\"ConfidenceScore\" >= 0 AND \"ConfidenceScore\" <= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ColorMetricSamples_DeltaE",
                table: "ColorMetricSamples",
                sql: "\"DeltaE\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ColorMetricSamples_L",
                table: "ColorMetricSamples",
                sql: "\"L\" >= 0 AND \"L\" <= 100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ColorMetricSamples_SequenceIndex",
                table: "ColorMetricSamples",
                sql: "\"SequenceIndex\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_Frames_ScanRunId_SequenceIndex",
                table: "Frames",
                columns: new[] { "ScanRunId", "SequenceIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Frames_StorageKey",
                table: "Frames",
                column: "StorageKey",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Runs_BaselineScanRunId",
                table: "Runs",
                column: "BaselineScanRunId",
                principalTable: "Runs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Runs_BaselineScanRunId",
                table: "Runs");

            migrationBuilder.DropTable(
                name: "Frames");

            migrationBuilder.DropIndex(
                name: "IX_Runs_BaselineScanRunId",
                table: "Runs");

            migrationBuilder.DropIndex(
                name: "IX_Runs_RegimenId_CapturedAtUtc",
                table: "Runs");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Regimens_DurationDays",
                table: "Regimens");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Regimens_ScheduledIntervalHours",
                table: "Regimens");

            migrationBuilder.DropIndex(
                name: "IX_ColorMetricSamples_ScanRunId_SequenceIndex",
                table: "ColorMetricSamples");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ColorMetricSamples_A",
                table: "ColorMetricSamples");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ColorMetricSamples_B",
                table: "ColorMetricSamples");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ColorMetricSamples_ConfidenceScore",
                table: "ColorMetricSamples");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ColorMetricSamples_DeltaE",
                table: "ColorMetricSamples");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ColorMetricSamples_L",
                table: "ColorMetricSamples");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ColorMetricSamples_SequenceIndex",
                table: "ColorMetricSamples");

            migrationBuilder.DropColumn(
                name: "AlgorithmVersion",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "BaselineScanRunId",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "DeltaEFormula",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "Runs");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "Runs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Regimens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Patients",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ShadeGuideValue",
                table: "ColorMetricSamples",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Runs_RegimenId",
                table: "Runs",
                column: "RegimenId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorMetricSamples_ScanRunId_SequenceIndex",
                table: "ColorMetricSamples",
                columns: new[] { "ScanRunId", "SequenceIndex" });
        }
    }
}

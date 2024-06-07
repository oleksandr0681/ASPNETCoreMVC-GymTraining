using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymTraining.Migrations
{
    /// <inheritdoc />
    public partial class PropertiesChange1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingSchedules_TrainersData_TrainerDataId",
                table: "TrainingSchedules");

            migrationBuilder.DropIndex(
                name: "IX_TrainingSchedules_TrainerDataId",
                table: "TrainingSchedules");

            migrationBuilder.DropColumn(
                name: "TrainerDataId",
                table: "TrainingSchedules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrainerDataId",
                table: "TrainingSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSchedules_TrainerDataId",
                table: "TrainingSchedules",
                column: "TrainerDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingSchedules_TrainersData_TrainerDataId",
                table: "TrainingSchedules",
                column: "TrainerDataId",
                principalTable: "TrainersData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

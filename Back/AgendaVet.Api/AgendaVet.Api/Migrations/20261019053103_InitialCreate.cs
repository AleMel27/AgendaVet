using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgendaVet.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "especies",
                columns: table => new
                {
                    id_especie = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_especies", x => x.id_especie);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "propietarios",
                columns: table => new
                {
                    id_propietario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apellido = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dni_ce = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefono = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    direccion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_registro = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propietarios", x => x.id_propietario);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id_rol);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "servicios",
                columns: table => new
                {
                    id_servicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    precio = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    duracion_estimada_min = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicios", x => x.id_servicio);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "razas",
                columns: table => new
                {
                    id_raza = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_especie = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_razas", x => x.id_raza);
                    table.ForeignKey(
                        name: "FK_razas_especies_id_especie",
                        column: x => x.id_especie,
                        principalTable: "especies",
                        principalColumn: "id_especie",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_rol = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apellido = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefono = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_id_rol",
                        column: x => x.id_rol,
                        principalTable: "roles",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pacientes",
                columns: table => new
                {
                    id_paciente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_propietario = table.Column<int>(type: "int", nullable: false),
                    id_especie = table.Column<int>(type: "int", nullable: false),
                    id_raza = table.Column<int>(type: "int", nullable: true),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sexo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_nacimiento = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    color = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    esterilizado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_registro = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pacientes", x => x.id_paciente);
                    table.ForeignKey(
                        name: "FK_pacientes_especies_id_especie",
                        column: x => x.id_especie,
                        principalTable: "especies",
                        principalColumn: "id_especie",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pacientes_propietarios_id_propietario",
                        column: x => x.id_propietario,
                        principalTable: "propietarios",
                        principalColumn: "id_propietario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pacientes_razas_id_raza",
                        column: x => x.id_raza,
                        principalTable: "razas",
                        principalColumn: "id_raza",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "citas",
                columns: table => new
                {
                    id_cita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    id_veterinario = table.Column<int>(type: "int", nullable: false),
                    id_servicio = table.Column<int>(type: "int", nullable: false),
                    fecha_hora = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    estado = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "Pendiente")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    motivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notas_agendamiento = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_creacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citas", x => x.id_cita);
                    table.ForeignKey(
                        name: "FK_citas_pacientes_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "pacientes",
                        principalColumn: "id_paciente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_citas_servicios_id_servicio",
                        column: x => x.id_servicio,
                        principalTable: "servicios",
                        principalColumn: "id_servicio",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_citas_usuarios_id_veterinario",
                        column: x => x.id_veterinario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "historias_clinicas",
                columns: table => new
                {
                    id_historia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_cita = table.Column<int>(type: "int", nullable: true),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    id_veterinario = table.Column<int>(type: "int", nullable: false),
                    fecha_atencion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    peso_kg = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    temperatura = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: true),
                    frecuencia_cardiaca = table.Column<int>(type: "int", nullable: true),
                    diagnostico = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tratamiento = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observaciones = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historias_clinicas", x => x.id_historia);
                    table.ForeignKey(
                        name: "FK_historias_clinicas_citas_id_cita",
                        column: x => x.id_cita,
                        principalTable: "citas",
                        principalColumn: "id_cita",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_historias_clinicas_pacientes_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "pacientes",
                        principalColumn: "id_paciente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historias_clinicas_usuarios_id_veterinario",
                        column: x => x.id_veterinario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "especies",
                columns: new[] { "id_especie", "nombre" },
                values: new object[,]
                {
                    { 1, "Canino" },
                    { 2, "Felino" },
                    { 3, "Ave" },
                    { 4, "Roedor" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id_rol", "descripcion", "nombre" },
                values: new object[,]
                {
                    { 1, "Acceso total", "Administrador" },
                    { 2, "Atención médica", "Veterinario" },
                    { 3, "Gestión de citas y clientes", "Recepcionista" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_citas_id_paciente",
                table: "citas",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_citas_id_servicio",
                table: "citas",
                column: "id_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_citas_id_veterinario",
                table: "citas",
                column: "id_veterinario");

            migrationBuilder.CreateIndex(
                name: "IX_historias_clinicas_id_cita",
                table: "historias_clinicas",
                column: "id_cita");

            migrationBuilder.CreateIndex(
                name: "IX_historias_clinicas_id_paciente",
                table: "historias_clinicas",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_historias_clinicas_id_veterinario",
                table: "historias_clinicas",
                column: "id_veterinario");

            migrationBuilder.CreateIndex(
                name: "IX_pacientes_id_especie",
                table: "pacientes",
                column: "id_especie");

            migrationBuilder.CreateIndex(
                name: "IX_pacientes_id_propietario",
                table: "pacientes",
                column: "id_propietario");

            migrationBuilder.CreateIndex(
                name: "IX_pacientes_id_raza",
                table: "pacientes",
                column: "id_raza");

            migrationBuilder.CreateIndex(
                name: "IX_propietarios_dni_ce",
                table: "propietarios",
                column: "dni_ce",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_razas_id_especie",
                table: "razas",
                column: "id_especie");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_rol",
                table: "usuarios",
                column: "id_rol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "historias_clinicas");

            migrationBuilder.DropTable(
                name: "citas");

            migrationBuilder.DropTable(
                name: "pacientes");

            migrationBuilder.DropTable(
                name: "servicios");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "propietarios");

            migrationBuilder.DropTable(
                name: "razas");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "especies");
        }
    }
}

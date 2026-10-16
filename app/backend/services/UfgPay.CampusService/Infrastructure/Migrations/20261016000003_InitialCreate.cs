using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UfgPay.CampusService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "estabelecimentos",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estabelecimentos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "servicos_campus",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    estabelecimento_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    preco_centavos = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicos_campus", x => x.id);
                    table.ForeignKey(
                        name: "FK_servicos_campus_estabelecimentos_estabelecimento_id",
                        column: x => x.estabelecimento_id,
                        principalTable: "estabelecimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pedidos_consumo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    servico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    valor_centavos = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    codigo_voucher = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedidos_consumo", x => x.id);
                    table.ForeignKey(
                        name: "FK_pedidos_consumo_servicos_campus_servico_id",
                        column: x => x.servico_id,
                        principalTable: "servicos_campus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_pedidos_user_id",
                table: "pedidos_consumo",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_pedidos_voucher",
                table: "pedidos_consumo",
                column: "codigo_voucher",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_consumo_servico_id",
                table: "pedidos_consumo",
                column: "servico_id");

            migrationBuilder.CreateIndex(
                name: "IX_servicos_campus_estabelecimento_id",
                table: "servicos_campus",
                column: "estabelecimento_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pedidos_consumo");

            migrationBuilder.DropTable(
                name: "servicos_campus");

            migrationBuilder.DropTable(
                name: "estabelecimentos");
        }
    }
}

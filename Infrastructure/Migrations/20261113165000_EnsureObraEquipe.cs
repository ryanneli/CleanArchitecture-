using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Infrastructure.Data;

#nullable disable

namespace Infrastructure.Migrations;

[Migration("20261113165000_EnsureObraEquipe")]
[DbContext(typeof(AppDbContext))]
public partial class EnsureObraEquipe : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF OBJECT_ID(N'[ObraUsuarios]', N'U') IS NULL
            BEGIN
                CREATE TABLE [ObraUsuarios] (
                    [EquipeId] int NOT NULL,
                    [Obrasid] int NOT NULL,
                    CONSTRAINT [PK_ObraUsuarios] PRIMARY KEY ([EquipeId], [Obrasid]),
                    CONSTRAINT [FK_ObraUsuarios_Obras_Obrasid]
                        FOREIGN KEY ([Obrasid]) REFERENCES [Obras] ([id]) ON DELETE CASCADE,
                    CONSTRAINT [FK_ObraUsuarios_Usuarios_EquipeId]
                        FOREIGN KEY ([EquipeId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
                );

                CREATE INDEX [IX_ObraUsuarios_Obrasid]
                    ON [ObraUsuarios] ([Obrasid]);
            END
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF OBJECT_ID(N'[ObraUsuarios]', N'U') IS NOT NULL
                DROP TABLE [ObraUsuarios];
            """);
    }
}

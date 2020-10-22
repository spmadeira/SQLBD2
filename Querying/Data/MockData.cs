using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Querying.Data
{
    public static class MockData
    {
        public static void Seed(Database database)
        {
            var sw = Stopwatch.StartNew();
            CreateSchema(database);
            sw.Stop();
            Console.WriteLine($"Created schema in {sw.ElapsedMilliseconds}ms");
            sw.Restart();
            SeedTables(database);
            sw.Stop();
            Console.WriteLine($"Seeded Tables in {sw.ElapsedMilliseconds}ms");
        }

        private static void SeedTables(Database database)
        {
            SeedTable(database, "Categoria", SeedResources.Categoria);
            SeedTable(database, "Contas", SeedResources.Contas);
            SeedTable(database, "Movimentacao", SeedResources.Movimentacao);
            SeedTable(database, "TipoConta", SeedResources.TipoConta);
            SeedTable(database, "TipoMovimento", SeedResources.TipoMovimento);
            SeedTable(database, "Usuario", SeedResources.Usuario);
        }

        private static void SeedTable(Database database, string tableName, string text)
        {
            using var sr = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(text)));
            
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                database.Insert(tableName, line.Split(",").Select<string, object>(e =>
                {
                    var trimmed = e.Trim();
                    var isNumber = int.TryParse(trimmed, out var numb);
                    if (isNumber) return numb;
                    else return trimmed;
                }).ToArray());
            }
        }
        
        private static void CreateSchema(Database database)
        {
            database.AddTable("Usuario",
                new []{"idUsuario", "Nome", "Logradouro", "Numero", "Bairro", "CEP", "UF", "DataNascimento" });
            
            database.AddTable("Contas",
                new []{ "idConta", "Descricao", "TipoConta_idTipoConta", "Usuario_idUsuario", "SaldoInicial" });
            
            database.AddTable("TipoConta",
                new []{"idTipoConta", "Descrição"});
            
            database.AddTable("Movimentacao",
                new []{"idMovimentacao", "DataMovimentacao", "Descricao", "TipoMovimento_idTipoMovimento", "Categoria_idCategoria", "Contas_idConta", "Valor"});
            
            database.AddTable("TipoMovimento",
                new []{ "idTipoMovimento", "DescMovimentacao" });
            
            database.AddTable("Categoria",
                new []{ "idCategoria", "DescCategoria" });
        }
    }
}
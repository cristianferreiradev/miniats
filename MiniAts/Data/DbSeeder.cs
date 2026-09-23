using Microsoft.EntityFrameworkCore;
using MiniAts.Entities;

namespace MiniAts.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Empresas.AnyAsync())
            return;

        var random = new Random(42);

        var empresas = Enumerable.Range(1, 10)
            .Select(i => new Empresa
            {
                Nome = $"Empresa {i}",
                Cnpj = $"{i:D14}"
            })
            .ToList();

        context.Empresas.AddRange(empresas);
        await context.SaveChangesAsync();

        var cargos = new[] { "Desenvolvedor .NET", "Analista de Dados", "QA",
                             "DevOps", "Product Owner", "Designer" };

        var vagas = Enumerable.Range(1, 200)
            .Select(i => new Vaga
            {
                Titulo = $"{cargos[random.Next(cargos.Length)]} {i}",
                Descricao = "Descrição da vaga.",
                Salario = random.Next(3, 20) * 1000m,
                Ativa = random.Next(10) > 1,
                DataCadastro = DateTime.UtcNow.AddDays(-random.Next(365)),
                EmpresaId = empresas[random.Next(empresas.Count)].Id
            })
            .ToList();

        context.Vagas.AddRange(vagas);
        await context.SaveChangesAsync();

        var candidaturas = new List<Candidatura>();
        foreach (var vaga in vagas)
        {
            var total = random.Next(0, 8);
            for (int i = 0; i < total; i++)
                candidaturas.Add(new Candidatura
                {
                    VagaId = vaga.Id,
                    CandidatoNome = $"Candidato {i} da vaga {vaga.Id}",
                    CandidatoEmail = $"candidato{i}.vaga{vaga.Id}@teste.com",
                    Data = DateTime.UtcNow.AddDays(-random.Next(90)),
                    Status = (StatusCandidatura)random.Next(4)
                });
        }

        context.Candidaturas.AddRange(candidaturas);
        await context.SaveChangesAsync();
    }
}
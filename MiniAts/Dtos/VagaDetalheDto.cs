namespace MiniAts.Dtos
{
    public record VagaDetalheDto(
    int Id,
    string Titulo,
    string Descricao,
    decimal? Salario,
    bool Ativa,
    DateTime DataCadastro,
    int EmpresaId,
    string EmpresaNome,
    int TotalCandidaturas);
}

namespace MiniAts.Dtos
{
    public record VagaListaDto(
     int Id,
     string Titulo,
     decimal? Salario,
     DateTime DataCadastro,
     string EmpresaNome,
     int TotalCandidaturas);
}

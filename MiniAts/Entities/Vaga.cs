namespace MiniAts.Entities
{
    public class Vaga
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal? Salario { get; set; }
        public bool Ativa { get; set; } = true;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        public List<Candidatura> Candidaturas { get; set; } = [];
    }
}

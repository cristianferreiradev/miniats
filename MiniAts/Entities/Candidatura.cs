namespace MiniAts.Entities
{
    public class Candidatura
    {

        public int Id { get; set; }
        public string CandidatoNome { get; set; } = string.Empty;
        public string CandidatoEmail { get; set; } = string.Empty;
        public DateTime Data { get; set; } = DateTime.UtcNow;
        public StatusCandidatura Status { get; set; } = StatusCandidatura.Recebida;

        public int VagaId { get; set; }
        public Vaga Vaga { get; set; } = null!;
    }

    public enum StatusCandidatura
    {
        Recebida = 0,
        EmAnalise = 1,
        Aprovada = 2,
        Reprovada = 3
    }
}

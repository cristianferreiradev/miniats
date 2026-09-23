namespace MiniAts.Entities
{
    public class Empresa
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;

        public List<Vaga> Vagas { get; set; } = [];
    }
}

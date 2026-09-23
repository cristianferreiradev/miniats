using System.ComponentModel.DataAnnotations;

namespace MiniAts.Dtos
{
    public record CriarVagaDto(
        [Required, StringLength(200, MinimumLength = 3)]
    string Titulo,

        [StringLength(4000)]
    string Descricao,

        [Range(0, 1_000_000)]
    decimal? Salario,

        [Required]
    int EmpresaId);
}

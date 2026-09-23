using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniAts.Data;
using MiniAts.Dtos;
using MiniAts.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MiniAts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VagasController : ControllerBase
{
    private readonly AppDbContext _context;

    public VagasController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet("erro-proposital")]
    [AllowAnonymous]

    public IActionResult ErroProposital()
    {
        throw new InvalidOperationException("Explodiu de propósito");
    }
     
    //[HttpGet("resumo")]
    //public async Task<IActionResult> Resumo(CancellationToken ct)
    //{
    //    var total = await _context.Vagas.CountAsync(ct);
    //    var ativas = await _context.Vagas.CountAsync(v => v.Ativa, ct);
    //    var empresas = await _context.Empresas.CountAsync(ct);
    //    var candidaturas = await _context.Candidaturas.CountAsync(ct);

    //    return Ok(new { total, ativas, empresas, candidaturas });
    //}

    [HttpGet("resumo")]
    public async Task<IActionResult> Resumo(CancellationToken ct)
    {
        var t1 = _context.Vagas.CountAsync(ct);
        var t2 = _context.Vagas.CountAsync(v => v.Ativa, ct);
        var t3 = _context.Empresas.CountAsync(ct);
        var t4 = _context.Candidaturas.CountAsync(ct);

        await Task.WhenAll(t1, t2, t3, t4);

        return Ok(new
        {
            total = t1.Result,
            ativas = t2.Result,
            empresas = t3.Result,
            candidaturas = t4.Result
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id, CancellationToken ct)
    {
        var vaga = await _context.Vagas.FirstOrDefaultAsync(v => v.Id == id);

        if (vaga is null)
            return NotFound();

        _context.Vagas.Remove(vaga);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, AtualizarVagaDto dto, CancellationToken ct)
    {
        var vaga = await _context.Vagas.FirstOrDefaultAsync(v => v.Id == id);

        if (vaga is null)
            return NotFound();

        vaga.Titulo = dto.Titulo.Trim();
        vaga.Descricao = dto.Descricao;
        vaga.Salario = dto.Salario;
        vaga.Ativa = dto.Ativa;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<VagaDetalheDto>> Criar(CriarVagaDto dto, CancellationToken ct)
    {
        var empresa = await _context.Empresas
            .FirstOrDefaultAsync(e => e.Id == dto.EmpresaId);

        if (empresa is null)
            return Problem(
                title: "Empresa não encontrada",
                detail: $"Não existe empresa com Id {dto.EmpresaId}.",
                statusCode: StatusCodes.Status400BadRequest);
        var vaga = new Vaga
        {
            Titulo = dto.Titulo.Trim(),
            Descricao = dto.Descricao,
            Salario = dto.Salario,
            EmpresaId = dto.EmpresaId,
            Ativa = true,
            DataCadastro = DateTime.UtcNow
        };

        _context.Vagas.Add(vaga);
        await _context.SaveChangesAsync();

        var resultado = new VagaDetalheDto(
            vaga.Id, vaga.Titulo, vaga.Descricao, vaga.Salario, vaga.Ativa,
            vaga.DataCadastro, vaga.EmpresaId, empresa.Nome, 0);

        return CreatedAtAction(nameof(ObterPorId), new { id = vaga.Id }, resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VagaDetalheDto>> ObterPorId(int id, CancellationToken ct)
    {
        var vaga = await _context.Vagas
            .Where(v => v.Id == id)
            .Select(v => new VagaDetalheDto(
                v.Id,
                v.Titulo,
                v.Descricao,
                v.Salario,
                v.Ativa,
                v.DataCadastro,
                v.EmpresaId,
                v.Empresa.Nome,
                v.Candidaturas.Count))
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (vaga is null)
            return NotFound();

        return Ok(vaga);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ResultadoPaginado<VagaListaDto>>> Listar(
    [FromQuery] string? titulo,
    [FromQuery] int? empresaId,
    [FromQuery] decimal? salarioMinimo,
    [FromQuery] int pagina = 1,
    [FromQuery] int tamanhoPagina = 20,
            CancellationToken ct = default)     // ← novo, sempre por último

    {
        if (pagina < 1) pagina = 1;
        if (tamanhoPagina is < 1 or > 100) tamanhoPagina = 20;

        var query = _context.Vagas.Where(v => v.Ativa);

        if (!string.IsNullOrWhiteSpace(titulo))
            query = query.Where(v => v.Titulo.Contains(titulo));

        if (empresaId.HasValue)
            query = query.Where(v => v.EmpresaId == empresaId.Value);

        if (salarioMinimo.HasValue)
            query = query.Where(v => v.Salario >= salarioMinimo.Value);

        var total = await query.CountAsync();

        var itens = await query
            .OrderByDescending(v => v.DataCadastro)
            .ThenBy(v => v.Id)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(v => new VagaListaDto(
                v.Id,
                v.Titulo,
                v.Salario,
                v.DataCadastro,
                v.Empresa.Nome,
                v.Candidaturas.Count))
            .AsNoTracking()
            .ToListAsync();

        return Ok(new ResultadoPaginado<VagaListaDto>(itens, pagina, tamanhoPagina, total));
    }


    [HttpGet("com-include")]
    public async Task<IActionResult> ListarComInclude()
    {
        var vagas = await _context.Vagas
            .Where(v => v.Ativa)
            .Include(v => v.Empresa)
            .Take(20)
            .ToListAsync();

        var resultado = vagas.Select(v => new
        {
            v.Id,
            v.Titulo,
            v.Salario,
            Empresa = v.Empresa.Nome
        });

        return Ok(resultado);
    }

    [HttpGet("com-projecao")]
    public async Task<IActionResult> ListarComProjecao()
    {
        var resultado = await _context.Vagas
            .Where(v => v.Ativa)
            .Take(20)
            .Select(v => new
            {
                v.Id,
                v.Titulo,
                v.Salario,
                Empresa = v.Empresa.Nome
            })
            .AsNoTracking()
            .ToListAsync();

        return Ok(resultado);
    }


    [HttpGet("n-mais-1")]
    public async Task<IActionResult> DemonstrarNMais1()
    {
        var vagas = await _context.Vagas
            .Where(v => v.Ativa)
            .Take(20)
            .ToListAsync();

        var resultado = new List<object>();

        foreach (var vaga in vagas)
        {
            // força o carregamento explícito da empresa, uma por uma
            await _context.Entry(vaga).Reference(v => v.Empresa).LoadAsync();

            resultado.Add(new { vaga.Id, vaga.Titulo, Empresa = vaga.Empresa.Nome });
        }

        return Ok(resultado);
    }
}
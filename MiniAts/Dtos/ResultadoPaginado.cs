namespace MiniAts.Dtos
{
    public record ResultadoPaginado<T>(
     IReadOnlyList<T> Itens,
     int Pagina,
     int TamanhoPagina,
     int Total)
    {
        public int TotalPaginas => (int)Math.Ceiling(Total / (double)TamanhoPagina);
    }
}
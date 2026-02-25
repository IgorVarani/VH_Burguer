namespace VHBurguer.DTOs.LogProdutoDto
{
    public class LerLogProdutoDto
    {
        public int LogId { get; set; }
        public int? ProdutoId { get; set; }
        public string NomeAnterior { get; set; } = null!;
        public decimal? PrecoAnterior { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}

namespace VHBurguer.DTOs.ProdutoDTO
{
    public class CriarProdutoDTO
    {
        public string Nome { get; set; } = null!;

        public decimal Preco { get; set; }

        public string Descricao { get; set; } = null!;

        public IFormFile Imagem { get; set; } = null!; // A imagem vem via multipart/form-data, ideal para upload de arquivos.

        public List<int> CategoriaIDs { get; set; } = new();
    }
}

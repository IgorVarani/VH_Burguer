using VHBurguer.Domains;

namespace VHBurguer.DTOs.ProdutoDTO
{
    public class LerProdutoDTO
    {
        public int ProdutoID { get; set; }

        public string Nome { get; set; } = null!;

        public decimal Preco { get; set; }

        public string Descricao { get; set; } = null!;

        public bool? StatusProduto { get; set; }

        // Categorias
        public List<int> CategoriaIDs { get; set; } = new();

        public List<string> Categorias { get; set; } = new();

        // Usuário que cadastrou

        public int? UsuarioID { get; set; }
        public string? UsuarioNome { get; set; }
        public string? UsuarioEmail { get; set; }
    }
}

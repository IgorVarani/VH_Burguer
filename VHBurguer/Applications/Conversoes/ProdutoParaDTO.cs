using VHBurguer.Domains;
using VHBurguer.DTOs.ProdutoDto;


namespace VHBurguer.Applications.Conversoes
{
    public class ProdutoParaDto
    {
        public static LerProdutoDto ConverterParaDto(Produto produto)
        {
            return new LerProdutoDto
            {
                ProdutoId = produto.ProdutoId,
                Nome = produto.Nome,
                Preco = produto.Preco,
                Descricao = produto.Descricao,
                StatusProduto = produto.StatusProduto,

                CategoriaIds = produto.Categoria.Select(categoria => categoria.CategoriaId).ToList(),

                Categorias = produto.Categoria.Select(categoria => categoria.Nome).ToList(),

                UsuarioId = produto.UsuarioId,
                UsuarioNome = produto.Usuario.Nome,
                UsuarioEmail = produto.Usuario.Email
            };
        }
    }
}
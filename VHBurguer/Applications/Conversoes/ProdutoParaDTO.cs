using VHBurguer.Domains;
using VHBurguer.DTOs.ProdutoDTO;

namespace VHBurguer.Applications.Conversoes
{
    public class ProdutoParaDTO
    {
        public static LerProdutoDTO ConverterParaDTO(Produto produto)
        {
            return new LerProdutoDTO
            {
                ProdutoID = produto.ProdutoID,
                Nome = produto.Nome,
                Preco = produto.Preco,
                Descricao = produto.Descricao,
                StatusProduto = produto.StatusProduto,

                CategoriaIDs = produto.Categoria.Select(categoria => categoria.CategoriaID).ToList(),

                Categorias = produto.Categoria.Select(categoria => categoria.Nome).ToList(),

                UsuarioID = produto.UsuarioID,
                UsuarioNome = produto.Usuario.Nome,
                UsuarioEmail = produto.Usuario.Email
            };
        }
    }
}
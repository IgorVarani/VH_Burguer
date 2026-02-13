using Microsoft.EntityFrameworkCore;
using VHBurguer.Contexts;
using VHBurguer.Domains;
using VHBurguer.Interfaces;

namespace VHBurguer.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly VH_BurguerContext _context;

        public ProdutoRepository(VH_BurguerContext context)
        {
            _context = context;
        }

        public List<Produto> Listar()
        {
            List<Produto> produtos = _context.Produto
                .Include(produto => produto.Categoria) // Busca produtos e para cada produto, traz as suas categorias.
                .Include(produto => produto.Usuario) // Busca produtos e para cada produto, traz os seus usuários.
                .ToList();

            return produtos;
        }

        public Produto ObterPorID(int id)
        {
            Produto? produto = _context.Produto
                .Include(produtoDB => produtoDB.Categoria)
                .Include(produtoDB => produtoDB.Usuario)

                // Procura no banco (aux produtoDB) e verifica se o ID do produto no banco é igual ao ID passado como parâmetro no método "ObterPorID".
                .FirstOrDefault(produtoDB => produtoDB.ProdutoID == id);

            return produto;
        }

        public bool NomeExiste(string nome, int? produtoIDAtual = null)
        {
            // AsQueryable() -> Monta a consulta para executar passo a passo, monta a consulta na tabela produto e não executa nada no banco ainda.
            var produtoConsultado = _context.Produto.AsQueryable();

            // Se o produto atual tiver valor, então atualizamos o produto.
            if(produtoIDAtual.HasValue)
            {
                produtoConsultado = produtoConsultado.Where(produto => produto.ProdutoID != produtoIDAtual.Value);
            }

            return produtoConsultado.Any(produto => produto.Nome == nome);
        }

        public byte[] ObterImagem(int id)
        {
            var produto = _context.Produto
                .Where(produto => produto.ProdutoID == id)
                .Select(produto => produto.Imagem)
                .FirstOrDefault();

            return produto;
        }
        
        public void Adicionar(Produto produto, List<int> categoriaIDs)
        {
            List<Categoria> categorias = _context.Categoria
                .Where(categoria => categoriaIDs.Contains(categoria.CategoriaID))
                .ToList(); // "Contains" retorna true se houver o registro.

            produto.Categoria = categorias; // Adiciona as categorias incluidas ao produto.

            _context.Produto.Add(produto);
            _context.SaveChanges();
        }

        public void Atualizar(Produto produto, List<int> categoriaIDs)
        {
            Produto? produtoBanco = _context.Produto
                .Include(produto => produto.Categoria)
                .FirstOrDefault(produtoAux => produto.ProdutoID == produto.ProdutoID);

            if(produtoBanco ==  null)
            {
                return;
            }

            produtoBanco.Nome = produto.Nome;
            produtoBanco.Preco = produto.Preco;
            produtoBanco.Descricao = produto.Descricao;

            if (produto.Imagem != null && produto.Imagem.Length > 0)
            {
                produtoBanco.Imagem = produto.Imagem;
            }

            if (produto.StatusProduto.HasValue)
            {
                produtoBanco.StatusProduto = produto.StatusProduto;
            }

            // Busca todas as categorias no banco com o ID igual das categorias que vieram da requisição/front.
            var categorias = _context.Categoria
                .Where(categoria => categoriaIDs.Contains(categoria.CategoriaID))
                .ToList();

            // Remove as ligações atuais entre o produto e as categorias. Ele não apaga a categoria do banco, só remove o vínculo com a tabela ProdutoCategoria.
            produtoBanco.Categoria.Clear();

            foreach(var categoria in categorias)
            {
                produtoBanco.Categoria.Add(categoria);
            }

            _context.SaveChanges();
        }

        public void Remover(int id)
        {
            Produto? produto = _context.Produto.FirstOrDefault(produto => produto.ProdutoID == id);

            if(produto == null)
            {
                return;
            }

            _context.Produto.Remove(produto);
            _context.SaveChanges();
        }
    }
}
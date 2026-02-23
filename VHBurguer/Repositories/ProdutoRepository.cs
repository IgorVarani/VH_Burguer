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

        public Produto ObterPorId(int Id)
        {
            Produto? produto = _context.Produto
                .Include(produtoDB => produtoDB.Categoria)
                .Include(produtoDB => produtoDB.Usuario)

                // Procura no banco (aux produtoDB) e verifica se o Id do produto no banco é igual ao Id passado como parâmetro no método "ObterPorId".
                .FirstOrDefault(produtoDB => produtoDB.ProdutoId == Id);

            return produto;
        }

        public bool NomeExiste(string nome, int? produtoIdAtual = null)
        {
            // AsQueryable() -> Monta a consulta para executar passo a passo, monta a consulta na tabela produto e não executa nada no banco ainda.
            var produtoConsultado = _context.Produto.AsQueryable();

            // Se o produto atual tiver valor, então atualizamos o produto.
            if(produtoIdAtual.HasValue)
            {
                produtoConsultado = produtoConsultado.Where(produto => produto.ProdutoId != produtoIdAtual.Value);
            }

            return produtoConsultado.Any(produto => produto.Nome == nome);
        }

        public byte[] ObterImagem(int Id)
        {
            var produto = _context.Produto
                .Where(produto => produto.ProdutoId == Id)
                .Select(produto => produto.Imagem)
                .FirstOrDefault();

            return produto;
        }
        
        public void Adicionar(Produto produto, List<int> categoriaIds)
        {
            List<Categoria> categorias = _context.Categoria
                .Where(categoria => categoriaIds.Contains(categoria.CategoriaId))
                .ToList(); // "Contains" retorna true se houver o registro.

            produto.Categoria = categorias; // Adiciona as categorias incluIdas ao produto.

            _context.Produto.Add(produto);
            _context.SaveChanges();
        }

        public void Atualizar(Produto produto, List<int> categoriaIds)
        {
            Produto? produtoBanco = _context.Produto
                .Include(produto => produto.Categoria)
                .FirstOrDefault(produtoAux => produto.ProdutoId == produto.ProdutoId);

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

            // Busca todas as categorias no banco com o Id igual das categorias que vieram da requisição/front.
            var categorias = _context.Categoria
                .Where(categoria => categoriaIds.Contains(categoria.CategoriaId))
                .ToList();

            // Remove as ligações atuais entre o produto e as categorias. Ele não apaga a categoria do banco, só remove o vínculo com a tabela ProdutoCategoria.
            produtoBanco.Categoria.Clear();

            foreach(var categoria in categorias)
            {
                produtoBanco.Categoria.Add(categoria);
            }

            _context.SaveChanges();
        }

        public void Remover(int Id)
        {
            Produto? produto = _context.Produto.FirstOrDefault(produto => produto.ProdutoId == Id);

            if(produto == null)
            {
                return;
            }

            _context.Produto.Remove(produto);
            _context.SaveChanges();
        }
    }
}
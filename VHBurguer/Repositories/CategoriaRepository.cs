using VHBurguer.Contexts;
using VHBurguer.Domains;
using VHBurguer.Interfaces;

namespace VHBurguer.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly VH_BurguerContext _context;

        public CategoriaRepository(VH_BurguerContext context)
        {
            _context = context;
        }

        public List<Categoria> Listar()
        {
            return _context.Categoria.ToList();
        }

        public Categoria ObterPorId(int id)
        {
            Categoria categoria = _context.Categoria.FirstOrDefault(c => c.CategoriaId == id);

            return categoria;
        }

        public bool NomeExiste(string nome, int? categoriaIdAtual = null)
        {
            // "AsQueryable()" Cria a consulta inicial da tabela Categoria, mas ainda não executa nada no banco.
            var consulta = _context.Categoria.AsQueryable();

            // Se foi informado um Id atual, significa que estamos EDITANDO uma categoria existente, então vamos ignorar essa própria categoria na verificação.
            if(categoriaIdAtual.HasValue)
            {
                // Remove da busca a categoria com mesmo Id. Evita que o sistema considere o próprio registro como duplicado.
                // Como se fosse um --> SELECT * FROM Categoria WHERE CategoriaId != 5
                consulta = consulta.Where(categoria => categoria.CategoriaId != categoriaIdAtual.Value);
            }

            // Verifica se existe alguma categoria com o mesmo nome, retornando "true" se encontrar ou "false" se não existe.
            return consulta.Any(c => c.Nome == nome);
        }

        public void Adicionar(Categoria categoria)
        {
            _context.Categoria.Add(categoria);
            _context.SaveChanges();
        }

        public void Atualizar(Categoria categoria)
        {
            Categoria categoriaBanco = _context.Categoria.FirstOrDefault(c => c.CategoriaId == categoria.CategoriaId);

            if(categoriaBanco != null)
            {
                return;
            }

            categoriaBanco.Nome = categoria.Nome;

            _context.SaveChanges();
        }

        public void Remover(int id)
        {
            Categoria categoriaBanco = _context.Categoria.FirstOrDefault(c => c.CategoriaId == id);

            if(categoriaBanco == null)
            {
                return;
            }

            _context.Categoria.Remove(categoriaBanco);
            _context.SaveChanges();
        }
    }
}

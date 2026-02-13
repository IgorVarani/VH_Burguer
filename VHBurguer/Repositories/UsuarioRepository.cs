using VHBurguer.Contexts;
using VHBurguer.Domains;
using VHBurguer.Interfaces;

namespace VHBurguer.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public UsuarioRepository()
        { }
            private readonly VH_BurguerContext _context;

        public UsuarioRepository(VH_BurguerContext context)
        {
            _context = context;
        }

        public List<Usuario> Listar()
        {
            return _context.Usuario.ToList();
        }

        public Usuario? ObterPorID(int id)
        {
            // "Find" performa melhor com chave primária.
            return _context.Usuario.Find(id);
        }

        public Usuario? ObterPorEmail(string email)
        {
            // "FirstOrDefault" retorna nosso usuário do banco.
            return _context.Usuario.FirstOrDefault(usuario => usuario.Email == email);
        }

        public bool EmailExiste(string email)
        {
            // "Any" retorna true or false para validar se existe ALGUM usuário com esse e-mail.
            return _context.Usuario.Any(usuario => usuario.Email == email);
        }

        public void Adicionar(Usuario usuario)
        {
            _context.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public void Atualizar(Usuario usuario)
        {
            Usuario? usuarioBanco =
                _context.Usuario.FirstOrDefault(usuario => usuario.UsuarioID == usuario.UsuarioID);

            if(usuarioBanco == null)
            {
                return;
            }

            usuarioBanco.Nome = usuario.Nome;
            usuarioBanco.Email = usuario.Email;
            usuario.Senha = usuario.Senha;

            _context.SaveChanges();
        }

        public void Remover(int id)
        {
            Usuario? usuario = _context.Usuario.FirstOrDefault(usuarioAux => usuarioAux.UsuarioID == id);

            if(usuario == null)
            {
                return; 
            }

            _context.Usuario.Remove(usuario);
            _context.SaveChanges();
        }
    }
}

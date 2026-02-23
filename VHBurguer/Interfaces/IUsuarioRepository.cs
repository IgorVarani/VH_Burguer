using VHBurguer.Domains;

namespace VHBurguer.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> Listar();

        // Pode ser que não retorne usuários na busca, então coloca-se "?" para permitir ser nulo.
        Usuario? ObterPorId(int Id);

        Usuario? ObterPorEmail(string email);

        bool EmailExiste(string email);

        void Adicionar(Usuario usuario);

        void Atualizar(Usuario usuario);

        void Remover(int Id);
    }
}
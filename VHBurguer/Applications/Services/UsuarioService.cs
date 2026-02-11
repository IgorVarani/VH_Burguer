using System.Security.Cryptography;
using System.Text;
using VHBurguer.Domains;
using VHBurguer.DTOs;
using VHBurguer.Exceptions;
using VHBurguer.Interfaces;

namespace VHBurguer.Applications.Services
{
    // service concentra o "como fazer"
    public class UsuarioService
    {
        // _repository é o canal para acessar os dados.
        private readonly IUsuarioRepository _repository;

        // injeção de dependencias
        // implementamos o repositório e o service só depende da interface
        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        // Por que private?
        // pq o método não é regra de negócio e não faz sentido existir fora do UsuarioService
        private static LerUsuarioDTO LerDTO(Usuario usuario) // pega a entidade usuario e gera um DTO
        {
            LerUsuarioDTO lerUsuario = new LerUsuarioDTO
            {
                UsuarioID = usuario.UsuarioID,
                Nome = usuario.Nome,
                Email = usuario.Email,
                StatusUsuario = usuario.StatusUsuario ?? true // se não tiver status no banco, deixa como true
            };

            return lerUsuario;
        }

        public List<LerUsuarioDTO> Listar()
        {
            List<Usuario> usuarios = _repository.Listar();

            List<LerUsuarioDTO> usuariosDTO = usuarios
                .Select(usuarioBanco => LerDTO(usuarioBanco)) // SELECT que percorre cada Usuario e LerDto(usuario)
                .ToList(); // ToList() -> devolve uma lista de DTOs

            return usuariosDTO;
        }

        private static void ValidarEmail(string email)
        {
            if (string.IsNullOrEmpty(email)  || !email.Contains("@")) {
                throw new DomainException("Email inválido");
            }
        }

        private static byte[] HashSenha(string senha)
        {
            if(string.IsNullOrWhiteSpace(senha)) // Garante que a senha não está vazia.
            {
                throw new DomainException("Senha é obrigatória.");
            }

            using var sha256 = SHA256.Create(); // Gera um hash SHA256 e devolve em byte[].
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
        }

        public LerUsuarioDTO ObterPorID(int id)
        {
            Usuario usuario = _repository.ObterPorID(id);

            if (usuario == null)
            {
                throw new DomainException("Usuário não existe.");
            }

            return LerDTO(usuario); // Se existe usuário, converte para DTO e devolve o usuário.
        }

        public LerUsuarioDTO ObterPorEmail(string email)
        {
            Usuario usuario = _repository.ObterPorEmail(email);

            if (usuario == null)
            {
                throw new DomainException("Usuário não existe.");
            }

            return LerDTO(usuario); // Se existe usuário, converte para DTO e devolve o usuário.
        }
    }
}

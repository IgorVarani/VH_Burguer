using System.Security.Cryptography;
using System.Text;
using VHBurguer.Domains;
using VHBurguer.DTOs.UsuarioDto;
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
        // pq o método não é regra de negócio e não faz sentIdo existir fora do UsuarioService
        private static LerUsuarioDto LerDto(Usuario usuario) // pega a entIdade usuario e gera um Dto
        {
            LerUsuarioDto lerUsuario = new LerUsuarioDto
            {
                UsuarioId = usuario.UsuarioId,
                Nome = usuario.Nome,
                Email = usuario.Email,
                StatusUsuario = usuario.StatusUsuario ?? true // se não tiver status no banco, deixa como true
            };

            return lerUsuario;
        }

        public List<LerUsuarioDto> Listar()
        {
            List<Usuario> usuarios = _repository.Listar();

            List<LerUsuarioDto> usuariosDto = usuarios
                .Select(usuarioBanco => LerDto(usuarioBanco)) // SELECT que percorre cada Usuario e LerDto(usuario)
                .ToList(); // ToList() -> devolve uma lista de Dtos

            return usuariosDto;
        }

        private static void ValidarEmail(string email)
        {
            if (string.IsNullOrEmpty(email)  || !email.Contains("@")) {
                throw new DomainException("Email inválIdo");
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

        public LerUsuarioDto ObterPorId(int Id)
        {
            Usuario usuario = _repository.ObterPorId(Id);

            if (usuario == null)
            {
                throw new DomainException("Usuário não existe.");
            }

            return LerDto(usuario); // Se existe usuário, converte para Dto e devolve o usuário.
        }

        public LerUsuarioDto ObterPorEmail(string email)
        {
            Usuario usuario = _repository.ObterPorEmail(email);

            if (usuario == null)
            {
                throw new DomainException("Usuário não existe.");
            }

            return LerDto(usuario); // Se existe usuário, converte para Dto e devolve o usuário.
        }

        public LerUsuarioDto Adicionar(CriarUsuarioDto usuarioDto)
        {
            ValidarEmail(usuarioDto.Email);

            if( _repository.EmailExiste(usuarioDto.Email))
            {
                throw new DomainException("Já existe um usuário com este e-mail.");
            }

            Usuario usuario = new Usuario // Criando entIdade "Usuário"
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                Senha = HashSenha(usuarioDto.Senha),
                StatusUsuario = true
            };

            _repository.Adicionar(usuario);

            return LerDto(usuario); // Retorna "LerDto" para não retornar o objeto com a senha.
        }

        public LerUsuarioDto Atualizar(int Id, CriarUsuarioDto usuarioDto)
        {
            Usuario usuarioBanco = _repository.ObterPorId(Id);

            if(usuarioBanco == null)
            {
                throw new DomainException("Usuário não encontrado.");
            }

            ValidarEmail(usuarioDto.Email);

            Usuario usuarioComMesmoEmail = _repository.ObterPorEmail(usuarioDto.Email);

            if(usuarioComMesmoEmail != null && usuarioComMesmoEmail.UsuarioId != Id)
            {
                throw new DomainException("Já existe um usuário com este e-mail.");
            }

            // Substitui as informações do banco (usuarioBanco)
            // Inserindo as alterações que estão vindo de usuarioDto.
            usuarioBanco.Nome = usuarioDto.Nome;
            usuarioBanco.Email = usuarioDto.Email;
            usuarioBanco.Senha = HashSenha(usuarioDto.Senha);

            _repository.Atualizar(usuarioBanco);

            return LerDto(usuarioBanco);
        }

        public void Remover(int Id)
        {
            Usuario usuario = _repository.ObterPorId(Id);

            if(usuario == null)
            {
                throw new DomainException("Usuário não encontrado.");
            }

            _repository.Remover(Id);
        }
    }
}
using VHBurguer.Applications.Autenticacao;
using VHBurguer.Domains;
using VHBurguer.DTOs.AutenticacaoDTO;
using VHBurguer.Exceptions;
using VHBurguer.Interfaces;

namespace VHBurguer.Applications.Services
{
    public class AutenticacaoService
    {
        private readonly IUsuarioRepository _repository;
        private readonly GeradorTokenJWT _tokenJWT;

        public AutenticacaoService(IUsuarioRepository repository, GeradorTokenJWT tokenJWT)
        {
            _repository = repository;
            _tokenJWT = tokenJWT;
        }

        // Compara a hash SHA256
        private static bool VerificarSenha(string senhaDigitada, byte[] senhaHashBanco)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hashDigitado = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senhaDigitada));

            return hashDigitado.SequenceEqual(senhaHashBanco);
        }

        public TokenDTO Login(LoginDTO loginDTO)
        {
            Usuario usuario = _repository.ObterPorEmail(loginDTO.Email);

            if (usuario == null)
            {
                throw new DomainException("E-mail ou senha inválidos.");
            }

            // Comparar a senha digitada com a senha armazenada.
            if(!VerificarSenha(loginDTO.Senha, usuario.Senha))
            {
                throw new DomainException("E-mail ou senha inváliods.");
            }

            // Gerando o token.
            var token = _tokenJWT.GerarToken(usuario);

            TokenDTO novoToken = new TokenDTO { Token = token };

            return novoToken;
        }
    }
}

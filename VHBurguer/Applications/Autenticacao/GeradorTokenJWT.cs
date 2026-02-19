using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VHBurguer.Domains;
using VHBurguer.Exceptions;

namespace VHBurguer.Applications.Autenticacao
{
    public class GeradorTokenJWT
    {
        private readonly IConfiguration _config;

        // Recebe as configurações do "appsettings.json".
        public GeradorTokenJWT(IConfiguration config)
        {
            _config = config;
        }

        public string GerarToken(Usuario usuario)
        {
            // "Key" é a chave secreta usada para assinar o token.
            // Garante que o token não foi alterado.
            var chave = _config["Jwt:Key"]!;

            // "Issuer" é quem gerou o token (nome da API e sistema que gerou).
            // a API valida se o token veio do emissor correto.
            var issuer = _config["Jwt:Issuer"]!;

            // "Audience" é para quem o token foi criado.
            // Define qual sistema pode usar o token.
            var audience = _config["Jwt:Issuer"]!;

            // O tempo de expiração define por quantos minutos o token será válido.
            // Depois disso, o usuário precisa logar novamente.
            var expiraEmMinutos = int.Parse(_config["Jwt:ExpiraEmMinutos"]!);

            // Converte a chave para bytes (necessário para criar a assinatura).
            var keyBytes = Encoding.UTF8.GetBytes(chave);

            //
            if(keyBytes.Length < 32)
            {
                throw new DomainException("Jwt: Key precisa ter pelo menos 32 caracteres (256 bits).");
            }

            // Cria a chave de segurança usada para assinar o token.
            var securityKey = new SymmetricSecurityKey(keyBytes);

            // Define o algoritmo de assinatura do token.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // "Claims" são as informações do usuário que vão dentro do token.
            // Essas informações podem ser recuperadas na API para identificar quem está logado.
            var claims = new List<Claim>
            {
                // ID do usuário (para saber quem fez a ação):
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),

                // Nome do usuário:
                new Claim(ClaimTypes.Name, usuario.Nome),

                // Email do usuário:
                new Claim(ClaimTypes.Email, usuario.Email),
            };

            // Cria o token JWT com todas as informações.
            var token = new JwtSecurityToken(
                issuer:  issuer,                                        // <-- Quem gerou o token.
                audience: audience,                                     // <-- Quem pode usar o token.
                claims: claims,                                         // <-- Dados do usuário.
                expires: DateTime.Now.AddMinutes(expiraEmMinutos),      // <-- Validade do token.
                signingCredentials: credentials                         // <-- Assinatura de segurança.
            );

            // Converte o token para string e essa string é enviada para o cliente.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

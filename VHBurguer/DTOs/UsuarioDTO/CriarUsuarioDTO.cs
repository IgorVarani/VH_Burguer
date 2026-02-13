namespace VHBurguer.DTOs.UsuarioDTO
{
    public class CriarUsuarioDTO
    {
        public string Nome { get; set; } = null;
        public string Email { get; internal set; }
        public string Senha { get; internal set; }

        public CriarUsuarioDTO(){ }
    }
}

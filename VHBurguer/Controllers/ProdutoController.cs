using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VHBurguer.Applications.Services;
using VHBurguer.DTOs.ProdutoDTO;
using VHBurguer.Exceptions;

namespace VHBurguer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _service;

        public ProdutoController(ProdutoService service)
        {
            _service = service;
        }

        // autenticação do usuário
        private int ObterUsuarioIDLogado()
        {
            // Busca no token/claims o valor armazenado como ID do usuário.
            // ClaimTypes.NameIdentifier geralmente guarda o ID do usuário no JWT.
            string? idTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrWhiteSpace(idTexto))
            {
                throw new DomainException("Usuário não autenticado.");
            }

            // Converte o ID que veio como texto para inteiro, nosso UsuarioID no sistema está como "int".
            // As Claims (informações do usuário dentro do token) sempre são armazenadas como texto.
            return int.Parse(idTexto);
        }

        [HttpGet]
        public ActionResult<List<LerProdutoDTO>> Listar()
        {
            List<LerProdutoDTO> produtos = _service.Listar();

            //return StatusCode(200, produtos);
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public ActionResult<LerProdutoDTO> ObterPorID(int id)
        {
            LerProdutoDTO produto = _service.ObterPorID(id);

            if (produto == null)
            {
                //return StatusCode(404);
                return NotFound();
            }

            return Ok(produto);
        }

        // "GET" -> API/Produto/5/Imagem.
        [HttpGet("{id}/imagem")]
        public ActionResult ObterImagem(int id)
        {
            try
            {
                var imagem = _service.ObterImagem(id);

                // Retorna o arquivo para o navegador, "image/jpeg" informa o tipo da imagem (MIME type).
                // O navegador entende que deve renderizar como imagem.
                return File(imagem, "imagem/jpeg");
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message); // NotFound -> não encontrado.
            }
        }
    }
}
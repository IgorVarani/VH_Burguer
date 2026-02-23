using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VHBurguer.Applications.Services;
using VHBurguer.DTOs.ProdutoDto;
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
        private int ObterUsuarioIdLogado()
        {
            // busca no token/claims o valor armazenado como Id do usuário
            // ClaimTypes.NameIdentifier geralmente guarda o Id do usuário no Jwt
            string? IdTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(IdTexto))
            {
                throw new DomainException("Usuário não autenticado");
            }

            // Converte o Id que veio como texto para inteiro
            // nosso UsuarioId no sistema está como int
            // as Claims (informações do usuário dentro do token) sempre são armazenadas como texto.
            return int.Parse(IdTexto);
        }

        [HttpGet]
        public ActionResult<List<LerProdutoDto>> Listar()
        {
            List<LerProdutoDto> produtos = _service.Listar();

            //return StatusCode(200, produtos);
            return Ok(produtos);
        }

        [HttpGet("{Id}")]
        public ActionResult<LerProdutoDto> ObterPorId(int Id)
        {
            LerProdutoDto produto = _service.ObterPorId(Id);

            if (produto == null)
            {
                //return StatusCode(404);
                return NotFound();
            }

            return Ok(produto);
        }

        // GET -> api/produto/5/imagem
        [HttpGet("{Id}/imagem")]
        public ActionResult ObterImagem(int Id)
        {
            try
            {
                var imagem = _service.ObterImagem(Id);

                // Retorna o arquivo para o navegador
                // "image/jpeg" informa o tipo da imagem (MIME type)
                // O navegador entende que deve renderizar como imagem
                return File(imagem, "image/jpeg");
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message); // NotFound -> não encontrado
            }
        }

        [HttpPost]
        // indica que recebe dados no formato multipart/form-data
        // necessário quando enviamos arquivos (ex. imagem do produto)
        [Consumes("multipart/form-data")]
        [Authorize] // exige login para adicionar produtos

        // [FromForm] -> diz que os dados vem do formulário da requisição (multipart/form-data)
        public ActionResult Adicionar([FromForm] CriarProdutoDto produtoDto)
        {
            try
            {
                int usuarioId = ObterUsuarioIdLogado();

                // o cadastro fica associado ao usuário logado
                _service.Adicionar(produtoDto, usuarioId);

                return StatusCode(201); // Created
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{Id}")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public ActionResult Atualizar(int Id, [FromForm] AtualizarProdutoDto produtoDto)
        {
            try
            {
                _service.Atualizar(Id, produtoDto);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{Id}")]
        [Authorize]
        public ActionResult Remover(int Id)
        {
            try
            {
                _service.Remover(Id);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

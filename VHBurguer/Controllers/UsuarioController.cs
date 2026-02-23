using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VHBurguer.Applications.Services;
using VHBurguer.DTOs.UsuarioDto;
using VHBurguer.Exceptions;

namespace VHBurguer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService service)
        {
            _service = service;
        }

        [HttpGet] // "GET" Lista informações.

        public ActionResult<List<LerUsuarioDto>> Listar()
        {
            List<LerUsuarioDto> usuarios = _service.Listar();

            // Retorna a lista de usuários, a partir da Dto de Services.
            return Ok(usuarios); // OK = 200 = Deu certo.
        }

        [HttpGet("{Id}")]
        public ActionResult<LerUsuarioDto> ObterPorId(int Id)
        {
            LerUsuarioDto usuario = _service.ObterPorId(Id);
            if (usuario == null)
            {
                return NotFound(); // Erro 404 (StatusCode) = NÃO ENCONTRADO
            }

            return Ok(usuario);
        }

        [HttpGet("email/{email}")]
        public ActionResult<LerUsuarioDto> ObterPorEmail(string email)
        {
            LerUsuarioDto usuario = _service.ObterPorEmail(email);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [HttpPost] // "POST" Envia dados.
        public ActionResult<LerUsuarioDto> Adicionar(CriarUsuarioDto usuarioDto)
        {
            try
            {
                LerUsuarioDto usuarioCriado = _service.Adicionar(usuarioDto);

                return StatusCode(201, usuarioCriado);
            }

            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{Id}")]
        public ActionResult<LerUsuarioDto> Atualizar(int Id, CriarUsuarioDto usuarioDto)
        {
            try
            {
                LerUsuarioDto usuarioAtualizado = _service.Atualizar(Id, usuarioDto);
                return StatusCode(200, usuarioAtualizado);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{Id}")] // "Delete" Remove dados. No nosso banco o "Delete" vai apenas inativar o usuário por conta da trigger (isso é um soft delete).
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

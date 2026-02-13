using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VHBurguer.Applications.Services;
using VHBurguer.DTOs.UsuarioDTO;
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

        public ActionResult<List<LerUsuarioDTO>> Listar()
        {
            List<LerUsuarioDTO> usuarios = _service.Listar();

            // Retorna a lista de usuários, a partir da DTO de Services.
            return Ok(usuarios); // OK = 200 = Deu certo.
        }

        [HttpGet("{id}")]
        public ActionResult<LerUsuarioDTO> ObterPorID(int id)
        {
            LerUsuarioDTO usuario = _service.ObterPorID(id);
            if (usuario == null)
            {
                return NotFound(); // Erro 404 (StatusCode) = NÃO ENCONTRADO
            }

            return Ok(usuario);
        }

        [HttpGet("email/{email}")]
        public ActionResult<LerUsuarioDTO> ObterPorEmail(string email)
        {
            LerUsuarioDTO usuario = _service.ObterPorEmail(email);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [HttpPost] // "POST" Envia dados.
        public ActionResult<LerUsuarioDTO> Adicionar(CriarUsuarioDTO usuarioDTO)
        {
            try
            {
                LerUsuarioDTO usuarioCriado = _service.Adicionar(usuarioDTO);

                return StatusCode(201, usuarioCriado);
            }

            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<LerUsuarioDTO> Atualizar(int id, CriarUsuarioDTO usuarioDTO)
        {
            try
            {
                LerUsuarioDTO usuarioAtualizado = _service.Atualizar(id, usuarioDTO);
                return StatusCode(200, usuarioAtualizado);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")] // "Delete" Remove dados. No nosso banco o "Delete" vai apenas inativar o usuário por conta da trigger (isso é um soft delete).
        public ActionResult Remover(int id)
        {
            try
            {
                _service.Remover(id);
                return NoContent();
            }

            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

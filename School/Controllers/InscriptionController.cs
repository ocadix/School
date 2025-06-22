using Inscription.Interface;
using Microsoft.AspNetCore.Mvc;

namespace School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscriptionController(IInscription inscription) : ControllerBase
    {
        private readonly IInscription _inscription = inscription;

        [HttpPost]
        [Route("SaveOrUpdateInscription")]
        public async Task<IActionResult> SaveOrUpdateInscription([FromBody] Inscription.Entities.Inscription inscriptionParam)
        {
            try
            {
                var response = await _inscription.SaveOrUpdateInscription(inscriptionParam);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, " Error en Guardar o Actualizar - SubjectsController Controller: " + e.Message.ToString());
            }
        }

        [HttpDelete]
        [Route("DeleteInscription")]
        public async Task<IActionResult> DeleteInscription([FromBody] string code)
        {
            try
            {
                var response = await _inscription.DeleteInscription(code);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, " Error en Eliminar - SubjectsController Controller: " + e.Message.ToString());
            }
        }

        [HttpGet]
        [Route("ListInscription")]
        public async Task<IActionResult> ListInscription([FromBody] string? code)
        {
            try
            {
                var response = await _inscription.ListInscription(code);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, " Error en Listar - SubjectsController Controller: " + e.Message.ToString());
            }
        }
    }
}

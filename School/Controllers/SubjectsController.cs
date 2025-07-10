using Microsoft.AspNetCore.Mvc;
using Subjects.Interface;

namespace School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController(ISubjects subjects) : ControllerBase
    {
        private readonly ISubjects _subjects = subjects;


        [HttpPost]
        [Route("SaveOrUpdateSubjects")]
        public async Task<IActionResult> SaveOrUpdateSubjects([FromBody] Subjects.Entities.Subjects subjects)
        {
            try
            {
                var response = await _subjects.SaveOrUpdateSubjects(subjects);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, " Error en Guardar o Actualizar - SubjectsController Controller: " + e.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteSubjects")]
        public async Task<IActionResult> DeleteSubjects([FromBody] string code)
        {
            try
            {
                var response = await _subjects.DeleteSubjects(code);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, " Error en Eliminar - SubjectsController Controller: " + e.Message);
            }
        }

        [HttpGet]
        [Route("ListSubjects")]
        public async Task<IActionResult> ListSubjects()
        {
            try
            {
                var response = await _subjects.ListSubjects();
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, " Error en Listar - SubjectsController Controller: " + e.Message);
            }
        }

        [HttpGet]
        [Route("ListSubjectsCredits")]
        public async Task<IActionResult> ListSubjectsCredits(string code)
        {
            try
            {
                var response = await _subjects.ListSubjectsCredits(code);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, " Error en Listar - SubjectsController Controller: " + e.Message);
            }
        }


    }
}

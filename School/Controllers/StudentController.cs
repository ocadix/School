using Microsoft.AspNetCore.Mvc;
using Student.Interface;

namespace School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController(IStudent student) : ControllerBase
    {
        private readonly IStudent _student = student;

        [HttpPost]
        [Route("SaveOrUpdateStudent")]
        public async Task<IActionResult> SaveOrUpdateStudent([FromBody] Student.Entities.Student studentparam)
        {
            try
            {
                var response = await _student.SaveOrUpdateStudent(studentparam);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, " Error en Guardar o Actualizar - SubjectsController Controller: " + e.Message.ToString());
            }
        }

        [HttpDelete]
        [Route("DeleteStudent")]
        public async Task<IActionResult> DeleteStudent([FromBody] string code)
        {
            try
            {
                var response = await _student.DeleteStudent(code);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, " Error en Eliminar - SubjectsController Controller: " + e.Message.ToString());
            }
        }

        [HttpGet]
        [Route("ListStudent")]
        public async Task<IActionResult> ListStudent()
        {
            try
            {
                var response = await _student.ListStudent();
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, " Error en Listar - SubjectsController Controller: " + e.Message.ToString());
            }
        }

        [HttpGet]
        [Route("ListStudentDocument")]
        public async Task<IActionResult> ListStudentDocument(string Code)
        {
            try
            {
                var response = await _student.ListStudentDocument(Code);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, " Error en Listar - SubjectsController Controller: " + e.Message.ToString());
            }
        }
    }
}

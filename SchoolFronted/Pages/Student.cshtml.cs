using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolFronted.Model;
using System.Text;
using System.Text.Json;

namespace SchoolFronted.Pages
{
    public class StudentModel(IHttpClientFactory clientFactory) : PageModel
    {
        private readonly IHttpClientFactory _clientFactory = clientFactory;

        [BindProperty]
        public Student EditingStudent { get; set; }

        [BindProperty]
        public int? EditingIndex { get; set; }

        [BindProperty]
        public RegisterSubjectViewModelStudent Newstudents { get; set; } = new RegisterSubjectViewModelStudent(); // Cambia el tipo aqu�

        public List<Student> Students { get; set; } = [];

        public async Task OnGetAsync()
        {
            await LoadStudentAsync();
        }

        private async Task LoadStudentAsync()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:5158/api/Student/ListStudent");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Students = JsonSerializer.Deserialize<List<Student>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var client = _clientFactory.CreateClient();

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    ModelState.AddModelError(string.Empty, "No se proporcion� un c�digo de estudiante para eliminar.");
                    await LoadStudentAsync();
                    return Page();
                }
                var jsonString = JsonSerializer.Serialize(id);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri("https://localhost:5158/api/Student/DeleteStudent"),
                    Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
                };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();


                return RedirectToPage();
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al eliminar al estudiante: {ex.Message}. Verifique la conexi�n o el c�digo del estudiante.");
                await LoadStudentAsync();
                return Page();
            }
            catch (JsonException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error de serializaci�n/deserializaci�n al eliminar: {ex.Message}");
                await LoadStudentAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurri� un error inesperado al eliminar: {ex.Message}");
                await LoadStudentAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRegister()
        {
            var subjectToSave = new Student
            {
                Code = null,
                Name = Newstudents.Name,
                NumDocument = Newstudents.NumDocument,
                Email = Newstudents.Email,
            };

            var client = _clientFactory.CreateClient();
            var json = JsonSerializer.Serialize(subjectToSave);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://localhost:5158/api/Student/SaveOrUpdateStudent", content);
                response.EnsureSuccessStatusCode();

                Newstudents = new RegisterSubjectViewModelStudent();
                return RedirectToPage();
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al registrar al estudiante: {ex.Message}");
                await LoadStudentAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            await LoadStudentAsync();

            if (EditingIndex.HasValue && EditingIndex.Value >= 0 && EditingIndex.Value < Students.Count)
            {
                var originalSubject = Students[EditingIndex.Value];
                EditingStudent = new Student
                {
                    Code = originalSubject.Code,
                    Name = originalSubject.Name,
                    NumDocument = originalSubject.NumDocument,
                    Email = originalSubject.Email
                };
            }
            else
            {
                ModelState.AddModelError(string.Empty, "�ndice de edici�n inv�lido.");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {

            var client = _clientFactory.CreateClient();
            var json = JsonSerializer.Serialize(EditingStudent);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://localhost:5158/api/Student/SaveOrUpdateStudent", content);
                response.EnsureSuccessStatusCode();

                EditingIndex = null;
                return RedirectToPage();
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al guardar al estudiante: {ex.Message}");
                await LoadStudentAsync();
                return Page();
            }
            catch (JsonException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al serializar al estudiante: {ex.Message}");
                await LoadStudentAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCancelEditAsync()
        {
            EditingIndex = null;
            return RedirectToPage();
        }


    }
}

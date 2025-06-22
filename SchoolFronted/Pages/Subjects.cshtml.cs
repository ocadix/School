using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolFronted.Model;
using System.Text;
using System.Text.Json;

namespace SchoolFronted.Pages
{
    public class SubjectsModel(IHttpClientFactory clientFactory) : PageModel
    {
        private readonly IHttpClientFactory _clientFactory = clientFactory;

        [BindProperty]
        public Subject EditingSubject { get; set; }

        [BindProperty]
        public int? EditingIndex { get; set; }

        [BindProperty]
        public RegisterSubjectViewModel NewSubject { get; set; } = new RegisterSubjectViewModel(); // Cambia el tipo aquí

        public List<Subject> Subjects { get; set; } = [];

        public async Task OnGetAsync()
        {
            await LoadSubjectsAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var client = _clientFactory.CreateClient();

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    ModelState.AddModelError(string.Empty, "No se proporcionó un código de materia para eliminar.");
                    await LoadSubjectsAsync();
                    return Page();
                }
                var jsonString = JsonSerializer.Serialize(id);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri("https://localhost:7253/api/Subjects/DeleteSubjects"),
                    Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
                };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();


                return RedirectToPage();
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al eliminar la materia: {ex.Message}. Verifique la conexión o el código de la materia.");
                await LoadSubjectsAsync();
                return Page();
            }
            catch (JsonException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error de serialización/deserialización al eliminar: {ex.Message}");
                await LoadSubjectsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error inesperado al eliminar: {ex.Message}");
                await LoadSubjectsAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRegister()
        {
            var subjectToSave = new Subject
            {
                Code = null,
                Name = NewSubject.Name,
                Credits = NewSubject.Credits
            };

            var client = _clientFactory.CreateClient();
            var json = JsonSerializer.Serialize(subjectToSave);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://localhost:7253/api/Subjects/SaveOrUpdateSubjects", content);
                response.EnsureSuccessStatusCode();

                NewSubject = new RegisterSubjectViewModel();
                return RedirectToPage();
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al registrar la materia: {ex.Message}");
                await LoadSubjectsAsync();
                return Page();
            }
        }

        private async Task LoadSubjectsAsync()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7253/api/Subjects/ListSubjects");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Subjects = JsonSerializer.Deserialize<List<Subject>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            await LoadSubjectsAsync();

            if (EditingIndex.HasValue && EditingIndex.Value >= 0 && EditingIndex.Value < Subjects.Count)
            {
                var originalSubject = Subjects[EditingIndex.Value];
                EditingSubject = new Subject
                {
                    Code = originalSubject.Code,
                    Name = originalSubject.Name,
                    Credits = originalSubject.Credits
                };
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Índice de edición inválido.");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {

            var client = _clientFactory.CreateClient();
            var json = JsonSerializer.Serialize(EditingSubject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://localhost:7253/api/Subjects/SaveOrUpdateSubjects", content);
                response.EnsureSuccessStatusCode();

                EditingIndex = null;
                return RedirectToPage();
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al guardar la materia: {ex.Message}");
                await LoadSubjectsAsync();
                return Page();
            }
            catch (JsonException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al serializar la materia: {ex.Message}");
                await LoadSubjectsAsync();
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

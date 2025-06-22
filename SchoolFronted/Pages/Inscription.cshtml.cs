using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolFronted.Model;
using System.Text;
using System.Text.Json;

namespace SchoolFronted.Pages
{
    public class InscriptionModel(IHttpClientFactory clientFactory) : PageModel
    {
        private readonly IHttpClientFactory _clientFactory = clientFactory;

        [BindProperty]
        public Inscription EditingInscription { get; set; }

        [BindProperty]
        public int? EditingIndex { get; set; }

        [BindProperty]
        public RegisterSubjectViewModelInscription NewInscription { get; set; } = new RegisterSubjectViewModelInscription(); // Cambia el tipo aquí

        public List<Inscription> Inscriptions { get; set; } = [];

        public async Task OnGetAsync()
        {
            await LoadInscriptionAsync();
        }

        private async Task LoadInscriptionAsync()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7253/api/Inscription/ListInscription");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Inscriptions = JsonSerializer.Deserialize<List<Inscription>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var client = _clientFactory.CreateClient();

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    ModelState.AddModelError(string.Empty, "No se proporcionó un código de inscripción para eliminar.");
                    await LoadInscriptionAsync();
                    return Page();
                }
                var jsonString = JsonSerializer.Serialize(id);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri("https://localhost:7253/api/Inscription/DeleteInscription"),
                    Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
                };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();


                return RedirectToPage();
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al eliminar la inscripción: {ex.Message}. Verifique la conexión o el código de la inscripción.");
                await LoadInscriptionAsync();
                return Page();
            }
            catch (JsonException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error de serialización/deserialización al eliminar: {ex.Message}");
                await LoadInscriptionAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error inesperado al eliminar: {ex.Message}");
                await LoadInscriptionAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRegister()
        {
            if (NewInscription == null)
                ModelState.AddModelError(string.Empty, $"Error al registrar la inscripcióm.");


            var client = _clientFactory.CreateClient();
            var json = JsonSerializer.Serialize(NewInscription);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://localhost:7253/api/Inscription/SaveOrUpdateInscription", content);
                response.EnsureSuccessStatusCode();

                NewInscription = new RegisterSubjectViewModelInscription();
                return RedirectToPage();
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al registrar al estudiante: {ex.Message}");
                await LoadInscriptionAsync();
                return Page();
            }
        }

    }
}

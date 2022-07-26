using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace RazorPagesRecipes.Pages.Recipes
{
    public class RecipesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private IWebHostEnvironment _host;
        public List<string> Categories { get; set; } = new List<string>();
        [BindProperty]
        public IFormFile? RecipeImage { get; set; }
        [BindProperty]
        public List<bool> IsCheckedCategory { get; set; } = new List<bool>();

        public RecipesModel(IHttpClientFactory client, IWebHostEnvironment host)
        {
            _httpClientFactory = client;
            _httpClient = _httpClientFactory.CreateClient("recipe");
            _host = host;
        }


        public async Task OnGet()
        {
            var httpResponseMessage =
                await _httpClient.GetAsync($"/categories");
            bool isRequestSucceed = httpResponseMessage.IsSuccessStatusCode;
            var categoryData = await httpResponseMessage.Content.ReadAsStringAsync();
            Categories = JsonSerializer.Deserialize<List<string>>(categoryData);
        }

        public async Task<IActionResult> OnPostAdd()
        {

            var x = RecipeImage;
            MemoryStream ms = new MemoryStream();

            RecipeImage.OpenReadStream().CopyToAsync(ms);
            var data = ms.ToArray();
            Guid id = Guid.NewGuid();

            var filePath = $"{_host.WebRootPath}/RecipesImages/{id}.{System.IO.Path.GetExtension(RecipeImage.FileName)}";
            System.IO.File.WriteAllBytes(filePath, data);
            return Page();

        }
    }
}

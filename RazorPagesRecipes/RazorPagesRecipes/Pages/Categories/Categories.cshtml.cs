using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesRecipes;
using System.Text;
using System.Text.Json;

namespace RazorPagesRecipes.Pages.Categories
{
    public class CategoriesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        public bool IsRequestSucceed { get; }
        public List<string> Categories { get; set; } = new List<string>();
        [BindProperty]
        public string CategoryNew { get; set; }
        [BindProperty]
        public string CategoryOld { get; set; }
        [BindProperty]
        public string ToBeDeletedCategory { get; set; }
        public CategoriesModel(IHttpClientFactory client)
        {
            _httpClientFactory = client;
            _httpClient = _httpClientFactory.CreateClient("recipe");
        }

        // Get all categories
        public async Task OnGet()
        {
            var httpResponseMessage =
                 await _httpClient.GetAsync($"/categories");
            bool isRequestSucceed = httpResponseMessage.IsSuccessStatusCode;
            var categoryData = await httpResponseMessage.Content.ReadAsStringAsync();
            Categories = JsonSerializer.Deserialize<List<string>>(categoryData);
        }

        // Add new category
        public async Task<IActionResult> OnPostAdd()
        {
            var categoryItemJson = new StringContent(
                        JsonSerializer.Serialize(CategoryNew),
                        Encoding.UTF8,
                        "application/json");

            using var httpResponseMessage =
                await _httpClient.PostAsync("/category", categoryItemJson);

            httpResponseMessage.EnsureSuccessStatusCode();
            return RedirectToPage("Categories");
        }

        // Edit a category
        public async Task<IActionResult> OnPostUpdate()
        {
            if (CategoryNew != CategoryOld)
            {
                var newCategoryJson = new StringContent(
                    JsonSerializer.Serialize(CategoryNew),
                    Encoding.UTF8,
                    "application/json");

                using var httpResponseMessage =
                    await _httpClient.PutAsync($"/category/{CategoryOld}", newCategoryJson);
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            return RedirectToPage("Categories");
        }

        public async Task<IActionResult> OnPostDelete()
        {
            using var httpResponseMessage =
                await _httpClient.DeleteAsync($"/category/{ToBeDeletedCategory}");

            httpResponseMessage.EnsureSuccessStatusCode();
            return RedirectToPage("Categories");
        }
    }
}

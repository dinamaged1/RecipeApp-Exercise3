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
        public List<string> Categories { get; set; }=new List<string>();
        [BindProperty]
        public string CategoryNew { get; set; }
        [BindProperty]
        public string CategoryOld { get; set; }
        public CategoriesModel(IHttpClientFactory client)
        {
            _httpClientFactory = client;
            _httpClient = _httpClientFactory.CreateClient("recipe");
        }
        public async Task OnGet()
        {
            var httpResponseMessage =
                 await _httpClient.GetAsync($"/categories");
            bool isRequestSucceed = httpResponseMessage.IsSuccessStatusCode;
            var categoryData = await httpResponseMessage.Content.ReadAsStringAsync();
            Categories = JsonSerializer.Deserialize<List<string>>(categoryData);
        }

        public async Task<IActionResult> OnPostUpdate()
        {
            var newCategoryJson = new StringContent(
                JsonSerializer.Serialize(CategoryNew),
                Encoding.UTF8,
                "application/json");

            using var httpResponseMessage =
                await _httpClient.PutAsync($"/category/{CategoryOld}", newCategoryJson);
            await OnGet();
            httpResponseMessage.EnsureSuccessStatusCode();
            return Page();
        }
    }
}

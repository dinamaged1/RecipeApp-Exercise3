using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using RazorPagesRecipes.Models;

namespace RazorPagesRecipes.Pages.Recipes
{
    public class RecipesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private IWebHostEnvironment _host;
        public List<string> Categories { get; set; } = new List<string>();
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
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

            httpResponseMessage =
                await _httpClient.GetAsync($"/recipes");
            httpResponseMessage.EnsureSuccessStatusCode();
            var recipeData = httpResponseMessage.Content.ReadAsStringAsync().Result;

            Categories = JsonSerializer.Deserialize<List<string>>(categoryData);
            Recipes = JsonSerializer.Deserialize<List<Recipe>>(recipeData);

            IsCheckedCategory = new List<bool>();
            Categories.ForEach(x => IsCheckedCategory.Add(false));
        }

        public async Task<IActionResult> OnPostAdd()
        {
            Guid id = Guid.NewGuid();

            // Get the image uploaded 
            // Save it to RecipeImages folder
            MemoryStream ms = new MemoryStream();
            await RecipeImage.OpenReadStream().CopyToAsync(ms);
            var data = ms.ToArray();
            var filePath = $"{_host.WebRootPath}/RecipesImages/{id}.{System.IO.Path.GetExtension(RecipeImage.FileName)}";
            System.IO.File.WriteAllBytes(filePath, data);

            string recipeTitle = Request.Form["title"];
            string recipeInstructionsInput = Request.Form["instructions"];
            string recipeIngredientsInput = Request.Form["ingredients"];
            List<string> ingredientsList = recipeInstructionsInput.Split('\n').ToList();
            List<string> instructionsList = recipeIngredientsInput.Split('\n').ToList();
            string imagePath = $"../RecipesImages/{id}.{System.IO.Path.GetExtension(RecipeImage.FileName)}";
            List<string> categoryList = new List<string>();
            for (int i = 0; i < IsCheckedCategory.Count; i++)
            {
                if (IsCheckedCategory[i] == true)
                {
                    categoryList.Add(Categories[i]);
                }
            }
            Recipe newRecipe = new Recipe(id, recipeTitle, imagePath, instructionsList, ingredientsList, categoryList);
            var recipeItemJson = new StringContent(JsonSerializer.Serialize(newRecipe), Encoding.UTF8, "application/json");
            using var httpResponseMessage = await _httpClient.PostAsync("/recipe", recipeItemJson);
            await OnGet();
            return Page();
        }
    }
}

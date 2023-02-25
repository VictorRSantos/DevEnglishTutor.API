using DevEnglishTutor.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevEnglishTutor.API.Controllers
{
    [ApiController]
    [Route("api/english-tutor")]
    public class EnglishTutorController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public EnglishTutorController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // api/english-tutor?text=
        [HttpGet]
        public async Task<IActionResult> Get(string text, [FromServices] IConfiguration configuration)
        {
            // Pegar o Token
            var token = configuration.GetValue<string>("ChatGptSecretKey");
            // Autorização
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var model = new ChatGptInputModel(text);

            var requestBody = JsonSerializer.Serialize(model);

            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/completions", content);

            var result = await response.Content.ReadFromJsonAsync<ChatGptViewModel>();

            var promptResponse = result?.choices.First();


            return Ok(promptResponse?.text?.Replace("\n","").Replace("\t",""));
        }



    }
}

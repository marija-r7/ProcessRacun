using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using ProcessRacun.Processing;

namespace ProcessRacun.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestRacunController : ControllerBase
    {
        [HttpGet]
        public JsonObject GetRacunAmount(string scannedLink)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(scannedLink);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = httpClient.GetAsync(scannedLink).Result;
            if (response.IsSuccessStatusCode)
            {
                var resp = response.Content.ReadAsStringAsync().Result;
                JsonObject JO = new JsonObject();
                JO = ProcessString.ParseResponseAsJsonObj(response);
                return JO;
            }
            else
            {
                Console.WriteLine("{0} {1}", (int)response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }
    }
}

using DSP.Gateway.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DSP.Gateway.Sevices
{
    public class ImageHttpService
    {
        public HttpClient Client { get; }

        public ImageHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("http://localhost:5303/DSP/ImageDeliveryService/Image/");
            Client = client;
        }

        public async Task<ImageDTO> GetImage(string id)
        {
            var response = await Client.GetAsync($"full/{id}");
            var image = JsonConvert.DeserializeObject<ImageDTO>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return image;
        }

        public async Task<ImageDTO> GetImageThumb(string id)
        {
            var response = await Client.GetAsync($"thumb/{id}");
            var image = JsonConvert.DeserializeObject<ImageDTO>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return image;
        }
    }
}

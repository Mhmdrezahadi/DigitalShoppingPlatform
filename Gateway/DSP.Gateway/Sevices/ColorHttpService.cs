
using DSP.Gateway.Data;

namespace DSP.Gateway.Sevices
{
    public class ColorHttpService 
    {
        public HttpClient Client { get; }
        public ColorHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("Color Base API Address");
            Client = client;
        }
        public bool AddColor(ProductColorDTO color)
        {
            throw new NotImplementedException();
        }

        public bool RemoveColor(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool EditColor(ProductColorDTO color)
        {
            throw new NotImplementedException();
        }

        public List<ProductColorDTO> GetColorsList()
        {
            throw new NotImplementedException();
        }
    }
}


using DSP.Gateway.Data;
using DSP.Gateway.Data.DTO.Other;

namespace DSP.Gateway.Sevices
{
    public class PaymentHttpService 
    {
        public HttpClient Client { get; }
        public PaymentHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("http://localhost:5301/DSP/ProductService");
            Client = client;
        }
        public List<SupportedPayment> GetSupportedPayments()
        {
            throw new NotImplementedException();
        }

        public Task<RequestForPayResponse> RequestForPay(Guid? paymentGateWayId, BasketDTO dbBasket, string TrackingCode)
        {
            throw new NotImplementedException();
        }

        public List<UserPaymentToReturnDTO> UserPayments(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<VerificationResponse> VerifyPayment(PaymentDTO payment)
        {
            throw new NotImplementedException();
        }
    }
}

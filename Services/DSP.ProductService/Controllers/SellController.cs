using DSP.ProductService.Data;
using DSP.ProductService.Services;
using DSP.ProductService.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DSP.ProductService.Controllers
{
    [ApiController]
    [Route("DSP/ProductService/Sell")]
    public class SellController : ControllerBase
    {
        private readonly ISellService _sellService;
        private readonly IManageService _manageService;
        public SellController(ISellService sellService, IManageService manageService)
        {
            _sellService = sellService;
            _manageService = manageService;
        }

        [HttpGet("MyDeviceList/{userId}")]
        public async Task<ActionResult<List<FastPricingToReturnDTO>>> MyDeviceList(Guid userId)
        {
            List<FastPricingToReturnDTO> ls = await _sellService.MyDeviceList(userId);

            return Ok(ls);
        }

        [HttpGet("MyDevice/{userId}/{Id}")]
        public async Task<ActionResult<FastPricingToReturnDTO>> MyDevice(Guid userId, Guid id)
        {
            FastPricingToReturnDTO dto = await _sellService.MyDevice(userId, id);

            return Ok(dto);
        }

        [HttpGet("SellRequest")]
        public async Task<ActionResult<PagedList<SellRequestToReturnDTO>>> SellRequestList([FromQuery] PaginationParams<SellRequestSearch> pagination)
        {
            PagedList<SellRequestToReturnDTO> ls = await _manageService.SellRequestList(pagination);

            return Ok(ls);
        }

        [HttpGet("SellRequest/{id}")]
        public async Task<ActionResult<SellRequestToReturnDTO>> SellRequest(Guid id)
        {
            SellRequestToReturnDTO dto = await _manageService.SellRequest(id);

            return Ok(dto);
        }

        [HttpGet("DeviceInSellRequest/{reqId}")]
        public async Task<ActionResult<FastPricingToReturnDTO>> DeviceInSellRequest(Guid reqId)
        {

            FastPricingToReturnDTO dto = await _manageService.DeviceInSellRequest(reqId);

            return Ok(dto);
        }

        [HttpPut("SellRequest/{id}")]
        public async Task<ActionResult<bool>> ChangeSellRequestStatus(Guid id, SellRequestStatusDTO dto)
        {
            bool res = await _manageService.ChangeSellRequestStatus(id, dto);

            return Ok(res);
        }

        [HttpGet("SellRequestStatusCount")]
        public ActionResult<List<SellRequestStatusCountDTO>> SellRequestStatusCount()
        {
            List<SellRequestStatusCountDTO> ls = _sellService.SellRequestStatusCount();

            return Ok(ls);
        }
        [HttpGet("FastPricingKeysAndValues/{catId}")]
        public async Task<ActionResult<List<FastPricingKeysAndDDsToReturnDTO>>> FastPricingKeysAndValues(Guid catId)
        {
            List<FastPricingKeysAndDDsToReturnDTO> ls = await _sellService.FastPricingKeysAndValues(catId);

            return Ok(ls);
        }

        [HttpGet("DeviceKeysAndValues/{catId}")]
        public async Task<ActionResult<List<FastPricingKeysAndDDsToReturnDTO>>> DeviceKeysAndValues(Guid catId)
        {
            List<FastPricingKeysAndDDsToReturnDTO> ls = await _sellService.FastPricingKeysAndValues(catId);

            return Ok(ls);
        }

        [HttpPost("Device/{userId}")]
        public async Task<ActionResult<Guid>> AddDevice(Guid userId, FastPricingForCreateDTO dto)
        {
            Guid resultId = await _sellService.AddDevice(userId, dto);
            return Ok(resultId);
        }

        [HttpDelete("Device/{userId}/{deviceId}")]
        public ActionResult<bool> RemoveDevice(Guid userId, Guid deviceId)
        {
            bool result = _sellService.RemoveDevice(deviceId, userId);

            return Ok(result);
        }

        [HttpPut("Device/{userId}/{deviceId}")]
        public ActionResult<bool> UpdateDevice(Guid userId, Guid deviceId, FastPricingForCreateDTO dto)
        {
            bool result = _sellService.UpdateDevice(deviceId, userId, dto);

            return Ok(result);
        }

        [HttpPost("FastPricingValues")]
        public async Task<ActionResult<FastPricingToReturnDTO>> FastPricingValues(FastPricingForCreateDTO dto)
        {
            FastPricingToReturnDTO res = await _sellService.FastPricingValues(dto);

            return Ok(res);
        }

        [HttpPost("SellRequest")]
        public async Task<ActionResult<bool>> SellRequest(SellRequestDTO dto)
        {
            bool res = await _sellService.SellRequest(dto);

            return Ok(res);
        }

        [HttpGet("MySellRequests/{userId}")]
        public async Task<ActionResult<List<SellRequestToReturnDTO>>> MySellRequests(Guid userId)
        {
            List<SellRequestToReturnDTO> ls = await _sellService.MySellRequests(userId);

            return Ok(ls);
        }
    }
}

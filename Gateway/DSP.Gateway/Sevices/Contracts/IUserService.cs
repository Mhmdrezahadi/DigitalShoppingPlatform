using DSP.Gateway.Data;
using DSP.Gateway.Utilities;

namespace DSP.Gateway.Sevices
{
    public interface IUserService
    {
        Task<LoginResultDTO> Auth(AuthDTO dto);
        Task<OtpResponseDTO> GetOtp(string mobileNumber);
        Task<bool> IsProfileCompleted(Guid userId);
        Task<bool> SetProfile(Guid userId, ProfileToUpdateDTO dto);
        Task<ProfileToReturnDTO> GetProfile(Guid userId);
        Task<List<string>> ProfilePicture(Guid userId, IFormFile img);
        Task<bool> LikeProduct(Guid userId, Guid productId);
        Task<bool> IsProductLikedByMe(Guid userId, Guid productId);
        Task<bool> UnLikeProduct(Guid userId, Guid productId);
        Task<int> LikedProductsCount(Guid userId);
        Task<List<ProvinceDTO>> GetStatesList();
        Task<List<CityDTO>> GetCitiesOfState(Guid stateId);
        Task<List<AddressToReturnDTO>> UserAdressList(Guid userId);
        Task<Guid> PostAddress(Guid userId, AddressForCreateDTO dto);
        Task<bool> DeleteAddress(Guid userId, Guid addressId);
        Task<bool> EditAddress(Guid userId, AddressToReturnDTO dto);
        Task<LoginResultDTO> Login(UserForLoginDTO user);
        Task<PagedList<UserToReturnDTO>> GetUsers(PaginationParams<UserSearch> pagination);
        Task<UserToReturnDTO> GetUser(Guid userId);
        Task<bool> LikeOrUnlike(Guid userId, Guid productId, bool action);
        Task<LoginResultDTO> RefreshToken(Guid userId);
    }
}

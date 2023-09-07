using DSP.Gateway.Data;
using DSP.Gateway.Entities;
using DSP.Gateway.Utilities;
using Kavenegar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Pipelines;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;

namespace DSP.Gateway.Sevices
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly UserDbContext _dbContext;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _otpCache;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IMemoryCache otpCache,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            UserDbContext dbContext,
            IWebHostEnvironment env,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _env = env;
            _otpCache = otpCache;
            _logger = logger;
        }
        public string JwtTokenCreator(List<Claim> claims, DateTime? time)
        {
            var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Audience"],
                claims,
                expires: time,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<LoginResultDTO> RefreshToken(Guid userId)
        {
            var result = new LoginResultDTO
            {
                Message = "خطا در ورود"
            };

            var userFromDB = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,
                userFromDB.Id.ToString()),
                new Claim(ClaimTypes.Name,
                userFromDB.UserName),
            };

            var roles = await _userManager.GetRolesAsync(userFromDB).ConfigureAwait(false);

            claims.AddRange(
                roles.ToList()
                .Select(role =>
                new Claim(
                    ClaimsIdentity.DefaultRoleClaimType,
                    role)));

            var access_token = JwtTokenCreator(claims, DateTime.Now.AddDays(7));

            var refresh_token = JwtTokenCreator(claims, DateTime.Now.AddMonths(7));

            result = new LoginResultDTO
            {
                IsAuthenticated = true,
                Roles = roles.ToList(),
                Message = "ورود موفق",
                Access_Token = access_token,
                Refresh_Token = refresh_token,
                FirstName = userFromDB.FirstName,
                LastName = userFromDB.LastName,
                UserId = userFromDB.Id,
                SnapShot = userFromDB.SnapShot,
            };

            return result;
        }

        // admin login
        public async Task<LoginResultDTO> Login(UserForLoginDTO user)
        {
            var result = new LoginResultDTO
            {
                Message = "خطا در ورود"
            };

            var auth = await _signInManager.PasswordSignInAsync(
                user.UserName,
                user.PassWord,
                false,
                false).ConfigureAwait(false);

            if (auth.Succeeded)
            {
                var userFromDB = await _userManager.FindByNameAsync(user.UserName).ConfigureAwait(false);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,
                    userFromDB.Id.ToString()),
                    new Claim(ClaimTypes.Name,
                    userFromDB.UserName),
                };

                var roles = await _userManager.GetRolesAsync(userFromDB).ConfigureAwait(false);

                claims.AddRange(
                    roles.ToList()
                    .Select(role =>
                    new Claim(
                        ClaimsIdentity.DefaultRoleClaimType,
                        role)));
                var access_token = JwtTokenCreator(claims, DateTime.Now.AddDays(7));

                var refresh_token = JwtTokenCreator(claims, DateTime.Now.AddMonths(7));

                result = new LoginResultDTO
                {
                    IsAuthenticated = true,
                    Roles = roles.ToList(),
                    Message = "ورود موفق",
                    Access_Token = access_token,
                    Refresh_Token = refresh_token,
                    FirstName = userFromDB.FirstName,
                    LastName = userFromDB.LastName,
                    UserId = userFromDB.Id,
                    SnapShot = userFromDB.SnapShot,
                };
            }

            return result;
        }

        // user(customer) login
        public async Task<LoginResultDTO> Auth(AuthDTO dto)
        {
            var result = new LoginResultDTO
            {
                Message = "خطا در ورود"
            };

            _otpCache.TryGetValue(dto.MobileNumber, out string password);

            if (password != dto.VerificationCode)
            {
                return result;
            }

            var auth = await _signInManager.PasswordSignInAsync(
                dto.MobileNumber,
                dto.VerificationCode,
                false,
                false).ConfigureAwait(false);


            if (auth.Succeeded)
            {
                var userFromDB = await _userManager
                    .FindByNameAsync(dto.MobileNumber).ConfigureAwait(false);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,
                    userFromDB.Id.ToString()),
                    new Claim(ClaimTypes.Name,
                    userFromDB.UserName),
                };

                var roles = await _userManager.GetRolesAsync(userFromDB).ConfigureAwait(false);

                claims.AddRange(
                    roles.ToList()
                    .Select(role =>
                    new Claim(
                        ClaimsIdentity.DefaultRoleClaimType,
                        role)));

                var access_token = JwtTokenCreator(claims, DateTime.Now.AddYears(2));
                var refresh_tokne = JwtTokenCreator(claims, DateTime.Now.AddYears(3));

                result = new LoginResultDTO
                {
                    IsAuthenticated = true,
                    Roles = roles.ToList(),
                    Message = "ورود موفق",
                    Access_Token = access_token,
                    Refresh_Token = refresh_tokne,
                    FirstName = userFromDB.FirstName,
                    LastName = userFromDB.LastName,
                    UserId = userFromDB.Id,
                    SnapShot = userFromDB.SnapShot,
                };
                // kill password
                await _userManager.RemovePasswordAsync(userFromDB);
            }
            return result;
        }

        public async Task<OtpResponseDTO> GetOtp(string mobileNumber)
        {
            if (mobileNumber.Length != 11)
            {
                return new OtpResponseDTO
                {
                    Success = false,
                    Message = "شماره تلفن باید 11 رقم و با 0 شروع شود.",
                };
            }

            //var smsApi = new KavenegarApi("SmsApiKey");
            var random = new Random();
            var verificationCode = random.Next(1111, 9999).ToString();
            //var result = smsApi.Send("", mobileNumber, $"کد تایید تل بال: {verificationCode}");

            SendResult result;
            if (!_env.IsDevelopment())
            {
                //todo //surround with try catch
                try
                {
                    // result = smsApi.VerifyLookup(mobileNumber, verificationCode, "MobifyOtp");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);

                    return new OtpResponseDTO
                    {
                        Success = false,
                        Message = "خطا در ارسال پیامک"
                    };
                }
            }
            else
            {
                verificationCode = "1111";
                result = new SendResult
                {
                    Cost = 250,
                    Date = 22255151,
                    Message = "کد : 1111",
                    Messageid = 1,
                    Receptor = mobileNumber,
                    Sender = "technical team",
                    Status = 1,
                    StatusText = "ارسال به مخابرات",
                };
            }

            var userFromDb = await _userManager
                .FindByNameAsync(mobileNumber);

            _otpCache.Set(
                mobileNumber,
                verificationCode,
                DateTimeOffset.Now.AddMinutes(2));

            if (userFromDb != null)
            {
                await _userManager.RemovePasswordAsync(userFromDb);

                await _userManager.AddPasswordAsync(userFromDb, verificationCode);
            }
            else
            {
                var registerResult = await _userManager.CreateAsync(new User
                {
                    UserName = mobileNumber,
                    PhoneNumber = mobileNumber,
                    FirstName = "",
                    LastName = "",
                    SnapShot = "",/// default pic path
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                }, verificationCode);
            }

            return new OtpResponseDTO
            {
                Success = true,
                Message = "پیامک ارسال شد"
            };
        }

        public async Task<ProfileToReturnDTO> GetProfile(Guid userId)
        {
            var userFromDb = await _userManager.FindByIdAsync(userId.ToString());
            if (userFromDb == null)
            {
                userFromDb = _dbContext.Users.Where(x => x.Id == userId).FirstOrDefault();

                _logger.LogWarning($"GetProfile api cant find user with id: {userId}");
            }
            ProfileToReturnDTO dto = new ProfileToReturnDTO
            {
                FirstName = userFromDb.FirstName,
                LastName = userFromDb.LastName,
                City = userFromDb.City,
                Province = userFromDb.Province,
                UserId = userFromDb.Id,
                MobileNumber = userFromDb.PhoneNumber,
                Email = userFromDb.Email,
                Image_L = userFromDb.SnapShot.Replace("_M", "_L"),
                Image_M = userFromDb.SnapShot,
                Image_S = userFromDb.SnapShot.Replace("_M", "_S")
            };
            return dto;
        }

        public async Task<bool> IsProfileCompleted(Guid userId)
        {
            var userFromDb = await _userManager.FindByIdAsync(userId.ToString());
            var result = userFromDb.FirstName != "" ? true : false;
            return result;
        }

        public async Task<bool> LikeProduct(Guid userId, Guid productId)
        {

            return false;
        }

        public async Task<bool> UnLikeProduct(Guid userId, Guid productId)
        {

            return false;
        }

        public async Task<bool> LikeOrUnlike(Guid userId, Guid productId, bool action)
        {
            return true;
        }

        public async Task<List<string>> ProfilePicture(Guid userId, IFormFile img)
        {
            return null;
        }

        public async Task<bool> SetProfile(Guid userId, ProfileToUpdateDTO dto)
        {
            var userFromDb = await _userManager
                .FindByIdAsync(userId.ToString());

            userFromDb.FirstName = dto.FirstName;
            userFromDb.LastName = dto.LastName;
            userFromDb.Province = dto.Province;
            userFromDb.City = dto.City;
            userFromDb.Email = dto.Email;

            var res = await _userManager.UpdateAsync(userFromDb);

            return res.Succeeded;
        }

        public async Task<bool> IsProductLikedByMe(Guid userId, Guid productId)
        {
            return true;
        }

        public async Task<int> LikedProductsCount(Guid userId)
        {
            return 1;
        }

        public async Task<List<ProvinceDTO>> GetStatesList()
        {
            return await _dbContext.Provinces
                .Where(x => x.Id == Guid.Parse("c3cff78a-cf37-47cc-b4d3-cef20593a248") ||
                            x.Id == Guid.Parse("6400284c-3ead-4ddb-bb62-79789d6c9a0a"))
                .Select(x => new ProvinceDTO
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<List<CityDTO>> GetCitiesOfState(Guid stateId)
        {
            return await _dbContext.Cities
                .Where(x => x.ProvinceId == stateId)
                .Select(x => new CityDTO
                {
                    Id = x.Id,
                    Name = x.Name
                }).OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<List<AddressToReturnDTO>> UserAdressList(Guid userId)
        {
            return await _dbContext.Addresses
                 .Where(x => x.UserId == userId && x.IsVerified == true)
                 .OrderBy(o => o.CreatedAt)
                 .Select(x => new AddressToReturnDTO
                 {
                     AddressId = x.Id,
                     City = x.City,
                     State = x.State,
                     Label = x.Label,
                     DetailedAddress = x.DetailedAddress,
                     ContactName = x.ContactName,
                     ContactNumber = x.ContactNumber,
                     PostalCode = x.PostalCode,
                     UserId = x.UserId
                 }).ToListAsync();
        }

        public async Task<Guid> PostAddress(Guid userId, AddressForCreateDTO dto)
        {
            var addressCount = _dbContext.Addresses.Where(x => x.Id == userId && x.IsVerified == true).Count();

            if (addressCount > 3)
                throw new PolicyException("user can has maximum 3 active addresses");

            var address = new Address
            {
                Id = Guid.NewGuid(),
                City = dto.City,
                State = dto.State,
                ContactNumber = dto.ContactNumber,
                ContactName = dto.ContactName,
                DetailedAddress = dto.DetailedAddress,
                Label = dto.Label,
                PostalCode = dto.PostalCode,
                UserId = userId,
            };

            _dbContext.Addresses.Add(address);

            if (await _dbContext.SaveChangesAsync() <= 0)
                throw new Exception("cant add address");

            return address.Id;
        }

        public async Task<bool> DeleteAddress(Guid userId, Guid addressId)
        {
            //if (await _dbContext.Orders.AnyAsync(x => x.AddressId == addressId))
            //    return false;

            var existing = await _dbContext.Addresses.FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId);

            _dbContext.Addresses.Remove(existing);

            try
            {
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    existing.IsVerified = false;

                    _dbContext.Addresses.Update(existing);

                    _dbContext.SaveChanges();

                    return true;
                }
                return false;
            }
        }

        public async Task<bool> EditAddress(Guid userId, AddressToReturnDTO dto)
        {
            if (dto.UserId != userId)
                return false;

            var existing = await _dbContext.Addresses.FirstOrDefaultAsync(x => x.Id == dto.AddressId && x.UserId == userId);

            //if (await _dbContext.Orders.AnyAsync(x => x.AddressId == dto.AddressId) ||
            //await _dbContext.SellRequests.AnyAsync(x => x.AddressId == dto.AddressId))
            var b = 1;
            if (1 == b)
            {

                existing.IsVerified = false;
                _dbContext.Addresses.Update(existing);
                _dbContext.SaveChanges();

                _dbContext.Addresses.Add(new Address
                {
                    Label = dto.Label,
                    City = dto.City,
                    State = dto.State,
                    PostalCode = dto.PostalCode,
                    ContactName = dto.ContactName,
                    ContactNumber = dto.ContactNumber,
                    DetailedAddress = dto.DetailedAddress,
                    UserId = userId
                });
                _dbContext.SaveChanges();
                return true;
            }

            existing.City = dto.City;
            existing.State = dto.State;
            existing.ContactNumber = dto.ContactNumber;
            existing.ContactName = dto.ContactName;
            existing.DetailedAddress = dto.DetailedAddress;
            existing.Label = dto.Label;
            existing.PostalCode = dto.PostalCode;

            _dbContext.Addresses.Update(existing);

            return (await _dbContext.SaveChangesAsync() > 0);
        }

        public async Task<PagedList<UserToReturnDTO>> GetUsers(PaginationParams<UserSearch> pagination)
        {
            var query = _userManager.Users
                .Where(x => !x.UserRoles.Any())
                .AsQueryable();

            if (pagination.Query != null)
            {
                if (!string.IsNullOrWhiteSpace(pagination.Query.SearchText))
                {
                    query = query.Where(x => x.FirstName.Contains(pagination.Query.SearchText) ||
                        x.LastName.Contains(pagination.Query.SearchText) ||
                        x.PhoneNumber.Contains(pagination.Query.SearchText) ||
                        x.Email.Contains(pagination.Query.SearchText) ||
                        x.Province.Contains(pagination.Query.SearchText) ||
                        x.City.Contains(pagination.Query.SearchText));
                }
                if (pagination.Query.UserSearchOrder != null)
                {
                    if (pagination.Query.UserSearchOrder == SearchOrder.New)
                        query = query.OrderByDescending(o => o.RegisterDate);
                    else
                        query = query.OrderBy(o => o.RegisterDate);
                }
                if (pagination.Query.StatusSearchOrder != null)
                {
                    if (pagination.Query.StatusSearchOrder == true)
                        query = query.OrderByDescending(o => o.Status);
                    else
                        query = query.OrderBy(o => o.Status);
                }
            }

            var resultQuery = query.Select(x => new UserToReturnDTO
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                City = x.City,
                Province = x.Province,
                MobileNumber = x.PhoneNumber,
                SnapShot = x.SnapShot,
                RegisterDate = x.RegisterDate,
                Status = x.Status
            });

            return await PagedList<UserToReturnDTO>.CreateAsync(resultQuery, pagination.CurrentPage, pagination.PageSize); ;

        }

        public async Task<UserToReturnDTO> GetUser(Guid userId)
        {
            return await _userManager.Users
                .Where(x => !x.UserRoles.Any() && x.Id == userId)
                .Select(x => new UserToReturnDTO
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    City = x.City,
                    Province = x.Province,
                    Email = x.Email,
                    MobileNumber = x.PhoneNumber,
                    SnapShot = x.SnapShot,
                    RegisterDate = x.RegisterDate,
                    Status = x.Status
                }).FirstOrDefaultAsync();
        }


    }
}

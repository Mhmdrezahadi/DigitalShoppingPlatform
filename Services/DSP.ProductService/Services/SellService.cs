
using DSP.ProductService.Data;
using DSP.ProductService.Utilities;
using Microsoft.EntityFrameworkCore;


namespace DSP.ProductService.Services
{
    public class SellService : ISellService
    {
        private readonly ProductServiceDbContext _dbContext;

        public SellService(ProductServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddDevice(Guid userId, FastPricingForCreateDTO dto)
        {
            var device = new Device
            {
                CategoryId = dto.CategoryId,
                UserId = userId,
                IsPriced = true
            };

            DevicePricingVerification(dto.CategoryId, dto);

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.Devices.Add(device);

                    await _dbContext.SaveChangesAsync();

                    var values = dto.FastPricingValues
                        .Select(s => new FastPricingValue
                        {
                            DeviceId = device.Id,
                            FastPricingDDId = s.FastPricingDDId,
                            FastPricingKeyId = s.FastPricingKeyId
                        }).ToList();

                    _dbContext.FastPricingValues.AddRange(values);

                    await _dbContext.SaveChangesAsync();

                    dbContextTransaction.Commit();
                    return device.Id;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }

            }
        }
        public bool RemoveDevice(Guid deviceId, Guid userId)
        {
            var device = _dbContext.Devices.Where(x => x.Id == deviceId && x.UserId == userId).FirstOrDefault();
            if (device == null)
                throw new NotFoundException("Device Not Found");
            device.IsVerified = false;

            _dbContext.Devices.Update(device);

            return (_dbContext.SaveChanges() > 0);
        }

        public bool UpdateDevice(Guid deviceId, Guid userId, FastPricingForCreateDTO dto)
        {
            var deviceWithValues = _dbContext.Devices
                .Where(x => x.Id == deviceId && x.UserId == userId)
                .Include(i => i.FastPricingValues)
                .ThenInclude(i => i.FastPricingKey)
                .ThenInclude(i => i.FastPricingDDs)
                .FirstOrDefault();

            if (deviceWithValues == null)
                throw new NotFoundException("Device Not Found");

            DevicePricingVerification(deviceWithValues.CategoryId, dto);

            foreach (var item in deviceWithValues.FastPricingValues)
            {
                var newItem = dto.FastPricingValues.FirstOrDefault(x => x.FastPricingKeyId == item.FastPricingKeyId);
                item.FastPricingDDId = newItem.FastPricingDDId;

                _dbContext.FastPricingValues.Update(item);
            }

            var deviceKeyIds = deviceWithValues.FastPricingValues.Select(s => s.FastPricingKeyId).ToList();

            var newValues = dto.FastPricingValues
                .Where(x => !deviceKeyIds.Contains(x.FastPricingKeyId))
                .Select(s => new FastPricingValue
                {
                    DeviceId = deviceWithValues.Id,
                    FastPricingKeyId = s.FastPricingKeyId,
                    FastPricingDDId = s.FastPricingDDId
                })
                .ToList();

            _dbContext.FastPricingValues.AddRange(newValues);

            _dbContext.SaveChanges();
            return true;

        }
        public async Task<List<FastPricingKeysAndDDsToReturnDTO>> FastPricingKeysAndValues(Guid catId)
        {
            return await _dbContext.FastPricingDefinitions.Where(x => x.CategoryId == catId)
                .Include(i => i.FastPricingKeys)
                .ThenInclude(i => i.FastPricingDDs)
                .Select(s => s.FastPricingKeys
                    .OrderBy(o => o.CreatedAt)
                    .Select(s => new FastPricingKeysAndDDsToReturnDTO
                    {
                        Name = s.Name,
                        Hint = s.Hint,
                        Section = s.Section,
                        FastPricingKeyId = s.Id,
                        ValueType = s.ValueType,
                        FastPricingDDs = s.FastPricingDDs.OrderBy(o => o.CreatedAt).Select(s => new FastPricingDDsToReturnDTO
                        {
                            Id = s.Id,
                            Label = s.Label,
                            ErrorTitle = s.ErrorTitle,
                            ErrorDiscription = s.ErrorDiscription,
                            OperationType = s.OperationType

                        }).ToList()
                    }).ToList()).FirstOrDefaultAsync();
        }

        public async Task<FastPricingToReturnDTO> FastPricingValues(FastPricingForCreateDTO dto)
        {
            var dtoKeyIds = dto.FastPricingValues.Select(s => s.FastPricingKeyId).ToList();
            var dtoDDIds = dto.FastPricingValues.Select(s => s.FastPricingDDId).ToList();

            var dbDefinition = await _dbContext.FastPricingDefinitions
                .Where(x => x.CategoryId == dto.CategoryId)
                .Include(i => i.Category)
                .Include(i => i.Product)
                .Include(i => i.FastPricingKeys)
                .ThenInclude(i => i.FastPricingDDs)
                .FirstOrDefaultAsync();

            if (dbDefinition == null)
                throw new NotFoundException("there is no implementation of pricing in this categroy");

            // check all keys recieved
            var isKeyIdInDb = dbDefinition.FastPricingKeys
                .Select(s => s.Id)
                .Any(a => !dtoKeyIds.Contains(a));

            if (isKeyIdInDb)
                throw new BadRequestException("all keys should set");

            /// check if an error on dropdown item recieved return badrequest of cant price
            var cantPrice = dbDefinition.FastPricingKeys
                 .SelectMany(s => s.FastPricingDDs)
                 .Where(x => dtoDDIds.Contains(x.Id) && x.OperationType == OperationType.ErrorOnPricing)
                 .Any();

            if (cantPrice)
                throw new BadRequestException("cant pricing");

            var minRates = dbDefinition.FastPricingKeys
                .SelectMany(s => s.FastPricingDDs)
                .Where(x => dtoDDIds.Contains(x.Id) && x.OperationType == OperationType.PercentPricing)
                .Select(x => x.MinRate)
                .ToList();

            var maxRates = dbDefinition.FastPricingKeys
                .SelectMany(s => s.FastPricingDDs)
                .Where(x => dtoDDIds.Contains(x.Id) && x.OperationType == OperationType.PercentPricing)
                .Select(x => x.MaxRate)
                .ToList();

            var refPrice = (double)dbDefinition.Product.Price;


            int maximumPrice = (int)(refPrice - (refPrice * (minRates.Sum() / 100)));
            int minimumPrice = (int)(refPrice - (refPrice * (maxRates.Sum() / 100)));

            //// Round the price
            int maxRefDivide, maxRefMode, minRefDivide, minRefMode;

            //find maxref
            if (maximumPrice.ToString().Length > 4)
            {
                maxRefDivide = 1000;
                maxRefMode = 100;
            }
            else if (maximumPrice.ToString().Length == 4)
            {
                maxRefDivide = 100;
                maxRefMode = 10;
            }
            else
            {
                maxRefDivide = 10;
                maxRefMode = 1;
            }
            //find minref
            if (minimumPrice.ToString().Length > 4)
            {
                minRefDivide = 1000;
                minRefMode = 100;
            }
            else if (minimumPrice.ToString().Length == 4)
            {
                minRefDivide = 100;
                minRefMode = 10;
            }
            else
            {
                minRefDivide = 10;
                minRefMode = 1;
            }

            maximumPrice = maximumPrice / maxRefDivide;
            minimumPrice = minimumPrice / minRefDivide;

            var twoMaxMiddleDigits = maximumPrice % maxRefMode;
            var twoMinMiddleDigits = minimumPrice % minRefMode;


            if (twoMaxMiddleDigits > 50)
            {
                maximumPrice += 100 - twoMaxMiddleDigits;
            }
            else
            {
                maximumPrice -= twoMaxMiddleDigits;
            }

            if (twoMinMiddleDigits > 50)
            {
                minimumPrice += 100 - twoMinMiddleDigits;
            }
            else
            {
                minimumPrice -= twoMinMiddleDigits;
            }

            maximumPrice = maximumPrice * maxRefDivide;
            minimumPrice = minimumPrice * minRefDivide;
            ////////////////////////////////////

            var res = dbDefinition.FastPricingKeys
                .Where(x => x.FastPricingDDs.Any(q => dtoDDIds.Contains(q.Id)))
                .ToList();

            var final = new FastPricingToReturnDTO
            {
                CategoryId = dbDefinition.CategoryId,
                CategoryName = dbDefinition.Category.Name,
                DT = DateTime.Now,
                ImageUrl_L = dbDefinition.Category.ImageUrl_L,
                ImageUrl_M = dbDefinition.Category.ImageUrl_M,
                ImageUrl_S = dbDefinition.Category.ImageUrl_S,
                MaxPrice = maximumPrice,
                MinPrice = minimumPrice,
                FastPricingKeys = res.Select(s => new FastPricingKeysAndDDsToReturnDTO
                {
                    Name = s.Name,
                    Hint = s.Hint,
                    Section = s.Section,
                    ValueType = s.ValueType,
                    FastPricingKeyId = s.Id,
                    FastPricingDDs = s.FastPricingDDs
                    .Where(x => dto.FastPricingValues
                        .Where(x => x.FastPricingKeyId == s.Id)
                        .Select(a => a.FastPricingDDId)
                        .FirstOrDefault() == x.Id)
                    .Select(s => new FastPricingDDsToReturnDTO
                    {
                        Label = s.Label,
                        Id = s.Id
                    }).ToList()
                }).ToList(),
            };

            return final;
        }

        public async Task<FastPricingToReturnDTO> MyDevice(Guid userId, Guid id)
        {
            var myDevice = await _dbContext.Devices
                .Where(x => x.Id == id && x.UserId == userId)
                .Include(x => x.Category)
                .Include(x => x.FastPricingValues)
                .ThenInclude(x => x.FastPricingDD)
                .Include(x => x.FastPricingValues)
                .ThenInclude(x => x.FastPricingKey)
                .ThenInclude(x => x.FastPricingDefinition)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync();


            int minimumPrice = 0;
            int maximumPrice = 0;
            if (myDevice.IsPriced)
            {
                var DeviceKeys = myDevice.FastPricingValues.Select(x => x.FastPricingDDId).ToList();
                var minRates = myDevice.FastPricingValues.Select(s => s.FastPricingDD.MinRate).ToList();
                var maxRates = myDevice.FastPricingValues.Select(s => s.FastPricingDD.MaxRate).ToList();


                var refPrice = (double)myDevice.FastPricingValues
                    .Select(x => x.FastPricingKey.FastPricingDefinition.Product.Price)
                    .FirstOrDefault();

                maximumPrice = (int)(refPrice - (refPrice * (minRates.Sum() / 100)));
                minimumPrice = (int)(refPrice - (refPrice * (maxRates.Sum() / 100)));


                //// Round the price
                int maxRefDivide, maxRefMode, minRefDivide, minRefMode;

                //find maxref
                if (maximumPrice.ToString().Length > 4)
                {
                    maxRefDivide = 1000;
                    maxRefMode = 100;
                }
                else if (maximumPrice.ToString().Length == 4)
                {
                    maxRefDivide = 100;
                    maxRefMode = 10;
                }
                else
                {
                    maxRefDivide = 10;
                    maxRefMode = 1;
                }
                //find minref
                if (minimumPrice.ToString().Length > 4)
                {
                    minRefDivide = 1000;
                    minRefMode = 100;
                }
                else if (minimumPrice.ToString().Length == 4)
                {
                    minRefDivide = 100;
                    minRefMode = 10;
                }
                else
                {
                    minRefDivide = 10;
                    minRefMode = 1;
                }

                maximumPrice = maximumPrice / maxRefDivide;
                minimumPrice = minimumPrice / minRefDivide;

                var twoMaxMiddleDigits = maximumPrice % maxRefMode;
                var twoMinMiddleDigits = minimumPrice % minRefMode;


                if (twoMaxMiddleDigits > 50)
                {
                    maximumPrice += 100 - twoMaxMiddleDigits;
                }
                else
                {
                    maximumPrice -= twoMaxMiddleDigits;
                }

                if (twoMinMiddleDigits > 50)
                {
                    minimumPrice += 100 - twoMinMiddleDigits;
                }
                else
                {
                    minimumPrice -= twoMinMiddleDigits;
                }

                maximumPrice = maximumPrice * maxRefDivide;
                minimumPrice = minimumPrice * minRefDivide;
                ////////////////////////////////////
            }

            var dto = new FastPricingToReturnDTO
            {
                DeviceId = myDevice.Id,
                CategoryId = myDevice.CategoryId,
                DT = DateTime.Now,
                MaxPrice = maximumPrice,
                MinPrice = minimumPrice,
                CategoryName = myDevice.Category.Name,
                ImageUrl_L = myDevice.Category.ImageUrl_L,
                ImageUrl_M = myDevice.Category.ImageUrl_M,
                ImageUrl_S = myDevice.Category.ImageUrl_S,
                IsPriced = myDevice.IsPriced,
                FastPricingKeys = myDevice.FastPricingValues
                .Select(s => s.FastPricingKey)
                .OrderBy(o => o.CreatedAt)
                .Select(x => new FastPricingKeysAndDDsToReturnDTO
                {
                    Name = x.Name,
                    Hint = x.Hint,
                    Section = x.Section,
                    ValueType = x.ValueType,
                    FastPricingKeyId = x.Id,
                    FastPricingDDs = x.FastPricingDDs.OrderBy(o => o.CreatedAt).Select(a => new FastPricingDDsToReturnDTO
                    {
                        Id = a.Id,
                        Label = a.Label
                    }).ToList()
                }).ToList()
            };

            return dto;
        }

        public async Task<List<FastPricingToReturnDTO>> MyDeviceList(Guid userId)
        {
            var myDevice = await _dbContext.Devices
                .Where(x => x.UserId == userId && x.IsVerified == true)
                .Include(x => x.Category)
                .Include(x => x.FastPricingValues)
                .ThenInclude(x => x.FastPricingDD)
                .Include(x => x.FastPricingValues)
                .ThenInclude(x => x.FastPricingKey)
                .ThenInclude(x => x.FastPricingDefinition)
                .ThenInclude(x => x.Product)
                .ToListAsync();

            List<FastPricingToReturnDTO> ls = new List<FastPricingToReturnDTO>();
            foreach (var item in myDevice)
            {
                if (item.IsPriced)
                {
                    var DeviceKeys = item.FastPricingValues.Select(x => x.FastPricingDDId).ToList();
                    var minRates = item.FastPricingValues.Select(s => s.FastPricingDD.MinRate).ToList();
                    var maxRates = item.FastPricingValues.Select(s => s.FastPricingDD.MaxRate).ToList();


                    var refPrice = (double)item.FastPricingValues
                        .Select(x => x.FastPricingKey.FastPricingDefinition.Product.Price)
                        .FirstOrDefault();


                    int maximumPrice = (int)(refPrice - (refPrice * (minRates.Sum() / 100)));
                    int minimumPrice = (int)(refPrice - (refPrice * (maxRates.Sum() / 100)));

                    //// Round the price
                    int maxRefDivide, maxRefMode, minRefDivide, minRefMode;

                    //find maxref
                    if (maximumPrice.ToString().Length > 4)
                    {
                        maxRefDivide = 1000;
                        maxRefMode = 100;
                    }
                    else if (maximumPrice.ToString().Length == 4)
                    {
                        maxRefDivide = 100;
                        maxRefMode = 10;
                    }
                    else
                    {
                        maxRefDivide = 10;
                        maxRefMode = 1;
                    }
                    //find minref
                    if (minimumPrice.ToString().Length > 4)
                    {
                        minRefDivide = 1000;
                        minRefMode = 100;
                    }
                    else if (minimumPrice.ToString().Length == 4)
                    {
                        minRefDivide = 100;
                        minRefMode = 10;
                    }
                    else
                    {
                        minRefDivide = 10;
                        minRefMode = 1;
                    }

                    maximumPrice = maximumPrice / maxRefDivide;
                    minimumPrice = minimumPrice / minRefDivide;

                    var twoMaxMiddleDigits = maximumPrice % maxRefMode;
                    var twoMinMiddleDigits = minimumPrice % minRefMode;


                    if (twoMaxMiddleDigits > 50)
                    {
                        maximumPrice += 100 - twoMaxMiddleDigits;
                    }
                    else
                    {
                        maximumPrice -= twoMaxMiddleDigits;
                    }

                    if (twoMinMiddleDigits > 50)
                    {
                        minimumPrice += 100 - twoMinMiddleDigits;
                    }
                    else
                    {
                        minimumPrice -= twoMinMiddleDigits;
                    }

                    maximumPrice = maximumPrice * maxRefDivide;
                    minimumPrice = minimumPrice * minRefDivide;
                    ////////////////////////////////////

                    ls.Add(new FastPricingToReturnDTO
                    {
                        DeviceId = item.Id,
                        CategoryId = item.Category.Id,
                        DT = DateTime.Now,
                        IsPriced = item.IsPriced,
                        CategoryName = item.Category.Name,
                        ImageUrl_L = item.Category.ImageUrl_L,
                        ImageUrl_M = item.Category.ImageUrl_M,
                        ImageUrl_S = item.Category.ImageUrl_S,
                        MaxPrice = maximumPrice,
                        MinPrice = minimumPrice,
                    });
                }
            }
            return ls;
        }

        public async Task<List<SellRequestToReturnDTO>> MySellRequests(Guid userId)
        {
            return await _dbContext.SellRequests
                   .Include(i => i.Device)
                   .ThenInclude(i => i.Category)
                   .Where(x => x.Device.UserId == userId)
                   .Select(s => new SellRequestToReturnDTO
                   {
                       Id = s.Id,
                       AgreedPrice = s.AgreedPrice,
                       DT = s.DT,
                       SellRequestStatus = s.SellRequestStatus,
                       StatusDescription = s.StatusDescription,
                       Code = s.Code,
                       Model = new CategoryToReturnDTO
                       {
                           Name = s.Device.Category.Name,
                           ImageUrl_L = s.Device.Category.ImageUrl_L,
                           ImageUrl_M = s.Device.Category.ImageUrl_M,
                           ImageUrl_S = s.Device.Category.ImageUrl_S,
                           CategoryId = s.Device.Category.Id,
                       },
                   }).ToListAsync();
        }

        public async Task<bool> SellRequest(SellRequestDTO dto)
        {
            var device = _dbContext.Devices.Where(x => x.Id == dto.DeviceId).FirstOrDefault();
            if (!device.IsPriced)
                throw new PolicyException("cant request for devices not priced");
            try
            {
                _dbContext.SellRequests.Add(new SellRequest
                {
                    AddressId = dto.AddressId,
                    Id = dto.DeviceId,
                    Code = PID.NewId(),
                    DT = DateTime.Now,
                    SellRequestStatus = SellRequestStatus.InProcess,
                    StatusDescription = ".درخواست شما برای تل بال ارسال شد و کارشناسای ما به زودی برای ادامه ی مراحل با شما تماس خواهند گرفت"
                });
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message.Contains("Cannot insert duplicate key"))
                    {
                        throw new PolicyException("cant send two sell request for one device");
                    }
                }
                throw new AppException(ex.Message);
            }
        }

        public List<SellRequestStatusCountDTO> SellRequestStatusCount()
        {
            var dbOrder = _dbContext.SellRequests
            .AsEnumerable()
            .GroupBy(g => g.SellRequestStatus)
            .Select(s => new SellRequestStatusCountDTO
            {
                SellRequestStatus = s.Select(a => a.SellRequestStatus).FirstOrDefault(),
                Count = s.Count()
            }).ToList();

            return dbOrder;
        }

        public void DevicePricingVerification(Guid catId, FastPricingForCreateDTO dto)
        {
            var dbDefinition = _dbContext.FastPricingDefinitions
            .Where(x => x.CategoryId == catId)
            .Include(i => i.Category)
            .Include(i => i.FastPricingKeys)
            .ThenInclude(i => i.FastPricingDDs)
            .FirstOrDefault();

            if (dbDefinition == null)
                throw new NotFoundException("there is no implementation of pricing in this categroy");

            var dtoKeyIds = dto.FastPricingValues.Select(s => s.FastPricingKeyId).ToList();
            var dtoDDIds = dto.FastPricingValues.Select(s => s.FastPricingDDId).ToList();

            // check all keys recieved
            var isKeyIdInDb = dbDefinition.FastPricingKeys
                .Select(s => s.Id)
                .Any(a => !dtoKeyIds.Contains(a));

            if (isKeyIdInDb)
                throw new BadRequestException("all keys should set");

            // check if an error on dropdown item recieved return badrequest of cant price
            var cantPrice = dbDefinition.FastPricingKeys
                 .SelectMany(s => s.FastPricingDDs)
                 .Where(x => dtoDDIds.Contains(x.Id) && x.OperationType == OperationType.ErrorOnPricing)
                 .Any();
        }

    }
}

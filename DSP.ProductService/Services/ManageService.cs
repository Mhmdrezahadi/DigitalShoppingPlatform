
using DSP.ProductService.Data;
using DSP.ProductService.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ValueType = DSP.ProductService.Data.ValueType;

namespace DSP.ProductService.Services
{
    public class ManageService : IManageService
    {
        private readonly ProductServiceDbContext _dbContext;
        private readonly ILogger<ManageService> _logger;

        public ManageService(ProductServiceDbContext dbContext, ILogger<ManageService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> AddToPropertyKeys(ProductKeysDefinitionsDTO dto)
        {
            return await DefinePropertyKes(dto);
        }

        public async Task AddToFastPricingDefinition(List<FastPricingKeysAndDDsToCreateDTO> dtoList, Guid definitionId)
        {
            foreach (var key in dtoList)
            {
                if (key.ValueType == ValueType.Boolean)
                {
                    if (key.FastPricingDDs.Count() != 2)
                    {
                        throw new BadRequestException(BadRequest.PricingConditionsError.ToString());
                    }
                }


                var keyId = Guid.NewGuid();
                _dbContext.FastPricingKeys.Add(new FastPricingKey
                {
                    Id = keyId,
                    Name = key.Name,
                    Hint = key.Hint,
                    ValueType = key.ValueType,
                    Section = key.Section,
                    FastPricingDefinitionId = definitionId
                });

                await _dbContext.SaveChangesAsync();


                foreach (var dd in key.FastPricingDDs)
                {
                    var opType = OperationType.NoAffectOnPricing;

                    if ((dd.MinRate != null && dd.MaxRate == null) ||
                        (dd.MinRate == null && dd.MaxRate != null))
                    {
                        throw new BadRequestException(BadRequest.PricingRateInCorrectFormat.ToString());
                    }

                    else if ((dd.ErrorTitle != null && dd.ErrorDiscription == null) ||
                       (dd.ErrorTitle == null && dd.ErrorDiscription != null))
                    {
                        throw new BadRequestException(BadRequest.PricingErrorInCorrectFormat.ToString());
                    }

                    else if ((dd.MinRate != null && dd.MaxRate != null && dd.ErrorTitle != null && dd.ErrorDiscription != null))
                    {
                        throw new BadRequestException(BadRequest.PricingBothRateAndErrorInCorrectFormant.ToString());
                    }

                    else if ((dd.MinRate != null && dd.MaxRate != null) &&
                        ((dd.MinRate > 100) ||
                        (dd.MaxRate > 100) ||
                        (dd.MaxRate < 0) ||
                        (dd.MaxRate < 0) ||
                        (dd.MinRate > dd.MaxRate)))
                    {
                        throw new BadRequestException(BadRequest.PricingRateError.ToString());
                    }
                    else if (dd.MinRate == null &&
                            dd.MaxRate == null &&
                            dd.ErrorTitle == null &&
                            dd.ErrorDiscription == null)
                    {
                        opType = OperationType.NoAffectOnPricing;
                    }
                    else if (dd.ErrorTitle != null && dd.ErrorDiscription != null)
                        opType = OperationType.ErrorOnPricing;

                    else if (dd.MinRate != null && dd.MaxRate != null)
                        opType = OperationType.PercentPricing;

                    _dbContext.FastPricingDDs.Add(new FastPricingDD
                    {
                        Label = dd.Label,
                        MinRate = dd.MinRate,
                        MaxRate = dd.MaxRate,
                        ErrorTitle = dd.ErrorTitle,
                        ErrorDiscription = dd.ErrorDiscription,
                        OperationType = opType,
                        FastPricingKeyId = keyId,

                    });
                    _dbContext.SaveChanges();
                }
            }
        }

        public async Task<bool> DefineFastPricingKey(FastPricingDefinitionToCreateDTO dto)
        {
            var defId = Guid.NewGuid();

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.FastPricingDefinitions.Add(new FastPricingDefinition
                    {
                        Id = defId,
                        CategoryId = dto.CategoryId,
                        ProductId = dto.ProductId
                    });
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.Message.Contains("Cannot insert duplicate key"))
                        {
                            throw new PolicyException("cant define two pricing in one model");
                        }
                    }
                    throw new AppException(ex.Message);
                }

                await AddToFastPricingDefinition(dto.FastPricingKeysAndDDs.ToList(), defId);

                dbContextTransaction.Commit();
                return true;
            }
        }

        public async Task<bool> EditFastPricing(Guid definitionId, FastPricingDefinitionToCreateDTO dto)
        {

            var dtoKeyIds = dto.FastPricingKeysAndDDs.Select(s => s.FastPricingKeyId).ToList();

            var dbDefinition = await _dbContext.FastPricingDefinitions
                .Where(x => x.Id == definitionId)
                .Include(s => s.FastPricingKeys)
                .ThenInclude(i => i.FastPricingDDs)
                .ThenInclude(i => i.FastPricingValues)
                .FirstOrDefaultAsync();

            var exsistingKeys = dbDefinition.FastPricingKeys
                .Where(x => dtoKeyIds.Contains(x.Id))
                .ToList();

            var removedKeys = dbDefinition.FastPricingKeys
                .Where(x => !dtoKeyIds.Contains(x.Id))
                .ToList();

            var addingKeys = dto.FastPricingKeysAndDDs.Where(x => x.FastPricingKeyId == null).ToList();

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {

                dbDefinition.ProductId = dto.ProductId;
                _dbContext.FastPricingDefinitions.Update(dbDefinition);
                _dbContext.SaveChanges();

                if (removedKeys.Count > 0)
                {

                    var removedDDs = removedKeys.SelectMany(x => x.FastPricingDDs).ToList();

                    var removedDDIds = removedKeys.SelectMany(x => x.FastPricingDDs).Select(s => s.Id).ToList();

                    var removedValues = await _dbContext.FastPricingValues
                        .Where(x => removedDDIds.Contains(x.Id))
                        .ToListAsync();

                    _dbContext.FastPricingValues.RemoveRange(removedValues);
                    if (await _dbContext.SaveChangesAsync() > 0)
                    {
                        var affectedDevices = _dbContext.Devices
                            .Where(x => x.FastPricingValues
                                .Any(a => removedValues
                                .Contains(a)))
                            .ToList();

                        foreach (var item in affectedDevices)
                        {
                            item.IsPriced = false;
                            _dbContext.Devices.Update(item);
                        }
                        _dbContext.SaveChanges();
                    }

                    _dbContext.FastPricingDDs.RemoveRange(removedDDs);
                    await _dbContext.SaveChangesAsync();

                    _dbContext.FastPricingKeys.RemoveRange(removedKeys);
                    if (await _dbContext.SaveChangesAsync() <= 0)
                        throw new AppException("cant remove removed keys");
                }
                if (addingKeys.Count > 0)
                {
                    await AddToFastPricingDefinition(addingKeys, definitionId);
                }
                foreach (var item in exsistingKeys)
                {
                    var newValue = dto.FastPricingKeysAndDDs.Where(s => s.FastPricingKeyId == item.Id).FirstOrDefault();

                    item.Name = newValue.Name;
                    item.Section = newValue.Section;
                    item.Hint = newValue.Hint;
                    item.ValueType = newValue.ValueType;

                    _dbContext.FastPricingKeys.Update(item);

                    if (await _dbContext.SaveChangesAsync() <= 0)
                        throw new BadRequestException("cant update existing keys");

                    var existingDDs = item.FastPricingDDs
                        .Where(a => dto.FastPricingKeysAndDDs
                            .Where(x => x.FastPricingKeyId == item.Id)
                            .SelectMany(s => s.FastPricingDDs.Select(t => t.Id))
                            .Contains(a.Id))
                        .ToList();

                    var removedDDs = item.FastPricingDDs
                        .Where(a => !dto.FastPricingKeysAndDDs
                            .Where(x => x.FastPricingKeyId == item.Id)
                            .SelectMany(s => s.FastPricingDDs.Select(t => t.Id))
                            .Contains(a.Id))
                        .ToList();

                    var addingDDs = dto.FastPricingKeysAndDDs
                        .Where(x => x.FastPricingKeyId == item.Id)
                        .SelectMany(s => s.FastPricingDDs.Where(a => a.Id == null))
                        .ToList();

                    //check if number of boolean value types is not more than 2
                    if (item.ValueType == ValueType.Boolean)
                    {
                        if (item.FastPricingDDs.Count() != 2)
                        {
                            throw new BadRequestException(BadRequest.PricingConditionsError.ToString());
                        }
                    }
                    foreach (var dd in existingDDs)
                    {
                        var newDD = dto.FastPricingKeysAndDDs
                                .Where(x => x.FastPricingKeyId == item.Id)
                                .Select(s => s.FastPricingDDs
                                    .Where(x => x.Id == dd.Id)
                                    .FirstOrDefault())
                                .FirstOrDefault();

                        var opType = OperationType.NoAffectOnPricing;

                        if ((newDD.MinRate != null && newDD.MaxRate == null) ||
                                               (newDD.MinRate == null && newDD.MaxRate != null))
                        {
                            throw new BadRequestException(BadRequest.PricingRateInCorrectFormat.ToString());
                        }

                        else if ((newDD.ErrorTitle != null && newDD.ErrorDiscription == null) ||
                           (newDD.ErrorTitle == null && newDD.ErrorDiscription != null))
                        {
                            throw new BadRequestException(BadRequest.PricingErrorInCorrectFormat.ToString());
                        }

                        else if ((newDD.MinRate != null && newDD.MaxRate != null && newDD.ErrorTitle != null && newDD.ErrorDiscription != null))
                        {
                            throw new BadRequestException(BadRequest.PricingBothRateAndErrorInCorrectFormant.ToString());
                        }

                        else if (newDD.MinRate != null && newDD.MaxRate != null &&
                            ((newDD.MinRate > 100) ||
                            (newDD.MaxRate > 100) ||
                            (newDD.MaxRate < 0) ||
                            (newDD.MinRate < 0) ||
                            (newDD.MinRate > newDD.MaxRate)))
                        {
                            throw new BadRequestException(BadRequest.PricingRateError.ToString());
                        }
                        else if (newDD.MinRate == null &&
                                newDD.MaxRate == null &&
                                newDD.ErrorTitle == null &&
                                newDD.ErrorDiscription == null)
                        {
                            opType = OperationType.NoAffectOnPricing;
                        }
                        else if (newDD.ErrorTitle != null && newDD.ErrorDiscription != null)
                            opType = OperationType.ErrorOnPricing;

                        else if (newDD.MinRate != null && newDD.MaxRate != null)
                            opType = OperationType.PercentPricing;


                        dd.Label = newDD.Label;
                        dd.MaxRate = newDD.MaxRate;
                        dd.MinRate = newDD.MinRate;
                        dd.OperationType = opType;
                        dd.ErrorTitle = newDD.ErrorTitle;
                        dd.ErrorDiscription = newDD.ErrorDiscription;

                        _dbContext.FastPricingDDs.Update(dd);

                        if (await _dbContext.SaveChangesAsync() <= 0)
                            throw new BadRequestException("cant update DDs of existing keys");
                    }

                    if (removedDDs.Count > 0)
                    {
                        var removedDDIds = removedDDs.Select(x => x.Id).ToList();

                        var removedValues = await _dbContext.FastPricingValues
                        .Where(x => removedDDIds.Contains(x.Id))
                        .ToListAsync();

                        _dbContext.FastPricingValues.RemoveRange(removedValues);

                        if (await _dbContext.SaveChangesAsync() > 0)
                        {
                            var affectedDevices = _dbContext.Devices
                                .Where(x => x.FastPricingValues
                                    .Any(a => removedValues
                                    .Contains(a)))
                                .ToList();

                            foreach (var device in affectedDevices)
                            {
                                device.IsPriced = false;
                                _dbContext.Devices.Update(device);
                            }
                            _dbContext.SaveChanges();
                        }

                        _dbContext.FastPricingDDs.RemoveRange(removedDDs);
                        await _dbContext.SaveChangesAsync();
                    }
                    if (addingDDs.Count > 0)
                    {
                        List<FastPricingDD> ddList = new List<FastPricingDD>();
                        foreach (var dd in addingDDs)
                        {
                            var opType = OperationType.NoAffectOnPricing;

                            if ((dd.MinRate != null && dd.MaxRate == null) ||
                                (dd.MinRate == null && dd.MaxRate != null))
                            {
                                throw new BadRequestException(BadRequest.PricingRateInCorrectFormat.ToString());
                            }

                            else if ((dd.ErrorTitle != null && dd.ErrorDiscription == null) ||
                               (dd.ErrorTitle == null && dd.ErrorDiscription != null))
                            {
                                throw new BadRequestException(BadRequest.PricingErrorInCorrectFormat.ToString());
                            }

                            else if ((dd.MinRate != null && dd.MaxRate != null && dd.ErrorTitle != null && dd.ErrorDiscription != null))
                            {
                                throw new BadRequestException(BadRequest.PricingBothRateAndErrorInCorrectFormant.ToString());
                            }

                            else if ((dd.MinRate != null && dd.MaxRate != null) &&
                                ((dd.MinRate > 100) ||
                                (dd.MaxRate > 100) ||
                                (dd.MaxRate < 0) ||
                                (dd.MaxRate < 0) ||
                                (dd.MinRate > dd.MaxRate)))
                            {
                                throw new BadRequestException(BadRequest.PricingRateError.ToString());
                            }
                            else if (dd.MinRate == null &&
                                    dd.MaxRate == null &&
                                    dd.ErrorTitle == null &&
                                    dd.ErrorDiscription == null)
                            {
                                opType = OperationType.NoAffectOnPricing;
                            }
                            else if (dd.ErrorTitle != null && dd.ErrorDiscription != null)
                                opType = OperationType.ErrorOnPricing;

                            else if (dd.MinRate != null && dd.MaxRate != null)
                                opType = OperationType.PercentPricing;


                            _dbContext.FastPricingDDs.Add(new FastPricingDD
                            {
                                Label = dd.Label,
                                MinRate = dd.MinRate,
                                MaxRate = dd.MaxRate,
                                ErrorTitle = dd.ErrorTitle,
                                ErrorDiscription = dd.ErrorDiscription,
                                OperationType = opType,
                                FastPricingKeyId = item.Id,

                            });
                            _dbContext.SaveChanges();
                        }
                    }
                }
                dbContextTransaction.Commit();
                return true;
                //}
                //catch (Exception ex)
                //{
                //dbContextTransaction.Rollback();
                //throw new AppException(ex.Message);
                //}
            }
        }

        public bool RemoveFastPricingDefinition(Guid id)
        {
            var dbDefinition = _dbContext.FastPricingDefinitions.Where(x => x.Id == id)
                .FirstOrDefault();


            var dbKeys = _dbContext.FastPricingKeys.Where(x => x.FastPricingDefinitionId == id).ToList();

            var dbDDs = _dbContext.FastPricingDDs
                .Where(x => dbKeys.Select(s => s.Id).ToList().Contains(x.FastPricingKeyId.Value))
                .ToList();

            var dbValues = _dbContext.FastPricingValues
                .Where(x => dbDDs.Select(s => s.Id).ToList().Contains(x.FastPricingDDId))
                .ToList();

            _dbContext.FastPricingValues.RemoveRange(dbValues);
            _dbContext.SaveChanges();

            _dbContext.FastPricingDDs.RemoveRange(dbDDs);
            _dbContext.SaveChanges();

            _dbContext.FastPricingKeys.RemoveRange(dbKeys);
            _dbContext.SaveChanges();

            _dbContext.FastPricingDefinitions.Remove(dbDefinition);
            _dbContext.SaveChanges();

            return true;
        }

        public async Task<bool> DefinePropertyKes(ProductKeysDefinitionsDTO dto)
        {
            _dbContext.PropertyKeys
                .AddRange(dto.PropertyKeys.Select(x => new PropertyKey
                {
                    CategoryId = dto.CategoryId,
                    Name = x.Name,
                    KeyType = x.KeyType
                }));

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditPropertyKeys(List<PropertyKeyDTO> list)
        {
            var existings = await _dbContext.PropertyKeys
                 .Where(x => list.Select(s => s.PropertyKeyId).ToList().Contains(x.Id))
                 .ToListAsync();

            foreach (var item in existings)
            {
                var newKey = list.FirstOrDefault(x => x.PropertyKeyId == item.Id);

                item.KeyType = newKey.KeyType;
                item.Name = newKey.Name;

                _dbContext.PropertyKeys.Update(item);
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<FastPricingDefinitionToReturnDTO> FastPricing(Guid id)
        {
            return await _dbContext.FastPricingDefinitions
                .Where(x => x.Id == id)
                .Include(i => i.Category)
                .ThenInclude(i => i.ParentCategory)
                .ThenInclude(i => i.ParentCategory)
                .Include(i => i.Product)
                .Include(i => i.FastPricingKeys)
                .ThenInclude(i => i.FastPricingDDs)
                .Select(s => new FastPricingDefinitionToReturnDTO
                {
                    Id = s.Id,
                    Category = new CategoryToReturnDTO
                    {
                        ArrangeId = s.Category.ParentCategory.ParentCategory.Arrange,
                        CategoryId = s.Category.ParentCategory.ParentCategory.Id,
                        ImageUrl_L = s.Category.ParentCategory.ParentCategory.ImageUrl_L,
                        ImageUrl_S = s.Category.ParentCategory.ParentCategory.ImageUrl_S,
                        ImageUrl_M = s.Category.ParentCategory.ParentCategory.ImageUrl_M,
                        Level = s.Category.ParentCategory.ParentCategory.Level,
                        Name = s.Category.ParentCategory.ParentCategory.Name
                    },
                    Brand = new CategoryToReturnDTO
                    {
                        ArrangeId = s.Category.ParentCategory.Arrange,
                        CategoryId = s.Category.ParentCategory.Id,
                        ImageUrl_L = s.Category.ParentCategory.ImageUrl_L,
                        ImageUrl_S = s.Category.ParentCategory.ImageUrl_S,
                        ImageUrl_M = s.Category.ParentCategory.ImageUrl_M,
                        Level = s.Category.ParentCategory.Level,
                        Name = s.Category.ParentCategory.Name
                    },
                    Model = new CategoryToReturnDTO
                    {
                        ArrangeId = s.Category.Arrange,
                        CategoryId = s.Category.Id,
                        ImageUrl_L = s.Category.ImageUrl_L,
                        ImageUrl_S = s.Category.ImageUrl_S,
                        ImageUrl_M = s.Category.ImageUrl_M,
                        Level = s.Category.Level,
                        Name = s.Category.Name
                    },
                    ProductId = s.ProductId,
                    ProductName = s.Product.ProductName,
                    KeysAndDDs = s.FastPricingKeys.OrderBy(o => o.CreatedAt).Select(a => new FastPricingKeysAndDDsToReturnDTO
                    {
                        FastPricingKeyId = a.Id,
                        Name = a.Name,
                        Section = a.Section,
                        Hint = a.Hint,
                        ValueType = a.ValueType,
                        FastPricingDDs = a.FastPricingDDs.OrderBy(o => o.CreatedAt).Select(b => new FastPricingDDsToReturnDTO
                        {
                            Id = b.Id,
                            Label = b.Label,
                            ErrorTitle = b.ErrorTitle,
                            ErrorDiscription = b.ErrorDiscription,
                            OperationType = b.OperationType,
                            MaxRate = b.MaxRate,
                            MinRate = b.MinRate
                        }).ToList(),
                    }).ToList(),
                }).FirstOrDefaultAsync();

            //return await _dbContext.FastPricingDefinitions.Where(x => x.Id == id)
            //    .Include(i => i.FastPricingKeys)
            //    .ThenInclude(i => i.FastPricingDDs)
            //    .Select(s => s.FastPricingKeys
            //        .OrderBy(o => o.CreatedAt)
            //        .Select(s => new FastPricingKeysAndDDsToReturnDTO
            //        {
            //            Name = s.Name,
            //            Hint = s.Hint,
            //            Section = s.Section,
            //            FastPricingKeyId = s.Id,
            //            ValueType = s.ValueType,
            //            FastPricingDDs = s.FastPricingDDs.OrderBy(o => o.CreatedAt).Select(s => new FastPricingDDsToReturnDTO
            //            {
            //                Id = s.Id,
            //                Label = s.Label,
            //                ErrorTitle = s.ErrorTitle,
            //                ErrorDiscription = s.ErrorDiscription,
            //                OperationType = s.OperationType,
            //                MaxRate = s.MaxRate,
            //                MinRate = s.MinRate
            //            }).ToList()
            //        }).ToList()).FirstOrDefaultAsync();
        }

        public async Task<PagedList<FastPricingDefinitionToReturnDTO>> FastPricingList(PaginationParams<FastPricingSearch> pagination)
        {
            var query = _dbContext.FastPricingDefinitions
                 .Include(s => s.Product)
                 .Include(i => i.Category)
                 .ThenInclude(i => i.ParentCategory)
                 .ThenInclude(i => i.ParentCategory)
                 .AsQueryable();


            if (pagination.Query != null)
            {
                if (!string.IsNullOrWhiteSpace(pagination.Query.SearchText))
                {
                    query = query.Where(x => x.Product.ProductName.Contains(pagination.Query.SearchText));
                }
                if (pagination.Query.CategoryId != null)
                    query = query.Where(x => x.Category.ParentCategory.ParentCategoryId == pagination.Query.CategoryId);
                if (pagination.Query.BrandId != null)
                    query = query.Where(x => x.Category.ParentCategoryId == pagination.Query.BrandId);
                if (pagination.Query.ModelId != null)
                    query = query.Where(x => x.CategoryId == pagination.Query.ModelId);

                if (pagination.Query.PricingSearchOrder != null)
                {
                    if (pagination.Query.PricingSearchOrder == SearchOrder.New)
                        query = query.OrderByDescending(o => o.CreatedAt);
                    else
                        query = query.OrderBy(o => o.CreatedAt);
                }

            }
            var resultQuery = query.Select(s => new FastPricingDefinitionToReturnDTO
            {
                Category = new CategoryToReturnDTO
                {
                    ArrangeId = s.Category.ParentCategory.ParentCategory.Arrange,
                    CategoryId = s.Category.ParentCategory.ParentCategory.Id,
                    ImageUrl_L = s.Category.ParentCategory.ParentCategory.ImageUrl_L,
                    ImageUrl_S = s.Category.ParentCategory.ParentCategory.ImageUrl_S,
                    ImageUrl_M = s.Category.ParentCategory.ParentCategory.ImageUrl_M,
                    Level = s.Category.ParentCategory.ParentCategory.Level,
                    Name = s.Category.ParentCategory.ParentCategory.Name
                },
                Brand = new CategoryToReturnDTO
                {
                    ArrangeId = s.Category.ParentCategory.Arrange,
                    CategoryId = s.Category.ParentCategory.Id,
                    ImageUrl_L = s.Category.ParentCategory.ImageUrl_L,
                    ImageUrl_S = s.Category.ParentCategory.ImageUrl_S,
                    ImageUrl_M = s.Category.ParentCategory.ImageUrl_M,
                    Level = s.Category.ParentCategory.Level,
                    Name = s.Category.ParentCategory.Name
                },
                Model = new CategoryToReturnDTO
                {
                    ArrangeId = s.Category.Arrange,
                    CategoryId = s.Category.Id,
                    ImageUrl_L = s.Category.ImageUrl_L,
                    ImageUrl_S = s.Category.ImageUrl_S,
                    ImageUrl_M = s.Category.ImageUrl_M,
                    Level = s.Category.Level,
                    Name = s.Category.Name
                },
                ProductId = s.ProductId,
                Id = s.Id,
                ProductName = s.Product.ProductName
            });

            return await PagedList<FastPricingDefinitionToReturnDTO>.CreateAsync(resultQuery, pagination.CurrentPage, pagination.CurrentPage);
        }

        public async Task<List<PropertyKeyDTO>> GetPropertyKeys(int catId)
        {
            return await _dbContext.PropertyKeys
                .Where(x => x.CategoryId == catId)
                .OrderBy(o => o.CreatedAt)
                .Select(s => new PropertyKeyDTO
                {
                    PropertyKeyId = s.Id,
                    KeyType = s.KeyType,
                    Name = s.Name
                }).ToListAsync();
        }

        public async Task<bool> RemovePropertyKeys(Guid id)
        {
            var existing = await _dbContext.PropertyKeys
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            var keyInValue = await _dbContext.PropertyValues.Where(x => x.PropertyKeyId == existing.Id)
                 .ToListAsync();

            if (keyInValue.Count > 0)
            {
                _dbContext.PropertyValues.RemoveRange(keyInValue);
                await _dbContext.SaveChangesAsync();
            }

            _dbContext.PropertyKeys.Remove(existing);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<FastPricingToReturnDTO> DeviceInSellRequest(Guid reqId)
        {
            var myDevice = await _dbContext.Devices
              .Where(x => x.Id == reqId)
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


                //var refPrice = myDevice.FastPricingValues
                //    .Select(x => x.FastPricingKey.FastPricingDefinition.Product.Price)
                //    .FirstOrDefault();


                //maximumPrice = refPrice - (refPrice * ((decimal)minRates.Sum() / 100));
                //minimumPrice = refPrice - (refPrice * ((decimal)maxRates.Sum() / 100));

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

        public async Task<PagedList<SellRequestToReturnDTO>> SellRequestList(PaginationParams<SellRequestSearch> pagination)
        {
            var query = _dbContext.SellRequests
                    .Include(i => i.Device)
                    .ThenInclude(i => i.Category)
                    .ThenInclude(i => i.ParentCategory)
                    .ThenInclude(i => i.ParentCategory)
                    .AsQueryable();

            if (pagination.Query != null)
            {
                if (!string.IsNullOrWhiteSpace(pagination.Query.SearchText))
                {
                    //query = query.Where(x =>
                    //    x.Code.Contains(pagination.Query.SearchText) ||
                    //    x.Device.User.FirstName.Contains(pagination.Query.SearchText) ||
                    //    x.Device.User.LastName.Contains(pagination.Query.SearchText));
                }
                if (pagination.Query.SellRequestSearchOrder != null)
                {
                    if (pagination.Query.SellRequestSearchOrder == SearchOrder.New)
                        query = query.OrderByDescending(o => o.CreatedAt);
                    else
                        query = query.OrderBy(o => o.CreatedAt);
                }
                //if (pagination.Query.SellRequestStatus != null)
                //    query = query.Where(x => x.SellRequestStatus == (int)pagination.Query.SellRequestStatus);

                if (pagination.Query.CategoryId != null)
                    query = query.Where(x => x.Device.Category.ParentCategory.ParentCategoryId == pagination.Query.CategoryId);
                if (pagination.Query.BrandId != null)
                    query = query.Where(x => x.Device.Category.ParentCategoryId == pagination.Query.BrandId);
                if (pagination.Query.ModelId != null)
                    query = query.Where(x => x.Device.CategoryId == pagination.Query.ModelId);
            }

            var resultQuery = query.Select(s => new SellRequestToReturnDTO
            {
                Id = s.Id,
                DT = s.DT,
                SellRequestStatus = s.SellRequestStatus,
                Code = s.Code,
                Model = new CategoryToReturnDTO
                {
                    CategoryId = s.Device.Category.Id,
                    ImageUrl_L = s.Device.Category.ImageUrl_L,
                    ImageUrl_M = s.Device.Category.ImageUrl_M,
                    ImageUrl_S = s.Device.Category.ImageUrl_S,
                    Name = s.Device.Category.Name,
                    ArrangeId = s.Device.Category.Arrange,
                    Level = s.Device.Category.Level
                },
                Brand = new CategoryToReturnDTO
                {
                    CategoryId = s.Device.Category.ParentCategory.Id,
                    ImageUrl_L = s.Device.Category.ParentCategory.ImageUrl_L,
                    ImageUrl_M = s.Device.Category.ParentCategory.ImageUrl_M,
                    ImageUrl_S = s.Device.Category.ParentCategory.ImageUrl_S,
                    Name = s.Device.Category.ParentCategory.Name,
                    ArrangeId = s.Device.Category.ParentCategory.Arrange,
                    Level = s.Device.Category.ParentCategory.Level
                },
                Category = new CategoryToReturnDTO
                {
                    CategoryId = s.Device.Category.ParentCategory.ParentCategory.Id,
                    ImageUrl_L = s.Device.Category.ParentCategory.ParentCategory.ImageUrl_L,
                    ImageUrl_M = s.Device.Category.ParentCategory.ParentCategory.ImageUrl_M,
                    ImageUrl_S = s.Device.Category.ParentCategory.ParentCategory.ImageUrl_S,
                    Name = s.Device.Category.ParentCategory.ParentCategory.Name,
                    ArrangeId = s.Device.Category.ParentCategory.ParentCategory.Arrange,
                    Level = s.Device.Category.ParentCategory.ParentCategory.Level
                },
                User = new UserToReturnDTO
                {
                    //Id = s.Device.User.Id,
                    //FirstName = s.Device.User.FirstName,
                    //LastName = s.Device.User.LastName,
                    //MobileNumber = s.Device.User.PhoneNumber,
                    //Province = s.Device.User.Province,
                    //Email = s.Device.User.Email,
                    //City = s.Device.User.City,
                    //SnapShot = s.Device.User.SnapShot
                }
            });

            return await PagedList<SellRequestToReturnDTO>.CreateAsync(resultQuery, pagination.CurrentPage, pagination.PageSize);
        }

        public async Task<SellRequestToReturnDTO> SellRequest(Guid id)
        {
            return await _dbContext.SellRequests.Where(x => x.Id == id)
                //.Include(i => i.Address)
                .Include(i => i.Device)
                .ThenInclude(i => i.Category)
                .ThenInclude(i => i.ParentCategory)
                .ThenInclude(i => i.ParentCategory)
                .Select(s => new SellRequestToReturnDTO
                {
                    Id = s.Id,
                    DT = s.DT,
                    AgreedPrice = s.AgreedPrice,
                    Code = s.Code,
                    StatusDescription = s.StatusDescription,
                    SellRequestStatus = s.SellRequestStatus,
                    Address = new AddressToReturnDTO
                    {
                        //State = s.Address.State,
                        //City = s.Address.City,
                        //ContactName = s.Address.ContactName,
                        //ContactNumber = s.Address.ContactNumber,
                        //DetailedAddress = s.Address.DetailedAddress,
                        //PostalCode = s.Address.PostalCode,
                        //Label = s.Address.Label,
                        //AddressId = s.AddressId,
                        //UserId = s.Device.User.Id
                    },
                    Model = new CategoryToReturnDTO
                    {
                        CategoryId = s.Device.Category.Id,
                        ImageUrl_L = s.Device.Category.ImageUrl_L,
                        ImageUrl_M = s.Device.Category.ImageUrl_M,
                        ImageUrl_S = s.Device.Category.ImageUrl_S,
                        Name = s.Device.Category.Name,
                        ArrangeId = s.Device.Category.Arrange,
                        Level = s.Device.Category.Level
                    },
                    Brand = new CategoryToReturnDTO
                    {
                        CategoryId = s.Device.Category.ParentCategory.Id,
                        ImageUrl_L = s.Device.Category.ParentCategory.ImageUrl_L,
                        ImageUrl_M = s.Device.Category.ParentCategory.ImageUrl_M,
                        ImageUrl_S = s.Device.Category.ParentCategory.ImageUrl_S,
                        Name = s.Device.Category.ParentCategory.Name,
                        ArrangeId = s.Device.Category.ParentCategory.Arrange,
                        Level = s.Device.Category.ParentCategory.Level
                    },
                    Category = new CategoryToReturnDTO
                    {
                        CategoryId = s.Device.Category.ParentCategory.ParentCategory.Id,
                        ImageUrl_L = s.Device.Category.ParentCategory.ParentCategory.ImageUrl_L,
                        ImageUrl_M = s.Device.Category.ParentCategory.ParentCategory.ImageUrl_M,
                        ImageUrl_S = s.Device.Category.ParentCategory.ParentCategory.ImageUrl_S,
                        Name = s.Device.Category.ParentCategory.ParentCategory.Name,
                        ArrangeId = s.Device.Category.ParentCategory.ParentCategory.Arrange,
                        Level = s.Device.Category.ParentCategory.ParentCategory.Level
                    },
                    User = new UserToReturnDTO
                    {
                        //Id = s.Device.User.Id,
                        //FirstName = s.Device.User.FirstName,
                        //LastName = s.Device.User.LastName,
                        //MobileNumber = s.Device.User.PhoneNumber,
                        //Province = s.Device.User.Province,
                        //Email = s.Device.User.Email,
                        //City = s.Device.User.City,
                        //SnapShot = s.Device.User.SnapShot
                    },
                }).FirstOrDefaultAsync();

        }

        public async Task<bool> ChangeSellRequestStatus(Guid id, SellRequestStatusDTO dto)
        {
            var dbSellRequest = await _dbContext.SellRequests.Include(i => i.Device).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (dbSellRequest == null)
                return false;

            dbSellRequest.SellRequestStatus = dto.SellRequestStatus;
            dbSellRequest.StatusDescription = dto.StatusDescription;
            dbSellRequest.AgreedPrice = dto.AgreedPrice;

            _dbContext.SellRequests.Update(dbSellRequest);
            await _dbContext.SaveChangesAsync();

            try
            {
                SendMessageOnChangeStatus(dbSellRequest.Device.UserId, dbSellRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return true;
        }
        public void SendMessageOnChangeStatus(Guid userId, SellRequest req)
        {
            //var dbUser = _dbContext.Users.Where(x => x.Id == userId).FirstOrDefault();
            //var smsApi = new KavenegarApi(_siteSettings.SmsApiKey);

            //smsApi.VerifyLookup(dbUser.PhoneNumber, ".", "", "", req.SellRequestStatus.ToDisplay(), "MobifyChangeOrderStatus");
        }

        public async Task<bool> FAQ(FAQToCreateDTO dto)
        {

            //var latestArrangeId =
            //await _dbContext.FAQs
            //.OrderByDescending(o => o.Arrange)
            //.Select(s => s.Arrange)
            //.FirstOrDefaultAsync();

            //_dbContext.FAQs.Add(new FAQ
            //{
            //    Answer = dto.Answer,
            //    Question = dto.Question,
            //    Arrange = latestArrangeId + 1
            //});

            //await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<FAQToReturnDTO>> ArrangeFAQs(List<int> arrangeIds)
        {
            //var FAQs = await _dbContext.FAQs
            //     .ToListAsync();

            //if (FAQs.Count != arrangeIds.Count ||
            //    FAQs.Select(s => s.Arrange).Any(x => !arrangeIds.Contains(x)))
            //{
            //    throw new BadRequestException("send all arranges");
            //}

            //foreach (var item in FAQs)
            //{
            //    var index = arrangeIds.IndexOf(item.Arrange);
            //    item.Arrange = index + 1;
            //}

            //_dbContext.FAQs.UpdateRange(FAQs);

            //await _dbContext.SaveChangesAsync();

            //return await _dbContext.FAQs.OrderBy(o => o.Arrange)
            //    .Select(s => new FAQToReturnDTO
            //    {
            //        Id = s.Id,
            //        Answer = s.Answer,
            //        Question = s.Question,
            //        ArrnageId = s.Arrange,
            //    }).ToListAsync();
            return null;
        }

        public async Task<bool> RemoveFAQ(Guid id)
        {
            //var dbFAQ = _dbContext.FAQs.Where(x => x.Id == id).FirstOrDefault();

            //if (dbFAQ == null)
            //    throw new NotFoundException($"cant find faq with {id}");

            //var removingArrange = dbFAQ.Arrange;

            //_dbContext.FAQs.Remove(dbFAQ);

            //if (_dbContext.SaveChanges() > 0)
            //{
            //    var upNeedFAQs = await _dbContext.FAQs.Where(x => x.Arrange > removingArrange).ToListAsync();

            //    foreach (var item in upNeedFAQs)
            //    {
            //        item.Arrange--;
            //        _dbContext.FAQs.Update(item);
            //    }

            //    _dbContext.SaveChanges();
            //    return true;
            //}
            return false;
        }

        public async Task<bool> UpdateAboutUs(AppVariableDTO dto)
        {
            //var dbAbout = _dbContext.AppVariables.Where(x => x.Id == 1).FirstOrDefault();

            //dbAbout.AboutUs = dto.Value;

            //await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SecurityAndPrivacy(AppVariableDTO dto)
        {
            //var dbSecAndPrivacy = _dbContext.AppVariables.Where(x => x.Id == 1).FirstOrDefault();

            //dbSecAndPrivacy.SecurityAndPrivacy = dto.Value;

            //await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TermsAndCondition(AppVariableDTO dto)
        {
            //var dbTermsAndCon = _dbContext.AppVariables.Where(x => x.Id == 1).FirstOrDefault();

            //dbTermsAndCon.TermsAndConditions = dto.Value;

            //await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<PagedList<TransactionToReturnDTO>> TransactionList(PaginationParams<TransactionSearch> pagination)
        {
            var query = _dbContext.Payments
                .AsQueryable();

            if (pagination.Query != null)
            {
                if (!string.IsNullOrWhiteSpace(pagination.Query.SearchText))
                {
                    query = query.Where(x => x.TrackingCode.Contains(pagination.Query.SearchText) ||
                                             x.Amount.ToString().Contains(pagination.Query.SearchText) ||
                                             x.GateWayName.Contains(pagination.Query.SearchText));
                                             //x.User.FirstName.Contains(pagination.Query.SearchText) ||
                                             //x.User.LastName.Contains(pagination.Query.SearchText));
                }

                if (pagination.Query.TransactionSearchOrder != null)
                {
                    if (pagination.Query.TransactionSearchOrder == SearchOrder.New)
                        query = query.OrderByDescending(o => o.CreatedAt);
                    else
                        query = query.OrderBy(o => o.CreatedAt);
                }
                if (pagination.Query.PaymentType != null)
                {
                    query = query.Where(x => x.PaymentType == pagination.Query.PaymentType);
                }
                if (pagination.Query.StatusSearchOrder != null)
                {
                    if (pagination.Query.StatusSearchOrder == true)
                        query = query.OrderByDescending(o => o.IsSuccess);
                    else
                        query = query.OrderBy(o => o.IsSuccess);
                }
            }

            var resultQuery = query.Select(s => new TransactionToReturnDTO
            {
                DT = s.DT,
                Id = s.Id,
                Status = s.IsSuccess,
                Code = s.TrackingCode,
                Gate = s.GateWayName,
                Amount = s.Amount,
                PaymentType = s.PaymentType,
                User = new UserToReturnDTO
                {
                    //Id = s.User.Id,
                    //FirstName = s.User.FirstName,
                    //LastName = s.User.LastName,
                    //Email = s.User.Email,
                    //MobileNumber = s.User.PhoneNumber,
                    //Province = s.User.Province,
                    //City = s.User.City,
                    //RegisterDate = s.User.RegisterDate,
                    //SnapShot = s.User.SnapShot
                }
            });

            return await PagedList<TransactionToReturnDTO>.CreateAsync(resultQuery, pagination.CurrentPage, pagination.PageSize);

        }

        public TransactionToReturnDTO GetTransaction(Guid id)
        {
            return _dbContext.Payments
                .Where(x => x.Id == id)
                .Select(s => new TransactionToReturnDTO
                {
                    DT = s.DT,
                    Id = s.Id,
                    Status = s.IsSuccess,
                    Code = s.TrackingCode,
                    Gate = s.GateWayName,
                    Amount = s.Amount,
                    PaymentType = s.PaymentType,
                    User = new UserToReturnDTO
                    {
                        //Id = s.User.Id,
                        //FirstName = s.User.FirstName,
                        //LastName = s.User.LastName,
                        //Email = s.User.Email,
                        //MobileNumber = s.User.PhoneNumber,
                        //Province = s.User.Province,
                        //City = s.User.City,
                        //RegisterDate = s.User.RegisterDate,
                        //SnapShot = s.User.SnapShot
                    }
                }).FirstOrDefault();
        }

        public TransactionItemDTO GetTransactionItems(Guid transactionId)
        {
            var dbTransaction = _dbContext.Payments
                .Where(x => x.Id == transactionId)
                .FirstOrDefault();

            var dto = new TransactionItemDTO();

            dto.TransactionType = dbTransaction.PaymentType;

            if (dbTransaction.PaymentType == PaymentType.Shopping)
            {

                dto.Products = _dbContext.Orders.Where(x => x.Id == dbTransaction.OrderId)
                     .SelectMany(s => s.OrderDetails)
                     .Select(a => new TransactionProductDTO
                     {
                         ProductId = a.Product.Id,
                         ProductName = a.Product.ProductName,
                         About = a.Product.About,
                         Description = a.Product.Description,
                         CategoryId = a.Product.CategoryId,
                         Discount = a.Product.Discount,
                         Code = a.Product.Code,
                         Count = a.Count,
                         ProductType = a.Product.ProductType,
                         Price = a.Amount,
                         Status = a.Product.Status,
                         Color = new ColorDTO
                         {
                             Code = a.Color.Code,
                             Id = a.Color.Id,
                             Name = a.Color.Name
                         },
                         Images = a.Product.Images.Select(b => new ProductImageToReturnDTO
                         {
                             Id = b.Id,
                             ImageUrl_L = b.ImageUrl_L,
                             ImageUrl_M = b.ImageUrl_M,
                             ImageUrl_S = b.ImageUrl_S
                         }).Take(1).ToList(),
                     }).ToList();
            }
            return dto;
        }

        public bool IsModelDefined(int id)
        {
            return _dbContext.FastPricingDefinitions.Where(x => x.CategoryId == id).Any();
        }
    }
}

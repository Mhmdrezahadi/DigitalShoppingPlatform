using DSP.ProductService.Data;
using DSP.ProductService.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DSP.ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductServiceDbContext _dbContext;
        private readonly ICategoryService _categoryService;
        private readonly IManageService _manageService;

        public ProductService(ProductServiceDbContext dbContext, ICategoryService categoryService, IManageService manageService)
        {
            _dbContext = dbContext;
            _categoryService = categoryService;
            _manageService = manageService;
        }

        public async Task<CreatedImageToReturnDTO> AddImage(IFormFile image)
        {
            var fileNameBase = Guid.NewGuid();

            var path = @"wwwroot/Products/";

            var source = image.OpenReadStream();

            //var dtoImage = System.Drawing.Image.FromStream(source);

            var fileNameS = fileNameBase + "_S.png";// + dto.Img.FileName.Split('.').Last();
            var fileNameM = fileNameBase + "_M.png";// + dto.Img.FileName.Split('.').Last();
            var fileNameL = fileNameBase + "_L.png";// + dto.Img.FileName.Split('.').Last();

            //ImageHelper.SaveJpeg(source, 100, 100, path + fileNameS, 60);
            //ImageHelper.SaveJpeg(source, 200, 200, path + fileNameM, 80);
            //ImageHelper.SaveJpeg(source, dtoImage.Height, dtoImage.Width, path + fileNameL, 100);

            var img = new Image
            {
                Id = fileNameBase,
                ImageUrl_L = "/Products/" + fileNameL,
                ImageUrl_M = "/Products/" + fileNameM,
                ImageUrl_S = "/Products/" + fileNameS
            };

            _dbContext.Images.Add(img);
            var succeed = await _dbContext.SaveChangesAsync() > 0;

            if (succeed)
            {
                return new CreatedImageToReturnDTO
                {
                    Id = img.Id,
                    ImageUrl_L = img.ImageUrl_L,
                    ImageUrl_M = img.ImageUrl_M,
                    ImageUrl_S = img.ImageUrl_S
                };
            }
            else
            {
                throw new AppException();
            }
        }

        public async Task<Guid> AddProduct(ProductForCreateDTO dto)
        {
            Guid id = Guid.NewGuid();

            if (dto.ProductDetailDTOs.Count <= 0)
                throw new BadRequestException("The product must have at least one color");


            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.Products.Add(new Product
                    {
                        Id = id,
                        About = dto.About,
                        CategoryId = dto.CategoryId,
                        Code = PID.ProductNewId(),
                        Description = dto.Description,
                        Discount = dto.Discount,
                        Price = dto.Price,
                        ProductName = dto.ProductName,
                        ProductType = dto.ProductType,
                        Status = dto.Status,
                        ProductTestDescription = dto.ProductTestDescription,
                        Warranty = dto.Warranty,
                        ProductDetails = dto.ProductDetailDTOs.Select(s => new ProductDetail
                        {
                            ColorId = s.ColorId,
                            ProductId = id,
                        }).ToList(),
                        //Colors = dto.Colors.Select(s => new Color
                        //{
                        //    Code = s.Code,
                        //    Name = s.Name,
                        //    ProductId = id
                        //}).ToList()
                    });

                    await _dbContext.SaveChangesAsync();

                    var dbImages = _dbContext.Images.Where(x => dto.ImagesIds.Contains(x.Id)).ToList();
                    foreach (var image in dbImages)
                    {
                        image.ProductId = id;
                    }

                    await _dbContext.SaveChangesAsync();

                    var values =
                        dto.PropertyValues
                         .Select(s => new PropertyValue
                         {
                             PropertyKeyId = s.PropertyKeyId,
                             Value = s.Value,
                             ProductId = id,
                         })
                         .ToList();

                    _dbContext.PropertyValues.AddRange(values);

                    await _dbContext.SaveChangesAsync();


                    _dbContext.PriceLogs.Add(new PriceLog
                    {
                        DT = DateTime.Now,
                        Price = dto.Price,
                        ProductId = id
                    });

                    await _dbContext.SaveChangesAsync();

                    dbContextTransaction.Commit();
                    return id;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new AppException(ex.Message);
                }
            }
        }

        public async Task<bool> EditProduct(Guid productId, ProductForCreateDTO dto)
        {
            var existing = await _dbContext.Products
                .Where(x => x.Id == productId)
                .FirstOrDefaultAsync();

            if (existing is null)
            {
                throw new BadRequestException($"there is no product with {productId}");
            }

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (existing.Price != dto.Price)
                    {
                        _dbContext.PriceLogs.Add(new PriceLog
                        {
                            Price = dto.Price,
                            ProductId = productId,
                            DT = DateTime.Now
                        });

                        BasketPriceTracker(productId, dto.Price, dto.Discount);

                        await _dbContext.SaveChangesAsync();
                    }

                    var dbImages = _dbContext.Images.Where(x => dto.ImagesIds.Contains(x.Id)).ToList();

                    foreach (var image in dbImages)
                    {
                        image.ProductId = productId;
                        _dbContext.Images.Update(image);
                    }
                    await _dbContext.SaveChangesAsync();

                    var dbDetails = await _dbContext.ProductDetails
                        .Where(x => x.ProductId == productId)
                        .ToListAsync();

                    var removingDetails = dbDetails
                        .Where(x => !dto.ProductDetailDTOs.Select(s => s.Id)
                            .Contains(x.Id))
                        .ToList();

                    var addingDetails = dto.ProductDetailDTOs.Where(x => x.Id == null).ToList();


                    if (dbDetails.Count + addingDetails.Count - removingDetails.Count <= 0)
                    {
                        throw new BadRequestException("The product must have at least one color");
                    }

                    //add addingDetails
                    List<ProductDetail> detailList = new List<ProductDetail>();
                    foreach (var item in addingDetails)
                    {
                        detailList.Add(new ProductDetail
                        {
                            ColorId = item.ColorId,
                            ProductId = existing.Id,
                        });
                    }
                    _dbContext.ProductDetails.AddRange(detailList);

                    await _dbContext.SaveChangesAsync();

                    //remove removingDetails
                    _dbContext.ProductDetails.RemoveRange(removingDetails);
                    _dbContext.SaveChanges();


                    existing.About = dto.About;
                    existing.CategoryId = dto.CategoryId;
                    existing.Description = dto.Description;
                    existing.Discount = dto.Discount;
                    existing.Price = dto.Price;
                    existing.ProductName = dto.ProductName;
                    existing.ProductType = dto.ProductType;
                    existing.Status = dto.Status;
                    existing.ProductTestDescription = dto.ProductTestDescription;
                    existing.Warranty = dto.Warranty;


                    _dbContext.Products.Update(existing);

                    await _dbContext.SaveChangesAsync();


                    var dbValues = await _dbContext.PropertyValues
                        .Where(x => x.ProductId == productId)
                        .ToListAsync();

                    var existingValues = dbValues
                        .Where(x => dto.PropertyValues
                            .Select(s => s.Id)
                            .Contains(x.Id))
                        .ToList();

                    var removedValues = dbValues
                        .Where(x => !dto.PropertyValues
                            .Select(s => s.Id)
                            .Contains(x.Id))
                        .ToList();

                    _dbContext.PropertyValues.RemoveRange(removedValues);
                    await _dbContext.SaveChangesAsync();


                    foreach (var item in existingValues)
                    {
                        var newValue = dto.PropertyValues.FirstOrDefault(x => x.Id == item.Id);

                        item.Value = newValue.Value;

                        _dbContext.PropertyValues.Update(item);
                    }
                    await _dbContext.SaveChangesAsync();

                    var addingValues = dto.PropertyValues
                        .Where(s => s.Id == null)
                        .ToList();

                    List<PropertyValue> propList = new List<PropertyValue>();

                    foreach (var item in addingValues)
                    {
                        propList.Add(new PropertyValue
                        {
                            Value = item.Value,
                            PropertyKeyId = item.PropertyKeyId,
                            ProductId = productId
                        });
                    }
                    _dbContext.PropertyValues.AddRange(propList);
                    await _dbContext.SaveChangesAsync();



                    dbContextTransaction.Commit();
                    return true;

                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new AppException(ex.Message);
                }
            }
        }

        public void BasketPriceTracker(Guid productId, decimal newPrice, double newDiscount)
        {
            var affectedBaskets = _dbContext.Orders
                .Include(i => i.OrderDetails)
                .Where(s => s.OrderStatus == OrderStatus.Basket && s.OrderDetails.Where(x => x.ProductId == productId).Any())
                .ToList();

            foreach (var basket in affectedBaskets)
            {
                List<decimal> OldPricesWithDiscount = new();
                List<decimal> NewPricesWithDiscount = new();
                List<decimal> OldPrices = new();
                List<decimal> NewPrices = new();
                List<double> oldDiscounts = new();
                List<double> newDiscounts = new();

                foreach (var od in basket.OrderDetails)
                {
                    OldPricesWithDiscount.Add(od.Count * (od.Amount - ((decimal)od.Discount * od.Amount / 100)));
                    NewPricesWithDiscount.Add(od.Count * (newPrice - ((decimal)newDiscount * newPrice / 100)));

                    OldPrices.Add(od.Count * od.Amount);
                    NewPrices.Add(od.Count * newPrice);

                    oldDiscounts.Add(od.Count * od.Discount);
                    newDiscounts.Add(od.Count * newDiscount);

                    od.Amount = newPrice;
                    od.Discount = newDiscount;

                    _dbContext.OrderDetails.Update(od);

                }

                basket.Price = basket.Price - OldPricesWithDiscount.Sum() + NewPricesWithDiscount.Sum();

                basket.TotalPrice = basket.TotalPrice - OldPrices.Sum() + NewPrices.Sum();

                basket.Discount = basket.Discount - oldDiscounts.Sum() + newDiscounts.Sum();

                _dbContext.SaveChanges();
            }


        }

        public async Task<List<ProductToReturnDTO>> CompareTwoProduct(List<Guid> productIds)
        {
            var query =
            from
            c in _dbContext.Categories
            join
            p in _dbContext.Products
            on c.Id equals p.CategoryId
            join
            i in _dbContext.Images
            on p.Id equals i.ProductId
            join
            k in _dbContext.PropertyKeys
            on c.Id equals k.CategoryId
            join
            v in _dbContext.PropertyValues
            on k.Id equals v.PropertyKeyId
            where
            productIds.Contains(p.Id)
            select
            new ProductToReturnDTO
            {
                ProductId = p.Id,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                About = p.About,
                Discount = p.Discount,
                Code = p.Code,
                ProductType = p.ProductType,
                Status = p.Status,
                ProductTestDescription = p.ProductTestDescription,
                Warranty = p.Warranty,
                CategoryId = p.CategoryId,
                Colors = p.ProductDetails.Select(s => new ColorDTO     //p.Colors.Select(s => new ColorDTO
                {
                    Id = s.Color.Id,
                    Code = s.Color.Code,
                    Name = s.Color.Name
                }).ToList(),
                Images = p.Images.OrderBy(o => o.CreatedAt).Select(s => new ProductImageToReturnDTO
                {
                    Id = s.Id,
                    ImageUrl_L = s.ImageUrl_L,
                    ImageUrl_M = s.ImageUrl_M,
                    ImageUrl_S = s.ImageUrl_S
                }).ToList(),
                PropertyKeys = c.PropertyKeys.Select(s => new PropertyKeyDTO
                {
                    PropertyKeyId = s.Id,
                    KeyType = s.KeyType,
                    Name = s.Name
                }).ToList(),
                PropertyValues = p.PropertyValues.Select(s => new PropertyValueDTO
                {
                    PropertyValueId = s.Id,
                    PropertyKeyId = s.PropertyKeyId,
                    Value = s.Value
                }).ToList()
            };

            return await query.ToListAsync();
        }

        public async Task<ProductKeysValuesToReturnDTO> GetProductKeysAndValues(Guid id)
        {
            return await _dbContext.Products.Where(x => x.Id == id)
                .Include(i => i.PropertyValues)
                .ThenInclude(i => i.PropertyKey)
                .Select(s => new ProductKeysValuesToReturnDTO
                {
                    PropertyKeys = s.PropertyValues.Select(a => new PropertyKeyDTO
                    {

                        PropertyKeyId = a.PropertyKey.Id,
                        KeyType = a.PropertyKey.KeyType,
                        Name = a.PropertyKey.Name
                    }).ToList(),
                    PropertyValues = s.PropertyValues.Select(s => new PropertyValueDTO
                    {
                        PropertyValueId = s.Id,
                        PropertyKeyId = s.PropertyKeyId,
                        Value = s.Value
                    }).ToList()
                }).FirstOrDefaultAsync();

            //var query =
            //    from
            //    c in _dbContext.Categories
            //    join
            //    p in _dbContext.Products
            //    on c.Id equals p.CategoryId
            //    join
            //    k in _dbContext.PropertyKeys
            //    on p.CategoryId equals k.CategoryId
            //    join
            //    v in _dbContext.PropertyValues
            //    on p.Id equals v.ProductId
            //    where p.Id == id
            //    select new ProductKeysValuesToReturnDTO
            //    {
            //        PropertyValues = p.PropertyValues.Select(s => new PropertyValueDTO
            //        {
            //            PropertyValueId = s.Id,
            //            PropertyKeyId = s.PropertyKeyId,
            //            Value = s.Value
            //        }).ToList(),
            //        PropertyKeys = c.PropertyKeys.Select(s => new PropertyKeyDTO
            //        {
            //            Name = s.Name,
            //            KeyType = s.KeyType,
            //            PropertyKeyId = s.Id
            //        }).ToList()
            //    };
            //return await query.FirstOrDefaultAsync();
        }

        public async Task<ProductToReturnDTO> GetProducts(Guid id)
        {

            return await _dbContext.Products.Where(x => x.Id == id)
                .Include(i => i.PropertyValues)
                .ThenInclude(i => i.PropertyKey)
                .Include(i => i.Images)
                .Include(i => i.Category)
                .Include(i => i.ProductDetails)
                .ThenInclude(i => i.Color)
                .Select(p => new ProductToReturnDTO
                {
                    ProductId = p.Id,
                    CategoryId = p.CategoryId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    About = p.About,
                    Discount = p.Discount,
                    Code = p.Code,
                    ProductType = p.ProductType,
                    Status = p.Status,
                    ProductTestDescription = p.ProductTestDescription,
                    Warranty = p.Warranty,
                    DiscountPrice = (p.Price - (p.Price * (decimal)p.Discount / 100)),
                    Colors = p.ProductDetails.Select(s => new ColorDTO                 //p.Colors.Select(s => new ColorDTO
                    {
                        Id = s.Color.Id,
                        Code = s.Color.Code,
                        Name = s.Color.Name
                    }).ToList(),
                    Images = p.Images.OrderBy(o => o.CreatedAt).Select(s => new ProductImageToReturnDTO
                    {
                        Id = s.Id,
                        ImageUrl_L = s.ImageUrl_L,
                        ImageUrl_M = s.ImageUrl_M,
                        ImageUrl_S = s.ImageUrl_S
                    }).ToList(),
                    PropertyKeys = p.PropertyValues.Select(a => new PropertyKeyDTO
                    {
                        PropertyKeyId = a.PropertyKey.Id,
                        KeyType = a.PropertyKey.KeyType,
                        Name = a.PropertyKey.Name
                    }).ToList(),
                    PropertyValues = p.PropertyValues.Select(s => new PropertyValueDTO
                    {
                        PropertyValueId = s.Id,
                        PropertyKeyId = s.PropertyKeyId,
                        Value = s.Value
                    }).Take(5).ToList(),
                    Model = new CategoryToReturnDTO
                    {
                        ArrangeId = p.Category.Arrange,
                        CategoryId = p.Category.Id,
                        ImageUrl_L = p.Category.ImageUrl_L,
                        ImageUrl_M = p.Category.ImageUrl_M,
                        ImageUrl_S = p.Category.ImageUrl_S,
                        Level = p.Category.Level,
                        Name = p.Category.Name,
                    },
                    Brand = new CategoryToReturnDTO
                    {
                        ArrangeId = p.Category.ParentCategory.Arrange,
                        CategoryId = p.Category.ParentCategory.Id,
                        ImageUrl_L = p.Category.ParentCategory.ImageUrl_L,
                        ImageUrl_M = p.Category.ParentCategory.ImageUrl_M,
                        ImageUrl_S = p.Category.ParentCategory.ImageUrl_S,
                        Level = p.Category.ParentCategory.Level,
                        Name = p.Category.ParentCategory.Name
                    },
                    Category = new CategoryToReturnDTO
                    {
                        ArrangeId = p.Category.ParentCategory.ParentCategory.Arrange,
                        CategoryId = p.Category.ParentCategory.ParentCategory.Id,
                        ImageUrl_L = p.Category.ParentCategory.ParentCategory.ImageUrl_L,
                        ImageUrl_M = p.Category.ParentCategory.ParentCategory.ImageUrl_M,
                        ImageUrl_S = p.Category.ParentCategory.ParentCategory.ImageUrl_S,
                        Level = p.Category.ParentCategory.ParentCategory.Level,
                        Name = p.Category.ParentCategory.ParentCategory.Name
                    }
                }).FirstOrDefaultAsync();
        }

        public ProductToReturnDTO GetProductsForAdmin(Guid id)
        {
            return _dbContext.Products.Where(x => x.Id == id)
            .Include(i => i.PropertyValues)
            .ThenInclude(i => i.PropertyKey)
            .Include(i => i.Images)
            .Include(i => i.Category)
            .ThenInclude(i => i.ParentCategory)
            .ThenInclude(i => i.ParentCategory)
            .Include(i => i.ProductDetails)
            .ThenInclude(i => i.Color)
            .Select(p => new ProductToReturnDTO
            {
                ProductId = p.Id,
                CategoryId = p.CategoryId,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                About = p.About,
                Discount = p.Discount,
                Code = p.Code,
                ProductType = p.ProductType,
                Status = p.Status,
                ProductTestDescription = p.ProductTestDescription,
                Warranty = p.Warranty,
                DiscountPrice = p.Price - (p.Price * (decimal)p.Discount / 100),
                Colors = p.ProductDetails.Select(s => new ColorDTO      //p.Colors.Select(s => new ColorDTO
                {
                    DetailId = s.Id,
                    Id = s.Color.Id,
                    Code = s.Color.Code,
                    Name = s.Color.Name
                }).ToList(),
                Images = p.Images.OrderBy(o => o.CreatedAt).Select(s => new ProductImageToReturnDTO
                {
                    Id = s.Id,
                    ImageUrl_L = s.ImageUrl_L,
                    ImageUrl_M = s.ImageUrl_M,
                    ImageUrl_S = s.ImageUrl_S
                }).ToList(),
                PropertyKeys = p.PropertyValues.Select(a => new PropertyKeyDTO
                {
                    PropertyKeyId = a.PropertyKey.Id,
                    KeyType = a.PropertyKey.KeyType,
                    Name = a.PropertyKey.Name
                }).ToList(),
                PropertyValues = p.PropertyValues.Select(s => new PropertyValueDTO
                {
                    PropertyValueId = s.Id,
                    PropertyKeyId = s.PropertyKeyId,
                    Value = s.Value
                }).ToList(),
                BrandId = p.Category.ParentCategoryId,
                Model = new CategoryToReturnDTO
                {
                    ArrangeId = p.Category.Arrange,
                    CategoryId = p.Category.Id,
                    ImageUrl_L = p.Category.ImageUrl_L,
                    ImageUrl_M = p.Category.ImageUrl_M,
                    ImageUrl_S = p.Category.ImageUrl_S,
                    Level = p.Category.Level,
                    Name = p.Category.Name,
                },
                Brand = new CategoryToReturnDTO
                {
                    ArrangeId = p.Category.ParentCategory.Arrange,
                    CategoryId = p.Category.ParentCategory.Id,
                    ImageUrl_L = p.Category.ParentCategory.ImageUrl_L,
                    ImageUrl_M = p.Category.ParentCategory.ImageUrl_M,
                    ImageUrl_S = p.Category.ParentCategory.ImageUrl_S,
                    Level = p.Category.ParentCategory.Level,
                    Name = p.Category.ParentCategory.Name
                },
                Category = new CategoryToReturnDTO
                {
                    ArrangeId = p.Category.ParentCategory.ParentCategory.Arrange,
                    CategoryId = p.Category.ParentCategory.ParentCategory.Id,
                    ImageUrl_L = p.Category.ParentCategory.ParentCategory.ImageUrl_L,
                    ImageUrl_M = p.Category.ParentCategory.ParentCategory.ImageUrl_M,
                    ImageUrl_S = p.Category.ParentCategory.ParentCategory.ImageUrl_S,
                    Level = p.Category.ParentCategory.ParentCategory.Level,
                    Name = p.Category.ParentCategory.ParentCategory.Name
                }
            }).FirstOrDefault();


            //return _dbContext.Products.Where(x => x.Id == id)
            //    .Include(p => p.PropertyValues)
            //    .ThenInclude(p => p.PropertyKey)
            //    .Include(i => i.Images)
            //    .Include(i => i.Category)
            //    .Select(p => new ProductDetailToReturnDTO
            //    {
            //        ProductId = p.Id,
            //        CategoryId = p.CategoryId,
            //        ProductName = p.ProductName,
            //        Description = p.Description,
            //        Price = p.Price,
            //        About = p.About,
            //        Discount = p.Discount,
            //        Code = p.Code,
            //        ProductType = p.ProductType,
            //        Status = p.Status,
            //        brandId = p.Category.ParentCategoryId,
            //        ProductTestDescription = p.ProductTestDescription,
            //        Warranty = p.Warranty,
            //        Colors = p.Colors.Select(s => new ColorDTO
            //        {
            //            Id = s.Id,
            //            Code = s.Code,
            //            Name = s.Name
            //        }).ToList(),
            //        Images = p.Images.Select(s => new ProductImageToReturnDTO
            //        {
            //            Id = s.Id,
            //            ImageUrl_L = s.ImageUrl_L,
            //            ImageUrl_M = s.ImageUrl_M,
            //            ImageUrl_S = s.ImageUrl_S
            //        }).ToList(),
            //        KeyAndValues = p.PropertyValues.Select(a => new ProductKeyToReturnDTO
            //        {
            //            Name = a.PropertyKey.Name,
            //            KeyType = a.PropertyKey.KeyType,
            //            PropertyKeyId = a.PropertyKeyId,
            //            ProductValue = new ProductValueToReturnDTO
            //            {
            //                PropertyKeyId = a.PropertyKeyId,
            //                PropertyValueId = a.Id,
            //                Value = a.Value
            //            }
            //        }).ToList(),

            //    }).FirstOrDefault();
        }

        public void GetSubcategories(List<int> list, Category category)
        {

            list.Add(category.Id);
            var childs = category.ChildCategories;
            foreach (var item in childs)
            {
                GetSubcategories(list, item);
            }
        }

        public async Task<List<PriceLogDTO>> ProductPriceLog(Guid productId)
        {
            return await _dbContext.PriceLogs
                 .Where(x => x.ProductId == productId)
                 .Select(s => new PriceLogDTO
                 {
                     DT = s.DT,
                     Price = s.Price
                 })
                 .OrderBy(o => o.DT)
                 .ToListAsync();
        }

        public async Task<List<PriceLogDTO>> ProductPriceLogInDateRange(Guid productId, DateTime fromDT, DateTime toDT)
        {
            return await _dbContext.PriceLogs
                 .Where(x =>
                 x.ProductId == productId && fromDT > x.DT && x.DT < toDT)
                 .Select(s => new PriceLogDTO
                 {
                     DT = s.DT,
                     Price = s.Price
                 })
                 .OrderBy(o => o.DT)
                 .ToListAsync();
        }

        public async Task<bool> RemoveImage(Guid id)
        {
            var existing = _dbContext.Images.Where(x => x.Id == id).FirstOrDefault();

            if (existing == null)
                return false;

            _dbContext.Remove(existing);
            if (await _dbContext.SaveChangesAsync() > 0)
            {
                var path = @"wwwroot";
                //ImageHelper.RemoveJpeg(path + existing.ImageUrl_L);
                //ImageHelper.RemoveJpeg(path + existing.ImageUrl_M);
                //ImageHelper.RemoveJpeg(path + existing.ImageUrl_S);

                return true;
            }
            return false;
        }

        public async Task<PagedList<ProductToReturnDTO>> SearchInProducts(PaginationParams<ProductSearch> pagination)
        {
            var query = _dbContext.Products
                .Where(x => x.Status != Status.Hidden && x.IsVerified == true)
                .OrderBy(o => o.Status)
                .AsQueryable();

            if (pagination.Query is not null)
            {
                if (pagination.Query.CategoryId is not null and > 0)
                {
                    //var path = await _categoryService.PathToLeaf(pagination.Query.CategoryId.Value);

                    List<int> path = new List<int>();
                    path = await _categoryService.MyPathToLeaf(path, pagination.Query.CategoryId.Value, true);

                    query = query.Where(x => path.Contains(x.CategoryId));

                }

                if (pagination.Query.ProductType is not null)
                {
                    var productType = Enum.Parse<ProductType>(pagination.Query.ProductType.ToString());

                    query = query.Where(x => x.ProductType == productType);
                }

                if (pagination.Query.BrandId is not null)
                {
                    List<int> path = new List<int>();
                    path = await _categoryService.MyPathToLeaf(path, pagination.Query.BrandId.Value, true);

                    query = query.Where(x => path.Contains(x.CategoryId));
                }

                if (pagination.Query.ModelId is not null)
                {
                    query = query.Where(x => x.CategoryId == pagination.Query.ModelId);
                }

                if (pagination.Query.SearchText is not null && pagination.Query.SearchText.Length > 0)
                {
                    query = query.Where(x =>
                    x.ProductName.Contains(pagination.Query.SearchText) ||
                    x.About.Contains(pagination.Query.SearchText) ||
                    x.Description.Contains(pagination.Query.SearchText));
                }
                if (pagination.Query.SearchOrder != null)
                {
                    if (pagination.Query.SearchOrder == ProductSearchOrder.Expensive)
                        query = query.OrderBy(o => o.Status).ThenByDescending(p => p.Price);
                    if (pagination.Query.SearchOrder == ProductSearchOrder.Cheap)
                        query = query.OrderBy(o => o.Status).ThenBy(p => p.Price);
                    if (pagination.Query.SearchOrder == ProductSearchOrder.New)
                        query = query.OrderBy(o => o.Status).ThenByDescending(p => p.CreatedAt);
                }
            }

            var resultQuery = query
                .Include(i => i.ProductDetails)
                .ThenInclude(i => i.Color)
                .Select(x => new ProductToReturnDTO
                {
                    ProductId = x.Id,
                    ProductName = x.ProductName,
                    Description = x.Description,
                    Price = x.Price,
                    About = x.About,
                    Discount = x.Discount,
                    Code = x.Code,
                    ProductType = x.ProductType,
                    Status = x.Status,
                    DiscountPrice = (x.Price - (x.Price * (decimal)x.Discount / 100)),
                    Colors = x.ProductDetails.Select(s => new ColorDTO // x.Colors.Select(s => new ColorDTO
                    {
                        Id = s.Color.Id,
                        Code = s.Color.Code,
                        Name = s.Color.Name,
                    }).ToList(),
                    Images = x.Images.OrderBy(o => o.CreatedAt).Select(s => new ProductImageToReturnDTO
                    {
                        Id = s.Id,
                        ImageUrl_L = s.ImageUrl_L,
                        ImageUrl_M = s.ImageUrl_M,
                        ImageUrl_S = s.ImageUrl_S
                    }).Take(1).ToList()
                });

            return await PagedList<ProductToReturnDTO>.CreateAsync(resultQuery, pagination.CurrentPage, pagination.PageSize);

        }

        public async Task<PagedList<ProductToReturnDTO>> SearchInProductsForAdmin(PaginationParams<ProductSearch> pagination)
        {
            var query = _dbContext.Products
                .Where(x => x.IsVerified == true)
                .AsQueryable();

            if (pagination.Query is not null)
            {
                if (pagination.Query.CategoryId is not null and > 0)
                {
                    //var path = await _categoryService.PathToLeaf(pagination.Query.CategoryId.Value);

                    List<int> path = new List<int>();
                    path = await _categoryService.MyPathToLeaf(path, pagination.Query.CategoryId.Value, true);

                    query = query.Where(x => path.Contains(x.CategoryId));
                }

                if (pagination.Query.ProductType is not null)
                {
                    var productType = Enum.Parse<DSP.ProductService.Data.ProductType>(pagination.Query.ProductType.ToString());
                    query = query.Where(x => x.ProductType == productType);

                }
                if (pagination.Query.BrandId is not null)
                {
                    List<int> path = new List<int>();
                    path = await _categoryService.MyPathToLeaf(path, pagination.Query.BrandId.Value, true);

                    query = query.Where(x => path.Contains(x.CategoryId));
                }

                if (pagination.Query.ModelId is not null)
                {
                    query = query.Where(x => x.CategoryId == pagination.Query.ModelId);
                }

                if (pagination.Query.SearchText is not null && pagination.Query.SearchText.Length > 0)
                {
                    query = query.Where(x =>
                    x.ProductName.Contains(pagination.Query.SearchText) ||
                    x.About.Contains(pagination.Query.SearchText) ||
                    x.Description.Contains(pagination.Query.SearchText));
                }

                var result = pagination.Query.SearchOrder;

                if (pagination.Query.SearchOrder == ProductSearchOrder.Expensive)
                    query = query.OrderByDescending(p => p.Price);
                if (pagination.Query.SearchOrder == ProductSearchOrder.Cheap)
                    query = query.OrderBy(p => p.Price);
                if (pagination.Query.SearchOrder == ProductSearchOrder.New)
                    query = query.OrderByDescending(p => p.CreatedAt);
            }

            var resultQuery = query
                .Include(i => i.ProductDetails)
                .ThenInclude(i => i.Color)
                .Select(x => new ProductToReturnDTO
                {
                    ProductId = x.Id,
                    ProductName = x.ProductName,
                    Description = x.Description,
                    Price = x.Price,
                    About = x.About,
                    Discount = x.Discount,
                    Code = x.Code,
                    CategoryId = x.CategoryId,
                    ProductType = x.ProductType,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    DiscountPrice = (x.Price - (x.Price * (decimal)x.Discount / 100)),
                    Images = x.Images.OrderBy(o => o.CreatedAt).Select(s => new ProductImageToReturnDTO
                    {
                        ImageUrl_L = s.ImageUrl_L,
                        ImageUrl_M = s.ImageUrl_M,
                        ImageUrl_S = s.ImageUrl_S
                    }).Take(1).ToList(),
                    Colors = x.ProductDetails.Select(s => new ColorDTO     //x.Colors.Select(s => new ColorDTO
                    {
                        DetailId = s.Id,
                        Id = s.Color.Id,
                        Code = s.Color.Code,
                        Name = s.Color.Name
                    }).ToList(),

                });

            return await PagedList<ProductToReturnDTO>.CreateAsync(resultQuery, pagination.CurrentPage, pagination.PageSize);

        }

        public async Task<bool> SetProductStatus(Guid productId, Status status)
        {
            var dbProduct = await _dbContext.Products.Where(x => x.Id == productId).FirstOrDefaultAsync();

            if (dbProduct == null)
                return false;
            dbProduct.Status = status;
            _dbContext.Products.Update(dbProduct);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveProduct(Guid productId)
        {
            var dbProduct = _dbContext.Products.Where(x => x.Id == productId).FirstOrDefault();
            if (dbProduct == null)
                throw new NotFoundException("Product Not Found");

            dbProduct.IsVerified = false;
            _dbContext.Update(dbProduct);
            await _dbContext.SaveChangesAsync();

            var BDs = _dbContext.BasketDetails.Where(x => x.ProductId == productId).ToList();

            _dbContext.BasketDetails.RemoveRange(BDs);
            await _dbContext.SaveChangesAsync();

            foreach (var BD in BDs)
            {
                var basket = _dbContext.Baskets.Find(BD.BasketId);
                _dbContext.Baskets.Remove(basket);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }
    }
}



using DSP.ProductService.Data;
using DSP.ProductService.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DSP.ProductService.Services
{
    public class OrderService : IOrderService
    {
        private readonly ProductServiceDbContext _dbContext;
        private readonly ILogger<OrderService> _logger;
        private readonly IProductService _productService;
        public OrderService(ProductServiceDbContext dbContext,
            ILogger<OrderService> logger,
            IProductService productService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _productService = productService;
        }

        public async Task<int> AddToBasket(Guid userId, Guid itemId, Guid ColorId)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(x => x.Id == itemId);

            if (product.Status != Status.Available)
                throw new BadRequestException("product is not available");

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                Guid? BasketId = await GetBasketId(userId);
                if (Guid.Empty == BasketId)
                {
                    await CreateBasket(userId);

                    BasketId = await GetBasketId(userId);
                }

                var Item = await (
                    from o in _dbContext.Baskets
                    join od in _dbContext.BasketDetails
                    on o.Id equals od.BasketId
                    join c in _dbContext.Colors
                    on od.ColorId equals c.Id
                    where
                    o.UserId == userId &&
                    od.ProductId == itemId &&
                    od.ColorId == ColorId
                    select od).FirstOrDefaultAsync();

                if (Item is null)
                {
                    _dbContext.BasketDetails.Add(new BasketDetail
                    {
                        BasketId = BasketId.Value,
                        ProductId = itemId,
                        Count = 1,
                        Amount = product.Price,
                        Discount = product.Discount,
                        ColorId = ColorId,
                    });

                    if (await _dbContext.SaveChangesAsync() > 0)
                    {
                        var dbBasket = _dbContext.Baskets.Where(x => x.Id == BasketId.Value)
                            .FirstOrDefault();

                        dbBasket.Discount += product.Discount;
                        dbBasket.Price += (product.Price - (product.Price * (decimal)product.Discount / 100));
                        dbBasket.TotalPrice += product.Price;

                        _dbContext.Baskets.Update(dbBasket);

                        await _dbContext.SaveChangesAsync();

                        dbContextTransaction.Commit();
                        return 1;
                    }
                    dbContextTransaction.Rollback();
                    throw new AppException("failed to create");
                }
                else
                {
                    if (product.ProductType == ProductType.Used)
                    {
                        throw new PolicyException("cant add many used product");
                    }

                    Item.Count += 1;
                    Item.Amount = product.Price;

                    _dbContext.BasketDetails.Update(Item);

                    if (await _dbContext.SaveChangesAsync() > 0)
                    {
                        var dbBasket = _dbContext.Baskets.Where(x => x.Id == BasketId.Value)
                            .FirstOrDefault();

                        dbBasket.Discount += product.Discount;
                        dbBasket.Price += (product.Price - (product.Price * (decimal)product.Discount / 100));
                        dbBasket.TotalPrice += product.Price;

                        _dbContext.Baskets.Update(dbBasket);

                        await _dbContext.SaveChangesAsync();

                        dbContextTransaction.Commit();
                        return Item.Count;
                    }
                    dbContextTransaction.Rollback();
                    throw new AppException("failed to add");
                }
            }
        }

        public BasketCountToReturnDTO ChangeProductsCountInBasket(Guid userId, bool action, Guid itemId, Guid colorId)
        {

            var dbBasket = _dbContext.Baskets.Where(x => x.UserId == userId)
                .Include(i => i.BasketDetails)
                .FirstOrDefault();

            if (dbBasket == null)
                throw new NotFoundException("there is no basket");

            var dbBaketDetails = _dbContext.BasketDetails
                .Where(x => x.BasketId == dbBasket.Id && x.ColorId == colorId && x.ProductId == itemId)
                .Include(i => i.Product)
                .FirstOrDefault();


            if (dbBaketDetails == null)
                throw new NotFoundException($"there is no product with given ids in basket");

            if (action == true)
            {
                if (dbBaketDetails.Product.ProductType == ProductType.Used)
                {
                    throw new PolicyException("cant add many used product");
                }

                dbBaketDetails.Count += 1;

                _dbContext.BasketDetails.Update(dbBaketDetails);

                if (_dbContext.SaveChanges() > 0)
                {
                    dbBasket.Discount += dbBaketDetails.Product.Discount;
                    dbBasket.Price += (dbBaketDetails.Product.Price - (dbBaketDetails.Product.Price * (decimal)dbBaketDetails.Product.Discount / 100));
                    dbBasket.TotalPrice += dbBaketDetails.Product.Price;

                    _dbContext.Baskets.Update(dbBasket);

                    _dbContext.SaveChanges();

                    return new BasketCountToReturnDTO
                    {
                        Count = dbBaketDetails.Count,
                        Price = dbBasket.Price,
                        TotalPrice = dbBasket.TotalPrice
                    };
                }
                throw new AppException("cant increase item count");
            }
            else
            {
                if (dbBaketDetails.Count == 1)
                {
                    _dbContext.BasketDetails.Remove(dbBaketDetails);

                    var recordeAffected = _dbContext.SaveChanges();

                    if (recordeAffected > 0)
                    {
                        dbBasket.Discount -= dbBaketDetails.Discount;
                        dbBasket.Price -= (dbBaketDetails.Amount - (dbBaketDetails.Amount * (decimal)dbBaketDetails.Discount / 100));
                        dbBasket.TotalPrice -= dbBaketDetails.Amount;

                        _dbContext.Baskets.Update(dbBasket);

                        _dbContext.SaveChanges();

                        return new BasketCountToReturnDTO
                        {
                            Count = 0,
                            Price = dbBasket.Price,
                            TotalPrice = dbBasket.TotalPrice
                        };
                    }
                    throw new AppException("cant decrease item count");
                }
                else
                {
                    dbBaketDetails.Count -= 1;

                    _dbContext.BasketDetails.Update(dbBaketDetails);

                    if (_dbContext.SaveChanges() > 0)
                    {

                        dbBasket.Discount -= dbBaketDetails.Discount;
                        dbBasket.Price -= (dbBaketDetails.Amount - (dbBaketDetails.Amount * (decimal)dbBaketDetails.Discount / 100));
                        dbBasket.TotalPrice -= dbBaketDetails.Amount;

                        _dbContext.Baskets.Update(dbBasket);

                        _dbContext.SaveChanges();

                        return new BasketCountToReturnDTO
                        {
                            Count = dbBaketDetails.Count,
                            Price = dbBasket.Price,
                            TotalPrice = dbBasket.TotalPrice
                        };
                    }
                    throw new AppException("cant decrease item count");
                }
            }
        }

        public async Task<BasketCountToReturnDTO> RemoveFromBasket(
        Guid userId,
        Guid itemId,
        Guid colorId)
        {
            var item = await _dbContext.BasketDetails
                .Include(i => i.Basket)
                .Where(x =>
                    x.ProductId == itemId
                    &&
                    x.ColorId == colorId
                    &&
                    x.Basket.UserId == userId)
                .FirstOrDefaultAsync();

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                if (item != null)
                {
                    _dbContext.BasketDetails.Remove(item);

                    var recordAffected = await _dbContext.SaveChangesAsync();

                    if (recordAffected > 0)
                    {
                        var dbBasket = _dbContext.Baskets.Where(x => x.Id == item.BasketId)
                            .FirstOrDefault();

                        dbBasket.Discount -= item.Count * item.Discount;
                        dbBasket.Price -= item.Count * (item.Amount - (item.Amount * (decimal)item.Discount / 100));
                        dbBasket.TotalPrice -= item.Count * item.Amount;

                        _dbContext.Baskets.Update(dbBasket);

                        _dbContext.SaveChanges();

                        dbContextTransaction.Commit();
                        return new BasketCountToReturnDTO
                        {
                            Count = item.Count,
                            Price = dbBasket.Price,
                            TotalPrice = dbBasket.TotalPrice
                        };
                    }
                }

                dbContextTransaction.Rollback();
                throw new NotFoundException("item not found");
            }
        }

        public async Task<bool> CreateBasket(Guid UserId)
        {

            if (await GetBasketId(UserId) == Guid.Empty)
            {
                var basket = new Basket
                {
                    UserId = UserId,
                    DT = DateTime.Now,
                    AddressId = null,
                    Price = 0,
                    Discount = 0,
                    Tax = 0,
                    TotalPrice = 0,
                    Description = string.Empty,
                    Due = TimeSpan.FromHours(2)
                };

                _dbContext.Baskets.Add(basket);

                return (await _dbContext.SaveChangesAsync()) > 0;
            }

            return false;
        }

        public async Task<Guid?> GetBasketId(Guid UserId)
        {
            return await _dbContext.Baskets
                 .Where(x => x.UserId == UserId)
                 .Select(o => o.Id)
                 .FirstOrDefaultAsync();
        }

        public async Task<int> GetBasketItemCount(Guid userId)
        {
            return await _dbContext.BasketDetails
                .Where(x => x.Basket.UserId == userId)
                .SumAsync(s => s.Count);
        }

        public async Task<PagedList<OrderToReturnDTO>> OrdersList(PaginationParams<OrderSearch> pagination)
        {
            var query = _dbContext.Orders
                .Where(x => x.OrderStatus != OrderStatus.Basket)
                //.Include(i => i.Address)
                //.Include(i => i.User)
                .Include(i => i.OrderDetails)
                .ThenInclude(i => i.Product)
                .AsQueryable();

            if (pagination.Query != null)
            {
                if (!string.IsNullOrWhiteSpace(pagination.Query.SearchText))
                {
                    var text = pagination.Query.SearchText;
                    //query = query.Where(x =>
                    //           x.Code.Contains(text) ||
                    //           x.User.FirstName.Contains(text) ||
                    //           x.User.LastName.Contains(text) ||
                    //           x.User.PhoneNumber.Contains(text) ||
                    //           x.Price.ToString().Contains(text) ||
                    //           x.User.City.Contains(text) ||
                    //           x.User.Province.Contains(text));
                }
                if (pagination.Query.OrderStatusDTO != null)
                    query = query.Where(x => x.OrderStatus == Enum.Parse<OrderStatus>(pagination.Query.OrderStatusDTO.ToString()));

                if (pagination.Query.OrderSearchOrder != null)
                {
                    if (pagination.Query.OrderSearchOrder == SearchOrder.New)
                        query = query.OrderByDescending(o => o.CreatedAt);
                    else
                        query = query.OrderBy(o => o.CreatedAt);
                }
            }
            var resultQuery = query.Select(s => new OrderToReturnDTO
            {
                DT = s.DT,
                OrderId = s.Id,
                //Address = new AddressToReturnDTO
                //{
                //    City = s.Address.City,
                //    State = s.Address.State,
                //},
                OrderStatus = Enum.Parse<OrderStatusDTO>(s.OrderStatus.ToString()),
                Description = s.Description,
                //User = new UserToReturnDTO
                //{
                //    FirstName = s.User.FirstName,
                //    LastName = s.User.LastName,
                //    MobileNumber = s.User.PhoneNumber,
                //    City = s.User.City,
                //    Province = s.User.Province,
                //    Email = s.User.Email
                //},
                Price = s.Price,
                Code = s.Code,
                TotalPrice = s.TotalPrice,
            });
            return await PagedList<OrderToReturnDTO>.CreateAsync(resultQuery, pagination.CurrentPage, pagination.PageSize);

        }

        public async Task<OrderToReturnDTO> OrderDetail(Guid id)
        {
            var result = await _dbContext.Orders
               .Where(x => x.OrderStatus != OrderStatus.Basket && x.Id == id)
               .Include(i => i.Payments)
               //.Include(i => i.User)
               //.Include(i => i.Address)
               .Include(i => i.OrderDetails)
               .ThenInclude(i => i.Product).ToListAsync();
            return result
               .Select(s => new OrderToReturnDTO
               {
                   DT = s.DT,
                   OrderId = s.Id,
                   Code = s.Code,
                   Discount = s.Discount,
                   Tax = s.Tax,
                   TrackingCode = s.Payments.Where(x => x.OrderId == s.Id)
                        .Select(a => a.TrackingCode)
                        .FirstOrDefault(),
                   OrderStatus = Enum.Parse<OrderStatusDTO>(s.OrderStatus.ToString()),
                   Description = s.Description,
                   Price = s.Price,
                   TotalPrice = s.TotalPrice
               }).FirstOrDefault();
        }

        public async Task<BasketToReturnDTO> MyBasket(Guid userId)
        {
            var result = await _dbContext.Baskets
                .Where(x => x.UserId == userId)
                .Include(i => i.BasketDetails)
                .ThenInclude(i => i.Product)
                .Include(i => i.BasketDetails)
                .ThenInclude(i => i.Color)
                .Select(s => new BasketToReturnDTO
                {
                    BasketId = s.Id,
                    Price = s.Price,
                    TotalPrice = s.TotalPrice,
                    Tax = s.Tax,
                    BasketDetails = s.BasketDetails.Select(a => new BasketDetailToReturnDTO
                    {
                        ProductId = a.Product.Id,
                        ProductName = a.Product.ProductName,
                        About = a.Product.About,
                        Description = a.Product.Description,
                        Code = a.Product.Code,
                        Count = a.Count,
                        Price = a.Product.Price,
                        DiscountPrice = (a.Product.Price - (a.Product.Price * (decimal)a.Product.Discount / 100)),
                        Status = a.Product.Status,
                        Discount = a.Product.Discount,
                        CategoryId = a.Product.CategoryId,
                        ProductType = a.Product.ProductType,
                        Images = a.Product.Images.OrderBy(o => o.CreatedAt).Select(a => new ProductImageToReturnDTO
                        {
                            Id = a.Id,
                            ImageUrl_L = a.ImageUrl_L,
                            ImageUrl_M = a.ImageUrl_M,
                            ImageUrl_S = a.ImageUrl_S
                        }).ToList(),
                        Color = new ColorDTO
                        {
                            Code = a.Color.Code,
                            Id = a.Color.Id,
                            Name = a.Color.Name,
                        }
                    }).ToList()

                }).FirstOrDefaultAsync();

            // generate 204 code for empty basket
            if (result is not null && result.BasketDetails.Count <= 0)
                return null;

            return result;
        }

        public async Task<bool> ChangeOrderStatus(Guid id, OrderStatusDTO dto)
        {
            var dbOrder = await _dbContext.Orders.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (dbOrder == null)
                return false;

            dbOrder.OrderStatus = Enum.Parse<OrderStatus>(dto.ToString());

            _dbContext.Orders.Update(dbOrder);

            await _dbContext.SaveChangesAsync();
            try
            {
                SendMessageOnChangeStatus(dbOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return true;
        }
        public void SendMessageOnChangeStatus(Order order)
        {
            //var dbUser = _dbContext.Users.Where(x => x.Id == order.UserId).FirstOrDefault();
            //var smsApi = new KavenegarApi(_siteSettings.SmsApiKey);

            //smsApi.VerifyLookup(dbUser.PhoneNumber, ".", "", "", order.OrderStatus.ToDisplay(), "MobifyChangeOrderStatus");
        }

        public async Task<List<OrderDetailToReturnDTO>> OrderItems(Guid orderId)
        {
            return await _dbContext.OrderDetails.Where(x => x.OrderId == orderId)
                .Include(i => i.Product)
                .Include(i => i.Color)
                .Select(s => new OrderDetailToReturnDTO
                {
                    Amount = s.Amount,
                    Color = new ColorDTO
                    {
                        Id = s.Color.Id,
                        Code = s.Color.Code,
                        Name = s.Color.Name
                    },
                    ColorId = s.ColorId,
                    Count = s.Count,
                    Discount = s.Discount,
                    ProductId = s.ProductId,

                    Product = new ProductToReturnDTO
                    {
                        ProductName = s.Product.ProductName,
                        ProductId = s.Product.Id,
                        Price = s.Product.Price,
                        Discount = s.Product.Discount,
                        About = s.Product.About,
                        Status = s.Product.Status,
                        CategoryId = s.Product.CategoryId,
                        Description = s.Product.Description,
                        ProductTestDescription = s.Product.ProductTestDescription,
                        Warranty = s.Product.Warranty,
                        Code = s.Product.Code,
                    },
                }).ToListAsync();
        }

        public async Task<List<MyOrderToReturnDTO>> MyOrders(Guid userId)
        {
            return await _dbContext.Orders
                .Where(x => x.UserId == userId && x.OrderStatus != OrderStatus.Basket)
                .OrderByDescending(o => o.CreatedAt)
                .Include(i => i.OrderDetails)
                .ThenInclude(i => i.Product)
                .ThenInclude(i => i.Images)
                .Select(s => new MyOrderToReturnDTO
                {
                    OrderId = s.Id,
                    Code = s.Code,
                    DT = s.DT,
                    OrderStatus = Enum.Parse<OrderStatusDTO>(s.OrderStatus.ToString()),
                    Count = s.OrderDetails.Select(a => a.Count).Sum(),
                    Price = s.Price,

                    ProductsInOrder = s.OrderDetails.Select(a => new ProductInOrderToReturnDTO
                    {
                        ProductId = a.Product.Id,
                        ProductName = a.Product.ProductName,
                        Count = a.Count,
                        Images = a.Product.Images.OrderBy(o => o.CreatedAt).Select(b => new ProductImageToReturnDTO
                        {
                            Id = b.Id,
                            ImageUrl_L = b.ImageUrl_L,
                            ImageUrl_M = b.ImageUrl_M,
                            ImageUrl_S = b.ImageUrl_S
                        }).Take(1).ToList()
                    }).ToList()
                }).ToListAsync();
        }

        public MyOrderToReturnDTO MyOrderDetails(Guid userId, Guid orderId)
        {
            return _dbContext.Orders
                .Where(x => x.UserId == userId && x.OrderStatus != OrderStatus.Basket && x.Id == orderId)
                .Include(i => i.OrderDetails)
                .ThenInclude(i => i.Product)
                .ThenInclude(i => i.Images)
                //.Include(i => i.Address)
                .Select(s => new MyOrderToReturnDTO
                {
                    OrderId = s.Id,
                    Code = s.Code,
                    DT = s.DT,
                    OrderStatus = Enum.Parse<OrderStatusDTO>(s.OrderStatus.ToString()),
                    Count = s.OrderDetails.Select(a => a.Count).Sum(),
                    Price = s.Price,
                    TotalPrice = s.TotalPrice,
                    Tax = s.Tax,
                    //Address = new AddressToReturnDTO
                    //{
                    //    AddressId = s.Address.Id,
                    //    City = s.Address.City,
                    //    State = s.Address.State,
                    //    ContactName = s.Address.ContactName,
                    //    ContactNumber = s.Address.ContactNumber,
                    //    DetailedAddress = s.Address.DetailedAddress,
                    //    Label = s.Address.Label,
                    //    PostalCode = s.Address.PostalCode,
                    //},
                    ProductsInOrder = s.OrderDetails.Select(a => new ProductInOrderToReturnDTO
                    {
                        ProductId = a.Product.Id,
                        ProductName = a.Product.ProductName,
                        About = a.Product.About,
                        Code = a.Product.Code,
                        Status = a.Product.Status,
                        ProductType = a.Product.ProductType,
                        Discount = a.Product.Discount,
                        CategoryId = a.Product.CategoryId,
                        Count = a.Count,
                        Description = a.Product.Description,
                        Price = a.Product.Price,
                        Color = new ColorDTO
                        {
                            Code = a.Color.Code,
                            Id = a.Color.Id,
                            Name = a.Color.Name
                        },
                        Images = a.Product.Images.OrderBy(o => o.CreatedAt).Select(b => new ProductImageToReturnDTO
                        {
                            Id = b.Id,
                            ImageUrl_L = b.ImageUrl_L,
                            ImageUrl_M = b.ImageUrl_M,
                            ImageUrl_S = b.ImageUrl_S
                        }).Take(1).ToList()
                    }).ToList()
                }).FirstOrDefault();
        }

        public async Task<bool> IsProductInMyBasket(Guid userId, Guid productId, Guid colorId)
        {
            return await _dbContext.Baskets
            .Include(i => i.BasketDetails)
            .Where(x => x.UserId == userId)
            .AnyAsync(s => s.BasketDetails.Any(a => a.ProductId == productId && a.ColorId == colorId));
        }

        public async Task<bool> SetBasketAddress(Guid userId, BasketAddressDTO dto)
        {
            var dbBasket = await _dbContext.Baskets
                .Where(x => x.Id == dto.BasketId && x.UserId == userId)
                .FirstOrDefaultAsync();

            if (dbBasket == null)
                throw new NotFoundException("there in no basket with given id");

            dbBasket.AddressId = dto.AddressId;

            _dbContext.Baskets.Update(dbBasket);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public List<OrderStatusCountDTO> OrderStatusCount()
        {
            var dbOrder = _dbContext.Orders
                .Where(x => x.OrderStatus != OrderStatus.Basket)
                .AsEnumerable()
                .GroupBy(g => g.OrderStatus)
                .Select(s => new OrderStatusCountDTO
                {
                    OrderStatus = Enum.Parse<OrderStatusDTO>(s.Select(a => a.OrderStatus).FirstOrDefault().ToString()),
                    Count = s.Count()
                }).ToList();

            return dbOrder;
        }

        public async Task<CheckOutResponse> CheckoutTheBasket(Guid userId, BasketCheckOutDTO dto)
        {
            var dbBasket = _dbContext.Baskets
                 .Where(x => x.Id == dto.BasketId)
                 .Include(i => i.BasketDetails)
                 .FirstOrDefault();

            if (dbBasket == null)
                throw new NotFoundException("basket not found");

            if (dbBasket.AddressId == null)
                throw new BadRequestException("basket must contains address");

            try
            {
                var TrackingCode = PID.NewId();

                RequestForPayResponse response = new();
                string selectedGate = "زرین پال";

                   // response = await _paymentService.RequestForPay(dto.PaymentGateWayId, dbBasket, TrackingCode);
                

                using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
                {
                    var orderId = Guid.NewGuid();

                    _dbContext.Orders.Add(new Order
                    {
                        Id = orderId,
                        UserId = userId,
                        DT = DateTime.Now,
                        AddressId = dbBasket.AddressId,
                        Price = dbBasket.Price,
                        TotalPrice = dbBasket.TotalPrice,
                        Discount = dbBasket.Discount,
                        Tax = dbBasket.Tax,
                        Description = dbBasket.Description,
                        Code = PID.NewId(),
                        OrderStatus = OrderStatus.Basket,
                    });

                    await _dbContext.SaveChangesAsync();

                    foreach (var bd in dbBasket.BasketDetails)
                    {
                        _dbContext.OrderDetails.Add(new OrderDetail
                        {
                            OrderId = orderId,
                            Amount = bd.Amount,
                            ColorId = bd.ColorId,
                            Count = bd.Count,
                            Discount = bd.Discount,
                            ProductId = bd.ProductId,
                        });
                    }
                    await _dbContext.SaveChangesAsync();

                    _dbContext.Payments.Add(new Payment
                    {
                        OrderId = orderId,
                        DT = DateTime.Now,
                        UserId = userId,
                        Authority = response.PaymentResponse.Authority,
                        TrackingCode = TrackingCode,
                        IsSuccess = false,
                        MerchantID = response.MerchantId,
                        Amount = dbBasket.Price,
                        GateWayName = selectedGate,
                        RefID = null,
                        Status = 0,
                        CardHash = null,
                        CardPan = null,
                        Fee = 0,
                        FeeType = null,
                        PaymentType = PaymentType.Shopping
                    });

                    _dbContext.SaveChanges();

                    dbContextTransaction.Commit();
                }

                return new CheckOutResponse
                {
                    PaymentUrl = response.PaymentResponse.PaymentURL,
                    Authority = response.PaymentResponse.Authority,
                    TrackingCode = TrackingCode
                };
            }
            catch (Exception)
            {
                throw new AppException("مشکل در اتصال به درگاه پرداخت");
            }
        }

        public async Task VerifyCheckOut(string trackingCode, string authority, string status)
        {

            var dbPayment = _dbContext.Payments.Where(x => x.Authority == authority && x.TrackingCode == trackingCode)
                .Include(i => i.Order)
                //.ThenInclude(i => i.Address)
                .FirstOrDefault();

            if (dbPayment == null)
                throw new AppException(ApiResultStatusCode.UnAuthorized, "unAuthorized Request", HttpStatusCode.Unauthorized);

            VerificationResponse response = new();// = await _paymentService.VerifyPayment(dbPayment);

            if (response.Status != null)
            {
                if (response.Status == 100 || response.Status == 101)
                {
                    using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
                    {
                        dbPayment.IsSuccess = true;
                        dbPayment.RefID = response.RefID;
                        dbPayment.Status = response.Status.Value;
                        dbPayment.DT = DateTime.Now;

                        if (response.ExtraDetail != null && response.ExtraDetail.Transaction != null)
                        {
                            dbPayment.CardHash = response.ExtraDetail.Transaction.CardPanHash;
                            dbPayment.CardPan = response.ExtraDetail.Transaction.CardPanMask;
                        }

                        _dbContext.Payments.Update(dbPayment);

                        _dbContext.SaveChanges();


                        var dbOrder = _dbContext.Orders.Where(x => x.Id == dbPayment.OrderId).FirstOrDefault();

                        dbOrder.OrderStatus = OrderStatus.Pending;

                        _dbContext.Orders.Update(dbOrder);

                        _dbContext.SaveChanges();

                        var dbBasket = _dbContext.Baskets.Where(x => x.UserId == dbPayment.UserId).FirstOrDefault();

                        if (dbBasket == null)
                            throw new BadRequestException("basket already paied");

                        var dbBasketDetails = await _dbContext.BasketDetails.Where(x => x.BasketId == dbBasket.Id)
                            .Include(i => i.Product)
                            .ToListAsync();

                        _dbContext.BasketDetails.RemoveRange(dbBasketDetails);

                        _dbContext.SaveChanges();

                        _dbContext.Baskets.Remove(dbBasket);

                        _dbContext.SaveChanges();

                        foreach (var BD in dbBasketDetails)
                        {
                            if (BD.Product.ProductType == ProductType.Used)
                                await _productService.SetProductStatus(BD.ProductId, Status.UnAvailable);
                        }

                        dbContextTransaction.Commit();

                    }
                }
            }
        }

        public CheckOutResultDTO CheckOutResult(string trackingCode, string authority)
        {
            var dbPayment = _dbContext.Payments.Where(x => x.Authority == authority && x.TrackingCode == trackingCode)
                .Include(i => i.Order)
                //.ThenInclude(i => i.Address)
                .FirstOrDefault();

            if (dbPayment == null)
                throw new NotFoundException("payment not found");

            return new CheckOutResultDTO
            {
                DT = dbPayment.DT,
                Price = dbPayment.Order.Price,
                TrackingCode = dbPayment.TrackingCode,
                Status = dbPayment.IsSuccess,
                //Address = new AddressToReturnDTO
                //{
                //    AddressId = dbPayment.Order.Address.Id,
                //    City = dbPayment.Order.Address.City,
                //    ContactName = dbPayment.Order.Address.ContactName,
                //    ContactNumber = dbPayment.Order.Address.ContactNumber,
                //    DetailedAddress = dbPayment.Order.Address.DetailedAddress,
                //    Label = dbPayment.Order.Address.Label,
                //    PostalCode = dbPayment.Order.Address.PostalCode,
                //    State = dbPayment.Order.Address.PostalCode
                //}
            };
        }
    }
}

using DSP.ProductService.Data;
using DSP.ProductService.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DSP.ProductService.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMemoryCache _treeCache;
        private readonly ProductServiceDbContext _dbContext;

        public CategoryService(
            IMemoryCache treeCache,
            ProductServiceDbContext dbContext)
        {
            _treeCache = treeCache;
            _dbContext = dbContext;
        }

        public async Task<List<CategoryToReturnDTO>> RootCategories()
        {
            return await _dbContext.Categories
                .Where(x => x.ParentCategoryId == null && x.IsVerified == true)
                .OrderBy(o => o.Arrange)
                .Select(x => new CategoryToReturnDTO
                {
                    ImageUrl_L = x.ImageUrl_L,
                    ImageUrl_M = x.ImageUrl_M,
                    ImageUrl_S = x.ImageUrl_S,
                    Level = x.Level,
                    Name = x.Name,
                    ArrangeId = x.Arrange,
                    CategoryId = x.Id
                }).ToListAsync();
        }

        public async Task<List<CategoryToReturnDTO>> BrandsOfCategory(Guid rootId)
        {
            return await _dbContext.Categories
                .Where(x => x.ParentCategoryId == rootId && x.IsVerified == true)
                .OrderBy(o => o.Arrange)
                .Select(x => new CategoryToReturnDTO
                {
                    ImageUrl_L = x.ImageUrl_L,
                    ImageUrl_M = x.ImageUrl_M,
                    ImageUrl_S = x.ImageUrl_S,
                    Level = x.Level,
                    Name = x.Name,
                    ArrangeId = x.Arrange,
                    CategoryId = x.Id
                })
                .ToListAsync();
        }

        public async Task<List<CategoryToReturnDTO>> AllBrands()
        {
            var categoryResult = await _dbContext.Categories
                .Where(x => x.ParentCategory != null && x.ParentCategory.ParentCategoryId == null && x.IsVerified == true)
                .OrderBy(o => o.Arrange)
                //.Where(x => x.Level == 2)
                .Select(x => new CategoryToReturnDTO
                {
                    ImageUrl_L = x.ImageUrl_L,
                    ImageUrl_M = x.ImageUrl_M,
                    ImageUrl_S = x.ImageUrl_S,
                    Level = x.Level,
                    Name = x.Name,
                    ArrangeId = x.Arrange,
                    CategoryId = x.Id
                })
                .ToListAsync();

            return categoryResult;
        }

        public async Task<List<CategoryToReturnDTO>> ModelsOfBrand(Guid brandCategoryid)
        {
            return await _dbContext.Categories
                //.Where(x => x.Level == 3)
                //.Where(x => x.ChildCategories.Count == 0 && x.ParentCategoryId==brandCategoryid)
                .Where(x => x.ParentCategoryId == brandCategoryid && x.IsVerified == true)
                .OrderBy(o => o.Arrange)
                .Select(x => new CategoryToReturnDTO
                {
                    ImageUrl_L = x.ImageUrl_L,
                    ImageUrl_M = x.ImageUrl_M,
                    ImageUrl_S = x.ImageUrl_S,
                    Level = x.Level,
                    Name = x.Name,
                    ArrangeId = x.Arrange,
                    CategoryId = x.Id
                })
                .ToListAsync();
        }


        public async Task<Guid?> AddCategory(CategoryForSetDTO dto)
        {
            if (dto.ParentCategoryId != null && !(await _dbContext.Categories.AnyAsync(x => x.Id == dto.ParentCategoryId)))
                throw new BadRequestException($"there is no category with : {dto.ParentCategoryId}");

            var latestArrangeId =
                await _dbContext.Categories
                .Where(x => x.ParentCategoryId == dto.ParentCategoryId)
                .OrderByDescending(o => o.Arrange)
                .Select(s => s.Arrange)
                .FirstOrDefaultAsync();

            var id = Guid.NewGuid();

            var category = new Category
            {
                Id = id,
                Name = dto.Name,
                ParentCategoryId = dto.ParentCategoryId,
                Arrange = latestArrangeId + 1,
                IsVerified = true
            };

            _dbContext.Categories.Add(category);

            if (await _dbContext.SaveChangesAsync() > 0)
                return id;
            return null;
        }

        public async Task<bool> DeleteCategory(Guid id)
        {
            var existing = await _dbContext.Categories
                .Include(i => i.ChildCategories)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
            {
                throw new NotFoundException("category not found");
            }
            if (existing.ChildCategories.Any())
            {
                throw new PolicyException("category has child");
            }
            _dbContext.Categories.Remove(existing);

            try
            {
                //await _dbContext.SaveChangesAsync();

                var removingArrange = existing.Arrange;

                if (_dbContext.SaveChanges() > 0)
                {
                    var upNeedCats = await _dbContext.Categories
                        .Where(x => x.ParentCategoryId == existing.ParentCategoryId &&
                            x.Arrange > removingArrange &&
                            x.IsVerified == true)
                        .ToListAsync();

                    foreach (var item in upNeedCats)
                    {
                        item.Arrange--;
                        _dbContext.Categories.Update(item);
                    }

                    _dbContext.SaveChanges();

                    var path = @"wwwroot";
                    //ImageHelper.RemoveJpeg(path + existing.ImageUrl_L);
                    //ImageHelper.RemoveJpeg(path + existing.ImageUrl_M);
                    //ImageHelper.RemoveJpeg(path + existing.ImageUrl_S);

                    return true;
                }


                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    existing.IsVerified = false;
                    existing.Arrange = 0;

                    _dbContext.Categories.Update(existing);

                    var removingArrange = existing.Arrange;

                    if (_dbContext.SaveChanges() > 0)
                    {
                        var upNeedCats = await _dbContext.Categories
                            .Where(x => x.ParentCategoryId == existing.ParentCategoryId &&
                                x.Arrange > removingArrange &&
                                x.IsVerified == true)
                            .ToListAsync();

                        foreach (var item in upNeedCats)
                        {
                            item.Arrange--;
                            _dbContext.Categories.Update(item);
                        }

                        _dbContext.SaveChanges();
                        return true;
                    }

                    return false;
                }
                return false;
            }
        }

        public async Task<bool> UpdateCategory(Guid catId, CategoryForSetDTO category)
        {
            var existing = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == catId);

            if (existing == null)
                return false;

            if (category.Name != null)
            {
                existing.Name = category.Name;
            }
            existing.ParentCategoryId = category.ParentCategoryId;

            _dbContext.Categories.Update(existing);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<CategoryToReturnDTO>> AllModels()
        {
            var categoryResult = await _dbContext.Categories
                .Include(i => i.ParentCategory)
                .ThenInclude(i => i.ParentCategory)
                .Where(x => x.ParentCategory != null &&
                        x.ParentCategory.ParentCategory != null &&
                        x.ParentCategory.ParentCategory.ParentCategoryId == null &&
                        x.IsVerified == true)
                //.Where(x => x.Level == 3)
                .Select(x => new CategoryToReturnDTO
                {
                    ImageUrl_L = x.ImageUrl_L,
                    ImageUrl_M = x.ImageUrl_M,
                    ImageUrl_S = x.ImageUrl_S,
                    Level = x.Level,
                    Name = x.Name,
                    ArrangeId = x.Arrange,
                    CategoryId = x.Id
                })
                .ToListAsync();

            return categoryResult;
        }

        public async Task<List<CategoryToReturnDTO>> CategoryArrange(Guid? parentId, List<int> arrangeIds)
        {
            var categories = await _dbContext.Categories
                 .Where(x => x.ParentCategoryId == parentId && x.IsVerified == true)
                 .ToListAsync();

            if (categories.Count != arrangeIds.Count ||
                categories.Where(a => a.IsVerified == true).Select(s => s.Arrange).Any(x => !arrangeIds.Contains(x)))
            {
                throw new BadRequestException("send all arranges");
            }

            foreach (var item in categories)
            {
                var index = arrangeIds.IndexOf(item.Arrange);
                item.Arrange = index + 1;
            }

            _dbContext.Categories.UpdateRange(categories);

            await _dbContext.SaveChangesAsync();

            return categories
                .OrderBy(x => x.Arrange)
                .Select(s => new CategoryToReturnDTO
                {
                    CategoryId = s.Id,
                    ImageUrl_L = s.ImageUrl_L,
                    ImageUrl_M = s.ImageUrl_M,
                    ImageUrl_S = s.ImageUrl_S,
                    Level = s.Level,
                    Name = s.Name,
                    ArrangeId = s.Arrange
                }).ToList();
        }

        public async Task<List<CategoryToReturnDTO>> GetSubCategories(Guid id)
        {
            List<Guid> childCats = new();
            childCats = await MyPathToLeaf(childCats, id, false);

            return await _dbContext.Categories.Where(x => childCats.Contains(x.Id) && x.IsVerified == true)
                .Select(s => new CategoryToReturnDTO
                {
                    Name = s.Name,
                    Level = s.Level,
                    ArrangeId = s.Arrange,
                    CategoryId = s.Id,
                    ImageUrl_L = s.ImageUrl_L,
                    ImageUrl_M = s.ImageUrl_M,
                    ImageUrl_S = s.ImageUrl_S
                })
                .ToListAsync();

        }

        public async Task<List<CategoryToReturnDTO>> GetParentCategories(Guid id)
        {
            List<Guid> childCats = new();
            childCats = await MyPathToRoot(childCats, id);

            return await _dbContext.Categories.Where(x => childCats.Contains(x.Id) && x.IsVerified == true)
                .Select(s => new CategoryToReturnDTO
                {
                    Name = s.Name,
                    Level = s.Level,
                    ArrangeId = s.Arrange,
                    CategoryId = s.Id,
                    ImageUrl_L = s.ImageUrl_L,
                    ImageUrl_M = s.ImageUrl_M,
                    ImageUrl_S = s.ImageUrl_S
                })
                .ToListAsync();

        }
        public async Task<List<Guid>> MyPathToRoot(List<Guid> path, Guid categoryId)
        {
            var res = await _dbContext.Categories
                .Where(x => x.Id == categoryId && x.IsVerified == true)
                .FirstOrDefaultAsync();

            if (res != null)
            {
                path.Add(res.Id);

                if (res.ParentCategoryId != null)
                {
                    //var parentCat = _dbContext.Categories.Where(x => x.Id == res.ParentCategoryId)
                    //    .FirstOrDefault();

                    //foreach (var item in res.ChildCategories)
                    //{
                    await MyPathToRoot(path, res.ParentCategoryId.Value);
                    //}
                }
            }
            return path;
        }
        public async Task<List<Guid>> MyPathToLeaf(List<Guid> path, Guid rootId, bool takeAll)
        {
            Category res;
            if (takeAll)
                res = await _dbContext.Categories
                   .Include(i => i.ChildCategories)
                   .Where(x => x.Id == rootId)
                   .FirstOrDefaultAsync();
            else
                res = await _dbContext.Categories
                   .Include(i => i.ChildCategories)
                   .Where(x => x.Id == rootId && x.IsVerified == true)
                   .FirstOrDefaultAsync();

            if (res != null)
            {
                path.Add(res.Id);
                if (res.ChildCategories != null)
                {
                    foreach (var item in res.ChildCategories)
                    {
                        await MyPathToLeaf(path, item.Id, takeAll);
                    }
                }
            }
            return path;
        }

        public List<CategoryWithBrandDTO> GetCategoriesWithBrands()
        {
            return _dbContext.Categories.Where(x => x.ParentCategoryId == null && x.IsVerified == true)
                .Include(i => i.ChildCategories)
                .Select(s => new CategoryWithBrandDTO
                {
                    CategoryId = s.Id,
                    Name = s.Name,
                    ArrangeId = s.Arrange,
                    Level = s.Level,
                    ImageUrl_L = s.ImageUrl_L,
                    ImageUrl_M = s.ImageUrl_M,
                    ImageUrl_S = s.ImageUrl_S,
                    Brands = s.ChildCategories.Select(a => new CategoryWithBrandDTO
                    {
                        CategoryId = a.Id,
                        Name = a.Name,
                        ArrangeId = a.Arrange,
                        Level = a.Level,
                        ImageUrl_L = a.ImageUrl_L,
                        ImageUrl_M = a.ImageUrl_M,
                        ImageUrl_S = a.ImageUrl_S,
                    }).ToList()
                }).ToList();
        }
    }
}

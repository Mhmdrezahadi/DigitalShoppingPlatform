﻿using DSP.ProductService.Data;
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

        public async Task<List<int>> FlattenedTree(int CategoryId)
        {
            List<int> res = new();

            res.Add(CategoryId);

            var childs = await _dbContext.Categories
                .Where(x => x.ParentCategoryId.Value == CategoryId)
                .Select(s => s.Id)
                .ToListAsync();

            res.AddRange(childs);

            foreach (var item in childs)
            {
                res.AddRange(await FlattenedTree(item));
            }

            _treeCache.Set(CategoryId, res);

            return res;
        }

        public async Task<List<int>> GetRootNodesIds()
        {

            return null;
            //return await _dbContext.Categories
            //      .Where(x => x.ParentCategoryId == null && x.IsVerified == true)
            //      .Select(s => s.Id)
            //      .ToListAsync();
        }

        public async Task FirstRunAfterBoot()
        {
            var ls = await GetRootNodesIds();
            foreach (var item in ls)
            {
                await FlattenedTree(item);
            }
        }

        public List<int> GetTreeFromCache(int CategoryId)
        {
            return _treeCache.Get<List<int>>(CategoryId);
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

        public async Task<List<CategoryToReturnDTO>> BrandsOfCategory(int rootId)
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

        public async Task<List<CategoryToReturnDTO>> ModelsOfBrand(int brandCategoryid)
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

        public async Task<List<int>> PathToRoot(int categoryId)
        {
            List<int> path = new();

            var parentId = await _dbContext.Categories
                .Where(x => x.Id == categoryId)
                .Select(x => x.ParentCategoryId)
                .FirstOrDefaultAsync();

            path.Add(categoryId);

            if (parentId is null)
            {
                return path;
            }

            path.AddRange(await PathToRoot(parentId.Value));

            return path;
        }

        public async Task<List<int>> MyPathToRoot(List<int> path, int categoryId)
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

        public async Task<List<int>> PathToLeaf(int rootId)
        {
            List<int> path = await _dbContext.Categories
                .Where(x => x.ParentCategoryId == rootId)
                .Select(s => s.Id)
                .ToListAsync();

            if (path.Count == 0)
            {
                return path;
            }
            else
            {
                path.ForEach(async x =>
                {
                    path.AddRange(await PathToLeaf(x));
                });
            }

            path.Add(rootId);

            return path;
        }

        public async Task<List<int>> MyPathToLeaf(List<int> path, int rootId, bool takeAll)
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

        public async Task<bool> AddCategory(CategoryForSetDTO dto)
        {
            if (dto.ParentCategoryId != null && !(await _dbContext.Categories.AnyAsync(x => x.Id == dto.ParentCategoryId)))
                throw new BadRequestException($"there is no category with : {dto.ParentCategoryId}");

            var latestArrangeId =
                await _dbContext.Categories
                .Where(x => x.ParentCategoryId == dto.ParentCategoryId)
                .OrderByDescending(o => o.Arrange)
                .Select(s => s.Arrange)
                .FirstOrDefaultAsync();

            var category = new Category
            {
                Name = dto.Name,
                ParentCategoryId = dto.ParentCategoryId,
                Arrange = latestArrangeId + 1,
                IsVerified = true
            };

            var fileNameBase = Guid.NewGuid().ToString();


            var path = @"wwwroot/Categories/";

            var source = dto.Img.OpenReadStream();

            //var image = Image.FromStream(source);

            var fileNameS = fileNameBase + "_S.png";// + dto.Img.FileName.Split('.').Last();
            var fileNameM = fileNameBase + "_M.png";// + dto.Img.FileName.Split('.').Last();
            var fileNameL = fileNameBase + "_L.png";// + dto.Img.FileName.Split('.').Last();

            //ImageHelper.SaveJpeg(source, 100, 100, path + fileNameS, 60);
            //ImageHelper.SaveJpeg(source, 200, 200, path + fileNameM, 80);
            //ImageHelper.SaveJpeg(source, image.Height, image.Width, path + fileNameL, 100);

            category.ImageUrl_S = "/Categories/" + fileNameS;
            category.ImageUrl_M = "/Categories/" + fileNameM;
            category.ImageUrl_L = "/Categories/" + fileNameL;

            _dbContext.Categories.Add(category);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategory(int id)
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

        public async Task<bool> UpdateCategory(int catId, CategoryForSetDTO category)
        {
            var existing = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == catId);

            if (existing == null)
                return false;

            var fileNameBase = Guid.NewGuid().ToString();

            var fileNameS = fileNameBase + "_S.png";// + dto.Img.FileName.Split('.').Last();
            var fileNameM = fileNameBase + "_M.png";// + dto.Img.FileName.Split('.').Last();
            var fileNameL = fileNameBase + "_L.png";// + dto.Img.FileName.Split('.').Last();

            var path = @"wwwroot/Categories/";
            if (category.Img != null)
            {
                var Deletepath = @"wwwroot";
                //ImageHelper.RemoveJpeg(Deletepath + existing.ImageUrl_L);
                //ImageHelper.RemoveJpeg(Deletepath + existing.ImageUrl_M);
                //ImageHelper.RemoveJpeg(Deletepath + existing.ImageUrl_S);

                //var source = category.Img.OpenReadStream();
                //var image = Image.FromStream(source);

                //ImageHelper.SaveJpeg(source, 100, 100, path + fileNameS, 60);
                //ImageHelper.SaveJpeg(source, 200, 200, path + fileNameM, 80);
                //ImageHelper.SaveJpeg(source, image.Height, image.Width, path + fileNameL, 100);

                existing.ImageUrl_L = "/Categories/" + fileNameL;
                existing.ImageUrl_M = "/Categories/" + fileNameM;
                existing.ImageUrl_S = "/Categories/" + fileNameS;

            }
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

        public async Task<List<CategoryToReturnDTO>> CategoryArrange(int? parentId, List<int> arrangeIds)
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

        public async Task<List<CategoryToReturnDTO>> GetSubCategories(int id)
        {
            List<int> childCats = new();
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

        public async Task<List<CategoryToReturnDTO>> GetParentCategories(int id)
        {
            List<int> childCats = new();
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

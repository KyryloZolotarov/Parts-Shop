﻿using Catalog.Host.Data.Entities;
using Catalog.Host.Data;
using Catalog.Host.Repositories.Interfaces;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Catalog.Host.Repositories
{
    public class CatalogItemRepository : ICatalogItemRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CatalogItemRepository> _logger;

        public CatalogItemRepository(
            IDbContextWrapper<ApplicationDbContext> dbContextWrapper,
            ILogger<CatalogItemRepository> logger)
        {
            _dbContext = dbContextWrapper.DbContext;
            _logger = logger;
        }

        public async Task<PaginatedItems<CatalogItem>> GetByPageAsync(int pageIndex, int pageSize, int? brandFilter, int? typeFilter, int? subTypeFilter, int? modelFilter)
        {
            IQueryable<CatalogItem> query = _dbContext.CatalogItems;

            if (brandFilter.HasValue)
            {
                query = query.Where(w => w.CatalogModel.CatalogBrandId == brandFilter.Value);
            }

            if (typeFilter.HasValue)
            {
                query = query.Where(w => w.CatalogSubType.CatalogTypeId == typeFilter.Value);
            }

            if (subTypeFilter.HasValue)
            {
                query = query.Where(w => w.CatalogSubTypeId == subTypeFilter.Value);
            }

            if (modelFilter.HasValue)
            {
                query = query.Where(w => w.CatalogModelId == modelFilter.Value);
            }

            var totalItems = await query.LongCountAsync();

            var itemsOnPage = await query.OrderBy(c => c.Name)
                .Include(i => i.CatalogSubType)
                .Include(i => i.CatalogSubType.CatalogType)
                .Include(i => i.CatalogModel)
                .Include(i => i.CatalogModel.CatalogBrand)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedItems<CatalogItem>() { TotalCount = totalItems, Data = itemsOnPage };
        }

        public Task<CatalogItem> GetByIdAsync(int id)
        {
            return _dbContext.CatalogItems
                .Include(i => i.CatalogSubType)
                .Include(i => i.CatalogSubType)
                .Include(i => i.CatalogModel)
                .Include(i => i.CatalogModel.CatalogBrand)
                .FirstAsync(h => h.Id == id);
        }

        public async Task<List<CatalogItem>> GetItemsListAsync(List<int> itemsId)
        {
            List<CatalogItem> items = new List<CatalogItem>();
            foreach (var id in itemsId)
            {
                var item = await _dbContext.CatalogItems
                    .Include(i => i.CatalogSubType)
                    .Include(i => i.CatalogSubType)
                    .Include(i => i.CatalogModel)
                    .Include(i => i.CatalogModel.CatalogBrand)
                    .FirstAsync(h => h.Id == id);
                items.Add(item);
            }

            return items;
        }

        public async Task<int?> Add(string name, string description, decimal price, int availableStock, string pictureFileName, int catalogSubTypeId, int catalogModelId, string partNumber)
        {
            var modelStatus = await _dbContext.CatalogBrands.AnyAsync(h => h.Id == catalogModelId);
            var subTypeStatus = await _dbContext.CatalogBrands.AnyAsync(h => h.Id == catalogSubTypeId);
            switch (modelStatus && subTypeStatus)
            {
                case false:
                    if (modelStatus == false && subTypeStatus == false)
                    {
                        throw new BusinessException($"Model Id: {catalogModelId} and SubType id: {catalogSubTypeId} were not found");
                    }
                    else if (subTypeStatus == false)
                    {
                        throw new BusinessException($"SubType Id: {catalogSubTypeId} was not found");
                    }
                    else
                    {
                        throw new BusinessException($"Model Id: {catalogModelId} was not found");
                    }

                case true:
                    var item1 = new CatalogItem
                    {
                        Description = description,
                        Name = name,
                        PictureFileName = pictureFileName,
                        Price = price,
                        AvailableStock = availableStock,
                        CatalogModelId = catalogModelId,
                        CatalogSubTypeId = catalogSubTypeId,
                        PartNumber = partNumber
                    };
                    var item = await _dbContext.AddAsync(item1);

                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Item {item.Entity.Name} id: {item.Entity.Id} added");
                    return item.Entity.Id;
            }
        }

        public async Task<int?> Update(int id, string name, string description, decimal price, int availableStock, string pictureFileName, int catalogSubTypeId, int catalogModelId, string partNumber)
        {
            var modelStatus = await _dbContext.CatalogModels.AnyAsync(h => h.Id == catalogModelId);
            var subTypeStatus = await _dbContext.CatalogSubTypes.AnyAsync(h => h.Id == catalogSubTypeId);
            switch (modelStatus && subTypeStatus)
            {
                case false:
                    if (modelStatus == false && subTypeStatus == false)
                    {
                        throw new BusinessException($"Model Id: {catalogModelId} and SubType id: {catalogSubTypeId} were not found");
                    }
                    else if (subTypeStatus == false)
                    {
                        throw new BusinessException($"SubType Id: {catalogSubTypeId} was not found");
                    }
                    else
                    {
                        throw new BusinessException($"Model Id: {catalogModelId} was not found");
                    }

                case true:
                    var item1 = new CatalogItem
                    {
                        Id = id,
                        Description = description,
                        Name = name,
                        PictureFileName = pictureFileName,
                        Price = price,
                        AvailableStock = availableStock,
                        CatalogModelId = catalogModelId,
                        CatalogSubTypeId = catalogSubTypeId,
                        PartNumber = partNumber
                    };
                    var item = _dbContext.Update(item1);

                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Item {item.Entity.Name} id: {item.Entity.Id} updated");
                    return item.Entity.Id;
            }
        }

        public async Task<int?> Delete(int id)
        {
            var itemExists = await _dbContext.CatalogItems.AnyAsync(x => x.Id == id);
            if (itemExists == true)
            {
                var itemDelete = await _dbContext.CatalogItems.FirstAsync(h => h.Id == id);
                _dbContext.Remove(itemDelete);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Item {itemDelete.Name} id: {itemDelete.Id} deleted");
                return itemDelete.Id;
            }
            else
            {
                throw new BusinessException($"Item id: {id} was not founded");
            }
        }

        public async Task<List<CatalogType>> GetTypesAsync()
        {
            return await _dbContext.CatalogItems
                .Select(h => h.CatalogSubType.CatalogType)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<CatalogBrand>> GetBrandsAsync()
        {
            return await _dbContext.CatalogItems
                .Select(h => h.CatalogModel.CatalogBrand)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<CatalogSubType>> GetSubTypesAsync(int id)
        {
            return await _dbContext.CatalogItems
                .Select(h => h.CatalogSubType)
                .Where(x => x.CatalogTypeId == id)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<CatalogModel>> GetModelsAsync(int id)
        {
            return await _dbContext.CatalogItems
                .Select(h => h.CatalogModel)
                .Where(x => x.CatalogBrandId == id)
                .Distinct()
                .ToListAsync();
        }
    }
}

﻿using Catalog.Host.Data;
using Catalog.Host.Repositories.Interfaces;
using Catalog.Host.Services.Interfaces;

namespace Catalog.Host.Services
{
    public class CatalogBrandService : BaseDataService<ApplicationDbContext>, ICatalogBrandService
    {
        private readonly ICatalogBrandRepository _catalogBrandRepository;
        public CatalogBrandService(
            IDbContextWrapper<ApplicationDbContext> dbContextWrapper,
            ILogger<BaseDataService<ApplicationDbContext>> logger,
            ICatalogBrandRepository catalogBrandRepository)
            : base(dbContextWrapper, logger)
        {
            _catalogBrandRepository = catalogBrandRepository;
        }

        public Task<int?> Add(string brandName)
        {
            return ExecuteSafeAsync(() => _catalogBrandRepository.Add(brandName));
        }

        public Task<int?> Update(int id, string brandName)
        {
            return ExecuteSafeAsync(() => _catalogBrandRepository.Update(id, brandName));
        }

        public Task<int?> Delete(int id)
        {
            return ExecuteSafeAsync(() => _catalogBrandRepository.Delete(id));
        }
    }
}

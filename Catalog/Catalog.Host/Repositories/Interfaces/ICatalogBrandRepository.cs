﻿namespace Catalog.Host.Repositories.Interfaces
{
    public interface ICatalogBrandRepository
    {
        Task<int?> Add(int id, string brandName);
        Task<int?> Update(int id, string brandName);
        Task<int?> Delete(int id);
    }
}
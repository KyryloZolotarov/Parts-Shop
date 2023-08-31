﻿using MVC.ViewModels;

namespace MVC.Services.Interfaces;

public interface ICatalogService
{
    Task<Catalog> GetCatalogItems(int page, int take, int? brand, int? model, int? type, int? subType);
    Task<IEnumerable<CatalogBrand>> GetBrands();
    Task<IEnumerable<CatalogType>> GetTypes();
    Task<IEnumerable<CatalogSubType>> GetSubTypesByType(int? selectedType);
    Task<IEnumerable<CatalogModel>> GetModelsByBrand(int? selectedBrand);
}
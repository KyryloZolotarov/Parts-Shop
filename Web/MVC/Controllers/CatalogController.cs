﻿using MVC.Services.Interfaces;
using MVC.ViewModels.BasketViewModels;
using MVC.ViewModels.CatalogViewModels;
using MVC.ViewModels.Pagination;

namespace MVC.Controllers;

public class CatalogController : Controller
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index(int? brandFilterApplied, int? modelFilterApplied, int? typesFilterApplied,
        int? subTypeFilterApplied, int? page, int? itemsPage)
    {
        page ??= 0;
        itemsPage ??= 6;

        var catalog = await _catalogService.GetCatalogItems(page.Value, itemsPage.Value, brandFilterApplied,
            modelFilterApplied, typesFilterApplied, subTypeFilterApplied);
        if (catalog == null) return View("Error");

        var info = new PaginationInfo
        {
            ActualPage = page.Value,
            ItemsPerPage = catalog.Data.Count,
            TotalItems = catalog.Count,
            TotalPages = (int)Math.Ceiling((decimal)catalog.Count / itemsPage.Value)
        };

        var vm = new IndexViewModel
        {
            CatalogItems = catalog.Data,
            Brands = (await _catalogService.GetBrands()).Select(x => new SelectListItem(x.Brand, x.Id.ToString())),
            Types = (await _catalogService.GetTypes()).Select(x => new SelectListItem(x.Type, x.Id.ToString())),
            PaginationInfo = info
        };

        vm.PaginationInfo.Next =
            vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1 ? "is-disabled" : "";
        vm.PaginationInfo.Previous = vm.PaginationInfo.ActualPage == 0 ? "is-disabled" : "";

        return View(vm);
    }

    public async Task<IActionResult> GetModels(int? selectedBrand)
    {
        if (selectedBrand == null)
        {
            var initialModels = new List<SelectListItem>
            {
                new() { Value = "", Text = "All" }
            };
            return Json(initialModels);
        }

        var models = await _catalogService.GetModelsByBrand(selectedBrand);
        var modelItems = models.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Model }).ToList();
        modelItems.Append(new SelectListItem { Value = "", Text = "All" });
        return Json(modelItems);
    }

    public async Task<IActionResult> GetSubTypes(int? selectedType)
    {
        if (selectedType == null)
        {
            var initialSubType = new List<SelectListItem>
            {
                new() { Value = "", Text = "All" }
            };
            return Json(initialSubType);
        }

        var models = await _catalogService.GetSubTypesByType(selectedType);
        var modelItems = models.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.SubType }).ToList();
        modelItems.Append(new SelectListItem { Value = "", Text = "All" });
        return Json(modelItems);
    }

    public async Task<IActionResult> GetItemById(int id)
    {
        var vm = await _catalogService.GetItemById(id);
        var item = new CatalogItemForSingleItem
        {
            basketItem = new BasketItem
            {
                Id = id,
                Count = 1
            },
            catalogItem = new CatalogItem
            {
                Id = vm.Id,
                Name = vm.Name,
                CatalogModel = vm.CatalogModel,
                CatalogSubType = vm.CatalogSubType,
                Description = vm.Description,
                PictureUrl = vm.PictureUrl,
                Price = vm.Price,
                AvailableStock = vm.AvailableStock
            }
        };

        return View(item);
    }
}
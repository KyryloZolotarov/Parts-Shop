﻿namespace Catalog.Host.Models.Requests.AddRequsts
{
    public class AddItemRequest
    {
        public string Name { get; set; } = null!;

        public string PartNumber { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public string PictureFileName { get; set; } = null!;

        public int CatalogTypeId { get; set; }

        public int CatalogBrandId { get; set; }

        public int CatalogSubTypeId { get; set; }
        public int CatalogModelId { get; set; }

        public int AvailableStock { get; set; }
    }
}
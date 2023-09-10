﻿using Infrastructure.Exceptions;
using Order.Host.Data;
using Order.Hosts.Data.Entities;
using Order.Hosts.Repositories.Interfaces;

namespace Order.Hosts.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OrderItemRepository> _logger;

    public OrderItemRepository(
        IDbContextWrapper<ApplicationDbContext> dbContextWrapper,
        ILogger<OrderItemRepository> logger)
    {
        _dbContext = dbContextWrapper.DbContext;
        _logger = logger;
    }

    public async Task<int?> Add(int id, string name, decimal price, int catalogSubTypeId, int catalogModelId, int count, int orderId)
    {
        var orderStatus = await _dbContext.OrderOrders.AnyAsync(h => h.Id == orderId);
        if (orderStatus)
        {
            var item1 = new OrderItemEntity
            {
                ItemId = id,
                Name = name,
                Price = price,
                CatalogModelId = catalogModelId,
                CatalogSubTypeId = catalogSubTypeId,
                Count = count,
                OrderId = orderId
            };
            var item = await _dbContext.OrderItems.AddAsync(item1);

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Item {item.Entity.Name} id: {item.Entity.Id} added");
            return item.Entity.Id;
        }

        throw new BusinessException($"Order Id: {orderId} was not found");
    }

    public async Task<int?> Update(int id, int itemId, string name, decimal price, int catalogSubTypeId, int catalogModelId, int count, int orderId)
    {
        var orderStatus = await _dbContext.OrderOrders.AnyAsync(h => h.Id == orderId);
        var itemStatus = await _dbContext.OrderItems.AnyAsync(h => h.Id == id);
        if (orderStatus && itemStatus)
        {
            var item1 = new OrderItemEntity
            {
                Id = id,
                ItemId = itemId,
                Name = name,
                Price = price,
                CatalogModelId = catalogModelId,
                CatalogSubTypeId = catalogSubTypeId,
                Count = count,
                OrderId = orderId
            };
            var item = _dbContext.OrderItems.Update(item1);

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Item {item.Entity.Name} id: {item.Entity.Id} updated");
            return item.Entity.Id;
        }

        if (itemStatus == false)
        {
            throw new BusinessException($"Item Id: {id} was not found");
        }

        if (orderStatus == false)
        {
            throw new BusinessException($"Order Id: {orderId} was not found");
        }

        throw new BusinessException($"Item Id: {id} and Order Id: {orderId} was not found");
    }

    public async Task<int?> Delete(int id)
    {
        var itemExists = await _dbContext.OrderItems.AnyAsync(x => x.Id == id);
        if (itemExists)
        {
            var itemDelete = await _dbContext.OrderItems.FirstAsync(h => h.Id == id);
            _dbContext.OrderItems.Remove(itemDelete);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Item Id Deleted {itemDelete.Id}");
            return itemDelete.Id;
        }

        throw new BusinessException($"Item Id {id} was not founded");
    }
}
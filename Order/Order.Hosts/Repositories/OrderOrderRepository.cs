﻿using Infrastructure.Exceptions;
using Order.Host.Data;
using Order.Hosts.Data.Entities;
using Order.Hosts.Repositories.Interfaces;

namespace Order.Hosts.Repositories
{
    public class OrderOrderRepository : IOrderOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<OrderOrderRepository> _logger;

        public OrderOrderRepository(
            IDbContextWrapper<ApplicationDbContext> dbContextWrapper,
            ILogger<OrderOrderRepository> logger)
        {
            _dbContext = dbContextWrapper.DbContext;
            _logger = logger;
        }

        public async Task<int?> Add(int userId)
        {
            var order = await _dbContext.AddAsync(new OrderOrderEntity()
            {
                UserId = userId
            });

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Order {order.Entity.Id}");
            return order.Entity.Id;
        }

        public async Task<int?> Update(int userId)
        {
            var orderExists = await _dbContext.OrderOrders.AnyAsync(x => x.UserId == userId);
            if (orderExists == true)
            {
                var order = _dbContext.Update(new OrderOrderEntity()
                {
                    Id = userId
                });
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Brand {order.Entity.Id}");
                return order.Entity.Id;
            }
            else
            {
                throw new BusinessException($"Brand id: {userId} was not founded");
            }
        }

        public async Task<int?> Delete(int id)
        {
            var orderExists = await _dbContext.OrderOrders.AnyAsync(x => x.Id == id);
            if (orderExists == true)
            {
                var orderDelete = await _dbContext.OrderOrders.FirstAsync(h => h.Id == id);
                _dbContext.Remove(orderDelete);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Order Id Deleted {orderDelete.Id}");
                return orderDelete.Id;
            }
            else
            {
                throw new BusinessException($"Order Id {id} was not founded");
            }
        }
    }
}

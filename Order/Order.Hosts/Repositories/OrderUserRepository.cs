﻿using Infrastructure.Exceptions;
using Order.Host.Data;
using Order.Hosts.Data.Entities;
using Order.Hosts.Repositories.Interfaces;

namespace Order.Hosts.Repositories
{
    public class OrderUserRepository : IOrderUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<OrderUserRepository> _logger;

        public OrderUserRepository(
            IDbContextWrapper<ApplicationDbContext> dbContextWrapper,
            ILogger<OrderUserRepository> logger)
        {
            _dbContext = dbContextWrapper.DbContext;
            _logger = logger;
        }

        public async Task<int?> Add(int id, string name, string givenName, string familyName, string email, string address)
        {
            var user1 = new OrderUserEntity()
                {
                    Id = id,
                    Name = name,
                    GivenName = givenName,
                    FamilyName = familyName,
                    Email = email,
                    Address = address
                };
            var user = await _dbContext.AddAsync(user1);

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"User {user.Entity.Name} id: {user.Entity.Id} added");
            return user.Entity.Id;
        }

        public async Task<int?> Update(int id, string name, string givenName, string familyName, string email, string address)
        {
            var userExists = await _dbContext.OrderUsers.AnyAsync(x => x.Id == id);
            if (userExists == true)
            {
                var user = _dbContext.Update(new OrderUserEntity()
                {
                    Id = id,
                    Name = name,
                    GivenName = givenName,
                    FamilyName = familyName,
                    Email = email,
                    Address = address
                });
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"User {user.Entity.Name} id: {user.Entity.Id} updated");
                return user.Entity.Id;
            }
            else
            {
                throw new BusinessException($"User {name} id: {id} was not founded");
            }
        }

        public async Task<int?> Delete(int id)
        {
            var userExists = await _dbContext.OrderUsers.AnyAsync(x => x.Id == id);
            if (userExists == true)
            {
                var userDelete = await _dbContext.OrderUsers.FirstAsync(h => h.Id == id);
                _dbContext.Remove(userDelete);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"User Id Deleted {userDelete.Id}");
                return userDelete.Id;
            }
            else
            {
                throw new BusinessException($"User Id {id} was not founded");
            }
        }
    }
}

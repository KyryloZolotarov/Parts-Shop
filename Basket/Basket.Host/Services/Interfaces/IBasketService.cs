﻿using Basket.Host.Models;

namespace Basket.Host.Services.Interfaces
{
    public interface IBasketService
    {
        Task TestAdd(string userId, string data);
        Task<GetResponse> TestGet(string userId);
    }
}

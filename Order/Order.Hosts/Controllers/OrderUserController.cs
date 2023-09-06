﻿using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Hosts.Models.Requests;
using Order.Hosts.Models.Responses;
using Order.Hosts.Services.Interfaces;

namespace Order.Hosts.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthPolicy.AllowClientPolicy)]
    [Scope("order.orderuser")]
    [Route(ComponentDefaults.DefaultRoute)]
    public class OrderUserController : ControllerBase
    {
        private readonly ILogger<OrderUserController> _logger;
        private readonly IOrderUserService _orderUserService;

        public OrderUserController(
            ILogger<OrderUserController> logger,
            IOrderUserService orderUserService)
        {
            _logger = logger;
            _orderUserService = orderUserService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<int?>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Add(AddUserRequest request)
        {
            var result = await _orderUserService.Add(request.Id, request.Name, request.GivenName, request.FamilyName, request.Email, request.Address);
            return Ok(new BaseResponse<int?>() { Id = result });
        }

        [HttpPut]
        [ProducesResponseType(typeof(BaseResponse<int?>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update(UpdateUserRequest request)
        {
            var result = await _orderUserService.Update(request.Id, request.Name, request.GivenName, request.FamilyName, request.Email, request.Address);
            return Ok(new BaseResponse<int?>() { Id = result });
        }

        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderUserService.Delete(id);
            return NoContent();
        }
    }
}

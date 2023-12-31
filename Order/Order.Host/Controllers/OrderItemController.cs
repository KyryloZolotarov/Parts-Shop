﻿using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Order.Hosts.Models.Requests;
using Order.Hosts.Models.Responses;
using Order.Hosts.Services.Interfaces;

namespace Order.Hosts.Controllers;

[ApiController]
[Authorize(Policy = AuthPolicy.AllowClientPolicy)]
[Scope("order.orderitem")]
[Route(ComponentDefaults.DefaultRoute)]
public class OrderItemController : ControllerBase
{
    private readonly ILogger<OrderItemController> _logger;
    private readonly IOrderItemService _orderItemService;

    public OrderItemController(
        ILogger<OrderItemController> logger,
        IOrderItemService orderItemService)
    {
        _logger = logger;
        _orderItemService = orderItemService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<int?>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Add(AddItemRequest request)
    {
        var result = await _orderItemService.Add(request.Id, request.Name, request.Price, request.CatalogSubTypeId, request.CatalogModelId, request.Count, request.OrderId);
        return Ok(new BaseResponse<int?> { Id = result });
    }

    [HttpPut]
    [ProducesResponseType(typeof(BaseResponse<int?>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Update(UpdateItemRequest request)
    {
        var result = await _orderItemService.Update(request.Id, request.ItemId, request.Name, request.Price, request.CatalogSubTypeId, request.CatalogModelId, request.Count, request.OrderId);
        return Ok(new BaseResponse<int?> { Id = result });
    }

    [HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _orderItemService.Delete(id);
        return NoContent();
    }
}
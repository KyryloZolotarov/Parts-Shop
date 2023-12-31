﻿using Catalog.Host.Models.Requests.AddRequsts;
using Catalog.Host.Models.Requests.UpdateRequsts;
using Catalog.Host.Models.Responses.AddResponses;
using Catalog.Host.Models.Responses.UpdateResponses;
using Catalog.Host.Services.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Host.Controllers;

[ApiController]
[Authorize(Policy = AuthPolicy.AllowClientPolicy)]
[Scope("catalog.catalogsubtype")]
[Route(ComponentDefaults.DefaultRoute)]
public class CatalogSubTypeController : Controller
{
    private readonly ICatalogSubTypeService _catalogSubTypeService;
    private readonly ILogger<CatalogSubTypeController> _logger;

    public CatalogSubTypeController(
        ILogger<CatalogSubTypeController> logger,
        ICatalogSubTypeService catalogSubTypeService)
    {
        _logger = logger;
        _catalogSubTypeService = catalogSubTypeService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AddSubTypeResponse<int?>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Add(AddSubTypeRequest request)
    {
        var result = await _catalogSubTypeService.Add(request.SubTypeName, request.CatalogTypeId);
        return Ok(new AddSubTypeResponse<int?> { Id = result });
    }

    [HttpPut]
    [ProducesResponseType(typeof(UpdateSubTypeResponse<int?>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Update(UpdateSubTypeRequest request)
    {
        var result = await _catalogSubTypeService.Update(request.Id, request.SubTypeName, request.CatalogTypeId);
        return Ok(new UpdateSubTypeResponse<int?> { Id = result });
    }

    [HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _catalogSubTypeService.Delete(id);
        return NoContent();
    }
}
using Ardalis.ApiEndpoints;
using CoffeeShop.Contracts;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductService.Endpoints;

public class GetItemTypes : EndpointBaseSync
    .WithRequest<ItemTypesQuery>
    .WithActionResult<ItemTypeDto>
{
    private readonly ILogger<GetItemTypes> _logger;

    public GetItemTypes(ILogger<GetItemTypes> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("/v1/api/item-types")]
    [SwaggerOperation(
        Summary = "Get list of item types",
        Description = "Get list of item types",
        OperationId = "Product.GetItemTypes",
        Tags = new[] { "ProductEndpoint" })]
    public override ActionResult<ItemTypeDto> Handle([FromQuery]ItemTypesQuery request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var results = new List<ItemTypeDto>
        {
            // beverages
            new() {Name = ItemType.CAPPUCCINO.ToString(), Type = ItemType.CAPPUCCINO},
            new() {Name = ItemType.COFFEE_BLACK.ToString(), Type = ItemType.COFFEE_BLACK},
            new() {Name = ItemType.COFFEE_WITH_ROOM.ToString(), Type = ItemType.COFFEE_WITH_ROOM},
            new() {Name = ItemType.ESPRESSO.ToString(), Type = ItemType.ESPRESSO},
            new() {Name = ItemType.ESPRESSO_DOUBLE.ToString(), Type = ItemType.ESPRESSO_DOUBLE},
            new() {Name = ItemType.LATTE.ToString(), Type = ItemType.LATTE},
            // food
            new() {Name = ItemType.CAKEPOP.ToString(), Type = ItemType.CAKEPOP},
            new() {Name = ItemType.CROISSANT.ToString(), Type = ItemType.CROISSANT},
            new() {Name = ItemType.MUFFIN.ToString(), Type = ItemType.MUFFIN},
            new() {Name = ItemType.CROISSANT_CHOCOLATE.ToString(), Type = ItemType.CROISSANT_CHOCOLATE}
        };

        return Ok(results.Distinct());
    }
}

public record ItemTypesQuery;
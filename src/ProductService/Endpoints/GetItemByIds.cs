using Ardalis.ApiEndpoints;
using CoffeeShop.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProductService.Attributes;
using ProductService.Domain;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductService.Endpoints;

public class GetItemByIds : EndpointBaseSync
    .WithRequest<ItemsByIdsQuery>
    .WithActionResult<ItemDto>
{
    private readonly ILogger<GetItemByIds> _logger;

    public GetItemByIds(ILogger<GetItemByIds> logger)
    {
        _logger = logger;
    }

    [HttpGet("/v1/api/items-by-types/{itemTypes}")]
    [SwaggerOperation(
        Summary = "Get items by types",
        Description = "Get items by types",
        OperationId = "Product.GetItemByIds",
        Tags = new[] {"ProductEndpoint"})]
    public override ActionResult<ItemDto> Handle([FromMultipleSource] ItemsByIdsQuery request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var results = new List<ItemDto>();
        var itemTypes = request.ItemTypes.Split(",").Select(id => (ItemType) Convert.ToInt16(id));
        foreach (var itemType in itemTypes)
        {
            var temp = Item.GetItem(itemType);
            results.Add(new ItemDto {Type = temp.Type, Price = temp.Price});
        }

        return Ok(results.Distinct());
    }
}

public record ItemsByIdsQuery([FromRoute(Name = "itemTypes")] string ItemTypes);
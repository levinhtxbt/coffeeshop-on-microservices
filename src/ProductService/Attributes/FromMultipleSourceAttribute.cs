using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProductService.Attributes;

public class FromMultipleSourceAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource? BindingSource { get; } = CompositeBindingSource.Create(
        new[] {BindingSource.Path, BindingSource.Query}, nameof(FromMultipleSourceAttribute));
}
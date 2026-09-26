namespace Catalog.Application.Exceptions;

public class ProductSearchUnavailableException
    : Exception
{
    public ProductSearchUnavailableException(
        string message)
        : base(message)
    {
    }
}

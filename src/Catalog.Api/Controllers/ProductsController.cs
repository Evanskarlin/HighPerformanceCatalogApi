using Catalog.Api.Models.Products;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCacheService _productCacheService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductRepository productRepository,
        IProductCacheService productCacheService,
        ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _productCacheService = productCacheService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAll()
    {
        var products = await _productRepository.GetAllAsync();

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Product>> GetById(Guid id)
    {
        var cachedProduct = await _productCacheService.GetAsync(id);

        if (cachedProduct is not null)
        {
            _logger.LogInformation(
                "Cache HIT for product {ProductId}",
                id);

            return Ok(cachedProduct);
        }

        _logger.LogInformation(
            "Cache MISS for product {ProductId}",
            id);

        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        await _productCacheService.SetAsync(product);

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(CreateProductRequest request)
    {
        var now = DateTime.UtcNow;
    
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Brand = request.Brand,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = now,
            UpdatedAt = now
        };
    
        await _productRepository.AddAsync(product);
    
        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Product>> Update(
        Guid id,
        UpdateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Category = request.Category;
        product.Brand = request.Brand;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);

        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        await _productRepository.DeleteAsync(product);

        return NoContent();
    }
}

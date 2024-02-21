using Service;

namespace TestsWithDoubles.TestDoubles;

public class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<int, Product> _products = new();
    
    public void Save(Product product)
    {
        _products[product.Id] = product;
    }

    public Product Get(int id)
    {
        if (!_products.ContainsKey(id))
        {
            throw new KeyNotFoundException("Product not found.");
        }
        
        return _products[id];
    }

    public bool Contains(int id)
    {
        return _products.ContainsKey(id);
    }

    public bool DidSave(Product product) =>
        _products.ContainsKey(product.Id) && _products[product.Id] == product;
}
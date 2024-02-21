namespace Service;

public class ProductService(IProductRepository _productRepository)
{
    public void OnboardNewProduct(int id, string name) =>
        _productRepository.Save(new Product(id, name));

    public Product GetProduct(int id) =>
        // step 2: add Contains method to IProductRepository before calling Get and throw KeyNotFoundException if not found
        _productRepository.Contains(id) ? _productRepository.Get(id) : throw new KeyNotFoundException("Product not found.");
}
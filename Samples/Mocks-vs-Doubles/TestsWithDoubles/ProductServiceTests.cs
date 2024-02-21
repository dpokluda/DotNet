using Service;
using TestsWithDoubles.TestDoubles;

namespace TestsWithDoubles;

[TestClass]
public class ProductServiceTests
{
    [TestMethod]
    public void OnboardNewProduct_WhenCalled_ShouldSaveProduct()
    {
        // setup
        var repository = new InMemoryProductRepository();
        var productService = new ProductService(repository);
        var product = new Product(1, "Product 1");
        
        // act
        productService.OnboardNewProduct(product.Id, product.Name);
        
        // validate
        Assert.IsTrue(repository.DidSave(product));
    }
    
    [TestMethod]
    public void GetProduct_WhenCalled_ShouldReturnProduct()
    {
        // setup
        var repository = new InMemoryProductRepository();
        var productService = new ProductService(repository);
        var product = new Product(1, "Product 1");
        repository.Save(product);
        
        // act
        var result = productService.GetProduct(product.Id);
        
        // validate
        Assert.AreEqual(product, result);
    }
    
    // step 2: add Contains method to IProductRepository and check it throws when product doesn't exist
    [TestMethod]
    public void GetProduct_WhenProductNotFound_ShouldThrowKeyNotFoundException()
    {
        // setup
        var repository = new InMemoryProductRepository();
        var productService = new ProductService(repository);
        
        // act and validate
        Assert.ThrowsException<KeyNotFoundException>(() => productService.GetProduct(1));
    }
}


using Moq;
using Service;

namespace TestsWithMocks;

[TestClass]
public class ProductServiceTests
{
    [TestMethod]
    public void OnboardNewProduct_WhenCalled_ShouldSaveProduct()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(repositoryMock.Object);
        var product = new Product(1, "Product 1");
        
        // Act
        productService.OnboardNewProduct(product.Id, product.Name);
        
        // Assert
        repositoryMock.Verify(repository => repository.Save(product), Times.Once);
    }
    
    [TestMethod]
    public void GetProduct_WhenCalled_ShouldReturnProduct()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(repositoryMock.Object);
        var product = new Product(1, "Product 1");
        repositoryMock.Setup(repository => repository.Contains(It.IsAny<int>())).Returns(true);
        repositoryMock.Setup(repository => repository.Get(It.IsAny<int>())).Returns(product);
        
        // Act
        var result = productService.GetProduct(1);
        
        // Assert
        repositoryMock.VerifyAll();
        Assert.AreEqual(product, result);
    }
    
    // step 2: add Contains method to IProductRepository and check it throws when product doesn't exist
    [TestMethod]
    public void GetProduct_WhenProductNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(repositoryMock.Object);
        repositoryMock.Setup(repository => repository.Contains(It.IsAny<int>())).Returns(false);
        
        // Act and Assert
        Assert.ThrowsException<KeyNotFoundException>(() => productService.GetProduct(1));
        repositoryMock.VerifyAll();
    }
}
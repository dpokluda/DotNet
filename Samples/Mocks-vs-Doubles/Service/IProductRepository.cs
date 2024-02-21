namespace Service;

public interface IProductRepository
{
    void Save(Product product);
    
    Product Get(int id);

    // step 2: add Contains method to IProductRepository before calling Get and throw KeyNotFoundException if not found
    bool Contains(int id);
}
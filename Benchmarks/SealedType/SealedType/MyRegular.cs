namespace SealedType;

public class MyRegular
{
    private readonly int _value;
    private readonly string _name;

    public MyRegular(int value, string name)
    {
        _value = value;
        _name = name;
    }
    
    public int Value => _value;
    public string Name => _name;
}
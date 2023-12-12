namespace Recursively;

public class Node<T>
{
    public readonly T Data;
    public Node<T> Left;
    public Node<T> Right;
    
    public Node(T data)
    {
        Data = data;
    }
}
namespace QueueUsingLinkedList;

public class Queue<T>
{
    private LinkedList<T> _list;

    public Queue()
    {
        _list = new LinkedList<T>();
    }
    
    public bool IsEmpty()
    {
        return _list.IsEmpty();
    }
    
    public void Enqueue(T item)
    {
        _list.PushBack(item);
    }
    
    public T Dequeue()
    {
        return _list.PopFront();
    }
    
    public T Peek()
    {
        return _list.PeekFront();
    }
    
    public void Print()
    {
        _list.Print();
    }
}
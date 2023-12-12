namespace Queue;

public class Queue<T>
{
    private T[] _data;
    private int _capacity;
    private int _head; // where the dequeue from
    private int _tail; // next available index for newly enqueued items

    public Queue()
    {
        _data = null;
        _capacity = 0;
        _head = 0;
        _tail = 0;
    }

    public bool IsEmpty()
    {
        return _head == _tail;
    }

    public int GetSize()
    {
        return _tail - _head;
    }

    public void Enqueue(T item)
    {
        if (_tail == _capacity)
        {
            var newCapacity = _capacity == 0 ? 1 : _capacity * 2;
            if (_tail - _head < _capacity / 2)
            {
                // there is still enough space; let's just shift the data
                newCapacity = _capacity;
            }
            Resize(newCapacity);
        }
        _data[_tail++] = item;
    }

    public T Dequeue()
    {
        if (IsEmpty())
        {
            throw new System.InvalidOperationException("Queue is empty");
        }
        return _data[_head++];
    }
    
    public T Peek()
    {
        if (IsEmpty())
        {
            throw new System.InvalidOperationException("Queue is empty");
        }
        return _data[_head];
    }
    
    private void Resize(int newCapacity)
    {
        var newData = new T[newCapacity];
        if (_data != null)
        {
            for (int i = 0; i < _tail - _head; ++i)
            {
                newData[i] = _data[i + _head];
            }
        }
        _data = newData;
        _head = 0;
        _tail = _tail - _head;
        _capacity = newCapacity;
    }
    
    public void Clear()
    {
        _head = 0;
        _tail = 0;
    }
    
    public void Print()
    {
        for (int i = _head; i < _tail; ++i)
        {
            System.Console.Write(_data[i] + ", ");
        }
        System.Console.WriteLine();
    }
}
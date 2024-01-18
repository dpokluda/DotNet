using System;

namespace Stack;

public class Stack<T>
{
    private T[] _data;
    private int _capacity;
    private int _size;

    public Stack()
    {
        _data = null;
        _capacity = 0;
        _size = 0;
    }

    public bool IsEmpty()
    {
        return _size == 0;
    }

    public int GetSize()
    {
        return _size;
    }
    
    public void Push(T item)
    {
        if (_size == _capacity)
        {
            Resize(_capacity == 0 ? 1 : _capacity * 2);
        }
        _data[_size++] = item;
    }

    public T Pop()
    {
        if (_size == 0)
        {
            throw new InvalidOperationException("Stack is empty");
        }

        return _data[--_size];
    }

    public T Peek()
    {
        if (_size == 0)
        {
            throw new InvalidOperationException("Stack is empty");
        }

        return _data[_size - 1];
    }

    public void Print()
    {
        for (int i = 0; i < _size; ++i)
        {
            Console.Write(_data[i] + ", ");
        }
        Console.WriteLine();
    }
    
    private void Resize(int newCapacity)
    {
        var newData = new T[newCapacity];
        if (_data != null)
        {
            for (int i = 0; i < _size; ++i)
            {
                newData[i] = _data[i];
            }
        }
        _data = newData;
        _capacity = newCapacity;
    }
}
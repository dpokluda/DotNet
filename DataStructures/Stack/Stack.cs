using System;

namespace Stack;

public class Stack<T>
{
    private T[] _data;
    private int _capacity;
    private int _top_index;

    public Stack()
    {
        _data = null;
        _capacity = 0;
        _top_index = 0;
    }

    public bool IsEmpty()
    {
        return _top_index == 0;
    }

    public int GetSize()
    {
        return _top_index;
    }
    
    public void Push(T item)
    {
        if (_top_index == _capacity)
        {
            Resize(_capacity == 0 ? 1 : _capacity * 2);
        }
        _data[_top_index++] = item;
    }

    public T Pop()
    {
        if (_top_index == 0)
        {
            throw new InvalidOperationException("Stack is empty");
        }

        return _data[--_top_index];
    }

    public T Peek()
    {
        if (_top_index == 0)
        {
            throw new InvalidOperationException("Stack is empty");
        }

        return _data[_top_index - 1];
    }

    public void Print()
    {
        for (int i = 0; i < _top_index; ++i)
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
            for (int i = 0; i < _top_index; ++i)
            {
                newData[i] = _data[i];
            }
        }
        _data = newData;
        _capacity = newCapacity;
    }
}
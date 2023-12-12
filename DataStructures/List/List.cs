using System;
using System.Collections.Generic;

namespace List;

public class List<T>
{
    private T[] _data;
    private int _size;
    private int _capacity;
    
    public List()
    {
        _data = null;
        _size = 0;
        _capacity = 0;
    }
    
    public List(T[] array)
    {
        Resize(array.Length);
        foreach(T item in array)
        {
            PushBack(item);
        }
    }

    public void PushBack(T item)
    {
        if (_size == _capacity)
        {
            Resize(_capacity == 0 ? 1 : _capacity * 2);
        }
        _data[_size++] = item;
    }
    
    public int GetSize()
    {
        return _size;
    }

    public bool IsEmpty()
    {
        return _size == 0;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _size)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            return _data[index];
        }
        set
        {
            if (index < 0 || index >= _size)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            _data[index] = value;
        }
    }
    
    public T PopBack()
    {
        if (_size == 0)
        {
            throw new InvalidOperationException("List is empty");
        }
        
        return _data[--_size];
    }
    
    public void Clear()
    {
        _size = 0;
    }
    
    private void Resize(int newCapacity)
    {
        if (newCapacity <= _size) return;
        
        T[] newData = new T[newCapacity];
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
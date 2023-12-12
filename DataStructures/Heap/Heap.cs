using System;

namespace Heap;

public class Heap<T> where T : IComparable<T>
{
    private T[] _data;
    private int _size;
    private int _capacity;

    public Heap()
    {
        _data = null;
        _size = 0;
        _capacity = 0;
    }

    public bool IsEmpty()
    {
        return _size == 0;
    }

    public int GetSize()
    {
        return _size;
    }

    public void Enqueue(T item)
    {
        if (_size == _capacity)
        {
            Resize(_capacity == 0 ? 1 : _capacity * 2);
        }
        
        _data[_size++] = item;
        SiftUp(_size - 1);
    }

    public T Dequeue()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Heap is empty");
        }

        var max = _data[0];
        _data[0] = _data[_size - 1];
        --_size;
        SiftDown(0);
        
        return max;
    }

    public T Peek()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Heap is empty");
        }

        return _data[0];
    }
    
    public void Print()
    {
        int nextLevel = 0;
        for (int i = 0; i < _size; i++)
        {
            Console.Write(_data[i]);
            if (i == nextLevel)
            {
                Console.WriteLine();;
                nextLevel += nextLevel + 2;
            }
            else if (i % 2 == 1)
            {
                // middle member
                Console.Write(" ^ ");
            }
            else
            {
                Console.Write("   ");
            }
        }
    }

    private int GetParent(int index)
    {
        return (index - 1) / 2;
    }
    
    int GetLeftChildIndex(int index) 
    {
        return index * 2 + 1;
    }

    int GetRightChildIndex(int index)
    {
        return index * 2 + 2;
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
    
    private void SiftUp(int index)
    {
        while (index > 0 && _data[index].CompareTo(_data[GetParent(index)]) > 0)
        {
            Swap(index, GetParent(index));
            index = GetParent(index);
        }
    }

    private void SiftDown(int index)
    {
        while (GetLeftChildIndex(index) < _size)
        {
            var leftChildIndex = GetLeftChildIndex(index);
            var rightChildIndex = GetRightChildIndex(index);
            var maxIndex = leftChildIndex;
            if (rightChildIndex < _size && _data[rightChildIndex].CompareTo(_data[leftChildIndex]) > 0)
            {
                maxIndex = rightChildIndex;
            }

            if (_data[index].CompareTo(_data[maxIndex]) > 0)
            {
                break;
            }

            Swap(index, maxIndex);
            index = maxIndex;
        }
    }
    
    private void Swap(int i, int j)
    {
        (_data[i], _data[j]) = (_data[j], _data[i]);
    }
}
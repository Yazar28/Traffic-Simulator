using System;
using System.Collections;
using System.Collections.Generic;

public class MyList<T> : IEnumerable<T>
{
    private T[] items;
    private int count;

    public MyList(int capacity = 4)
    {
        items = new T[capacity];
        count = 0;
    }

    public int Count => count;

    public void Add(T item)
    {
        if (count >= items.Length)
            Resize();

        items[count++] = item;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count)
            throw new IndexOutOfRangeException();

        for (int i = index; i < count - 1; i++)
            items[i] = items[i + 1];

        count--;
    }

    public T Get(int index)
    {
        if (index < 0 || index >= count)
            throw new IndexOutOfRangeException();

        return items[index];
    }

    public T this[int index]
    {
        get => Get(index);
        set
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            items[index] = value;
        }
    }

    private void Resize()
    {
        T[] newItems = new T[items.Length * 2];
        for (int i = 0; i < count; i++)
            newItems[i] = items[i];
        items = newItems;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        for (int i = 0; i < count; i++)
            yield return items[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)this).GetEnumerator();
    }

    public void Clear()
    {
        count = 0;
    }
}

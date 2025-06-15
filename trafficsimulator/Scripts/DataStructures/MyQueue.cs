using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class MyQueue<T> : IEnumerable<T>
{
    private T[] items;
    private int front;
    private int rear;
    private int count;

    public MyQueue(int capacity = 4)
    {
        items = new T[capacity];
        front = 0;
        rear = 0;
        count = 0;
    }

    public int Count => count;

    public void Enqueue(T item)
    {
        if (count == items.Length)
            Resize();

        items[rear] = item;
        rear = (rear + 1) % items.Length;
        count++;
    }

    public T Dequeue()
    {
        if (count == 0)
            throw new InvalidOperationException("Queue is empty.");

        T item = items[front];
        front = (front + 1) % items.Length;
        count--;
        return item;
    }

    public T Peek()
    {
        if (count == 0)
            throw new InvalidOperationException("Queue is empty.");

        return items[front];
    }

    public bool IsEmpty()
    {
        return count == 0;
    }

    private void Resize()
    {
        T[] newItems = new T[items.Length * 2];
        for (int i = 0; i < count; i++)
            newItems[i] = items[(front + i) % items.Length];

        items = newItems;
        front = 0;
        rear = count;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < count; i++)
            yield return items[(front + i) % items.Length];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

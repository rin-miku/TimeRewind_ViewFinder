using System;

public class RewindBuffer<T>
{
    private const int defaultCapacity = 3000;
    private T[] buffers = new T[defaultCapacity];
    private int position;

    public void Resize()
    {
        int capacity = buffers.Length * 2;
        Array.Resize(ref buffers, capacity);
    }

    public void WriteBuffer(T values)
    {
        if (position >= buffers.Length) Resize();
        buffers[position] = values;
        position++;
    }

    public T ReadBuffer()
    {
        position--;
        if (position < 0) return default;
        T values = buffers[position];
        return values;
    }
}

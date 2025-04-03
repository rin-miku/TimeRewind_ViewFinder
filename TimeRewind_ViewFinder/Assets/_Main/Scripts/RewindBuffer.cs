using System;

public class RewindBuffer<T>
{
    private const int defaultCapacity = 3000;
    private T[] buffers = new T[defaultCapacity];
    private int position;
    private int rewindPosition;

    public void Resize()
    {
        int capacity = buffers.Length * 2;
        Array.Resize(ref buffers, capacity);
    }

    public void WriteBuffer(T values)
    {
        position++;
        if (position >= buffers.Length)
        {
            Resize();
        }
        buffers[position] = values;
    }

    public T ReadBuffer()
    {
        if (position < 0) return default;
        T values = buffers[position];
        position--;
        return values;
    }
}

using System;

public class RewindBuffer<T>
{
    public const int defaultCapacity = 3000;
    private T[] buffer = new T[defaultCapacity];
    private int position;
    private int rewindPosition;

    public void Resize()
    {
        int capacity = buffer.Length * 2;
        Array.Resize(ref buffer, capacity);
    }

    public void WriteBuffer(T values)
    {
        position++;
        if (position >= buffer.Length)
        {
            Resize();
        }
        buffer[position] = values;
    }

    public T ReadBuffer()
    {
        if (position < 0) return default(T);
        T values = buffer[position];
        position--;
        return values;
    }
}

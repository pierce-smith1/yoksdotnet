namespace yoksdotnet.common;

public class CircularBuffer<T>(int size)
{
    private readonly T[] _buffer = new T[size];

    public int start = 0;
    public int End => start == 0 ? _buffer.Length - 1 : start - 1;
    public int Count => size;

    public void Shift()
    {
        start = (start + 1) % _buffer.Length;
    }

    public int ToBufferIndex(int i)
    {
        return (i + start) % _buffer.Length;
    }

    public ref T First() => ref _buffer[start];
    public ref T Last() => ref _buffer[End];

    public T this[int index]
    {
        get => _buffer[ToBufferIndex(index)];
        set => _buffer[ToBufferIndex(index)] = value;
    }
}

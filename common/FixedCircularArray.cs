using System.Collections;
using System.Collections.Generic;

namespace yoksdotnet.common;

public class FixedCircularArray<T>(int size) : IEnumerable<T>
{
    private T[] _buffer = new T[size];

    private int _currentIndex = 0;
    private int _count = 0;

    public int Count => _count;

    public void Add(T item)
    {
        _buffer[_currentIndex] = item;

        _currentIndex++;
        _currentIndex = _currentIndex % size;

        if (_count < size)
        {
            _count++;
        }
    }

    public void Clear()
    {
        _count = 0;
        _currentIndex = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(_buffer, _count);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public struct Enumerator(T[] buffer, int count) : IEnumerator<T>
    {
        private int _pos = -1;

        public bool MoveNext()
        {
            _pos++;
            return _pos < count;
        }

        public void Reset()
        {
            _pos = 1;
        }

        public readonly void Dispose() {}

        readonly object IEnumerator.Current => Current!;

        public readonly T Current => buffer[_pos];
    }
}

using System;
using System.Collections;
using System.Collections.Generic;

public class UIPanelStack<T> : IEnumerable<T>, IEnumerator<T>
{
    private List<T> stackList = new List<T>();
    private int position = -1; // 用于迭代器

    // 入栈
    public void Push(T item)
    {
        stackList.Add(item);
    }

    // 出栈
    public T Pop()
    {
        if (stackList.Count == 0)
            throw new InvalidOperationException("Stack is empty");

        T item = stackList[^1];
        stackList.RemoveAt(stackList.Count - 1);
        return item;
    }

    // 获取栈顶元素（不移除）
    public T Peek()
    {
        if (stackList.Count == 0)
            throw new InvalidOperationException("Stack is empty");

        return stackList[^1];
    }

    // 移除指定元素（如果存在）
    public bool Remove(T item)
    {
        return stackList.Remove(item);
    }

    // 检查是否包含某个元素
    public bool Contains(T item)
    {
        return stackList.Contains(item);
    }

    // 清空栈
    public void Clear()
    {
        stackList.Clear();
    }

    // 获取栈内元素数量
    public int Count => stackList.Count;

    public IEnumerator<T> GetEnumerator()
    {
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // **实现 IEnumerator<T> 迭代器**
    public bool MoveNext()
    {
        if (position < stackList.Count - 1)
        {
            position++;
            return true;
        }
        return false;
    }

    public void Reset()
    {
        position = -1;
    }

    public T Current
    {
        get
        {
            if (position < 0 || position >= stackList.Count)
                throw new InvalidOperationException();
            return stackList[position];
        }
    }

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        // 这里可以清理资源，但 List<T> 不需要特殊处理
    }
}

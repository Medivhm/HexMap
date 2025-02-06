using System;
using System.Collections;
using System.Collections.Generic;

public class UIPanelStack<T> : IEnumerable<T>, IEnumerator<T>
{
    private List<T> stackList = new List<T>();
    private int position = -1; // ���ڵ�����

    // ��ջ
    public void Push(T item)
    {
        stackList.Add(item);
    }

    // ��ջ
    public T Pop()
    {
        if (stackList.Count == 0)
            throw new InvalidOperationException("Stack is empty");

        T item = stackList[^1];
        stackList.RemoveAt(stackList.Count - 1);
        return item;
    }

    // ��ȡջ��Ԫ�أ����Ƴ���
    public T Peek()
    {
        if (stackList.Count == 0)
            throw new InvalidOperationException("Stack is empty");

        return stackList[^1];
    }

    // �Ƴ�ָ��Ԫ�أ�������ڣ�
    public bool Remove(T item)
    {
        return stackList.Remove(item);
    }

    // ����Ƿ����ĳ��Ԫ��
    public bool Contains(T item)
    {
        return stackList.Contains(item);
    }

    // ���ջ
    public void Clear()
    {
        stackList.Clear();
    }

    // ��ȡջ��Ԫ������
    public int Count => stackList.Count;

    public IEnumerator<T> GetEnumerator()
    {
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // **ʵ�� IEnumerator<T> ������**
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
        // �������������Դ���� List<T> ����Ҫ���⴦��
    }
}

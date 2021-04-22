using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 新栈
/// </summary>
/// <typeparam name="T"></typeparam>
public class MyStack<T>
{
    private Node<T> head;  //头节点
    private int count;  //栈内元素个数

    public int Count
    {
        get { return count; }
    }

    /// <summary>
    /// 入栈
    /// </summary>
    /// <param name="data"></param>
    public void Push(T data)
    {
        Node<T> newNode = new Node<T>(data);
        newNode.nextNode = head;
        head = newNode;
        count++;
    }

    /// <summary>
    /// 出栈
    /// </summary>
    /// <returns></returns>
    public T Pop()
    {
        if(count < 1)
        {
            Debugger.LogError($"--------------栈内为空-----------");
            return default;
        }
        T data = head.data;
        head = head.nextNode;
        count--;
        return data;
    }

    /// <summary>
    /// 返回栈顶元素
    /// </summary>
    /// <returns></returns>
    public T Peek()
    {
        if (count < 1)
        {
            Debugger.LogError($"--------------栈内为空-----------");
            return default;
        }
        T data = head.data;
        return data;
    }

    /// <summary>
    /// 移除指定元素
    /// </summary>
    /// <param name="data"></param>
    public void Remove(T data)
    {
        Node<T> node = null;
        node = head;
        if (node == null)
            return;
        if (node.data.Equals(data))
        {
            if(node.nextNode == null)
            {
                head = null;
            }
            {
                head = node.nextNode;
            }
            count--;
            return;
        }

        Node<T> lastNode = null;
        for(int i = 0; i < count-1; i++)
        {
            lastNode = node;

            if (node.nextNode == null)
            {
                Debugger.LogError($"------------------栈内不包含{data}元素------------------");
                return;
            }
            node = node.nextNode;

            if (node.data.Equals(data))
            {
                if (node.nextNode == null)
                {
                    lastNode.nextNode = null;
                }
                else
                {
                    lastNode.nextNode = node.nextNode;
                }
                count--;
                return;
            }
        }

        Debugger.LogError($"------------------栈内不包含{data}元素------------------");
    }
}

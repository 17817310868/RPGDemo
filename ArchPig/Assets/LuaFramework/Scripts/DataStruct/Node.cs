using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 节点类
/// </summary>
/// <typeparam name="T"></typeparam>
public class Node<T>
{
    public T data;  //节点数据
    public Node<T> nextNode;  //指向下一个节点

    public Node()
    {

    }

    public Node(T data)
    {
        this.data = data;
        nextNode = null;
    }

    public Node(Node<T> nextNode)
    {
        this.data = default;
        this.nextNode = nextNode;
    }

    public Node(T data,Node<T> nextNode)
    {
        this.data = default;
        this.nextNode = nextNode;
    }
}

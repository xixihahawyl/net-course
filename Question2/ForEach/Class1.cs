﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForEach
{
	public class Node<T>
	{
		public Node<T> Next { get; set; }
		public T Data { get; set; }

		public Node(T t)
		{
			Next = null;
			Data = t;
		}
	}

	public class GenericList<T>
	{
		private Node<T> tail;

		public GenericList()
		{
			Head = tail = null;
		}

		public Node<T> Head { get; private set; }
		public void Add(T t)
		{
			Node<T> n = new Node<T>(t);
			if (tail == null)
			{
				Head = tail = n;
			}
			else
			{
				tail.Next = n;
				tail = n;
			}
		}

		public void ForEach(Action<T> action)
		{
			for (Node<T> it = Head; it != null; it = it.Next)
			{
				action(it.Data);
			}
		}
	}
}
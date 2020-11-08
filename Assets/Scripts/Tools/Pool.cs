using System.Collections.Generic;

namespace Unitils
{
	public class Pool<T> where T : class, new()
	{
		private class PoolElement
		{
			public T Element { get; private set; }
			public bool Using;

			public PoolElement(T element)
			{
				this.Element = element;
				this.Using = false;
			}
		}

		private LinkedList<PoolElement> poolElements;
		private int size;

		public delegate T OnCreateElementDelegate(int index);
		public event OnCreateElementDelegate OnCreateElement;

		public delegate void OnDeleteElementDelegate(T poolInstance);
		public event OnDeleteElementDelegate OnDeleteElement;

		public Pool(int size)
		{
			this.size = size;
			this.poolElements = new LinkedList<PoolElement>();
		}

		public void Resize(int size)
		{
			this.size = size;
		}

		public void Preload()
		{
			int count = this.size - this.poolElements.Count;
			if (count <= 0) return;
			for (int i = 0; i < count; i++) {
				this.Allocate();
			}
		}

		public void Clear()
		{
			if (this.OnDeleteElement != null) {
				LinkedListNode<PoolElement> node = this.poolElements.First;
				while (node != null) {
					this.OnDeleteElement(node.Value.Element);
					node = node.Next;
				}
			}
			this.poolElements.Clear();
		}

		public T Get()
		{
			PoolElement poolElement;
			LinkedListNode<PoolElement> node = this.poolElements.First;
			while (node != null) {
				if (!node.Value.Using) break;
				node = node.Next;
			}

			if (node != null) {
				poolElement = node.Value;
			}
			else {
				poolElement = this.Allocate();
			}

			poolElement.Using = true;
			return poolElement.Element;
		}

		public void Restore(T element)
		{
			LinkedListNode<PoolElement> node = this.poolElements.First;
			while (node != null) {
				if (ReferenceEquals(node.Value.Element, element)) break;
				node = node.Next;
			}
			if (node == null) return;

			node.Value.Using = false;
			if (this.poolElements.Count > this.size) {
				this.poolElements.Remove(node);
				this.OnDeleteElement?.Invoke(node.Value.Element);
			}
		}

		public List<T> GetUsingElements()
		{
			List<T> result = new List<T>();
			LinkedListNode<PoolElement> node = this.poolElements.First;
			while (node != null) {
				if (node.Value.Using) {
					result.Add(node.Value.Element);
				}
				node = node.Next;
			}
			return result;
		}

		private PoolElement Allocate()
		{
			PoolElement poolElement;
			if (this.OnCreateElement != null) {
				poolElement = new PoolElement(this.OnCreateElement(this.poolElements.Count));
			}
			else {
				poolElement = new PoolElement(new T());
			}
			this.poolElements.AddLast(poolElement);
			return poolElement;
		}
	}
}
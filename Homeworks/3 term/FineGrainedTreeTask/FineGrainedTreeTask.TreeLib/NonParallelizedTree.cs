namespace TreeTask.TreeLib
{
	public class NonParallelizedTree<T> : ITree<T> // Nonparallelized version, for algorithm commentary see parallelized version
	{
		public int Key { get; internal set; }
		public T Data { get; internal set; }

		public NonParallelizedTree<T> Left { get; internal set; }
		public NonParallelizedTree<T> Right { get; internal set; }

		public NonParallelizedTree(int key = 0, T data = default, NonParallelizedTree<T> left = null, NonParallelizedTree<T> right = null) // Only positive keys, initialization root with 0
		{
			Left = left;
			Right = right;

			Key = key;
			Data = data;
		}
		public NonParallelizedTree(NonParallelizedTree<T> node)
		{
			Left = node.Left;
			Right = node.Right;

			Key = node.Key;
			Data = node.Data;
		}

		public bool Equals(NonParallelizedTree<T> secondTree)
		{
			if (this is null || secondTree is null)
			{
				return false;
			}

			if (Data.Equals(secondTree.Data) && Key == secondTree.Key)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool Search(int key)
		{
			if (key < 1)
			{
				return false; // Exception??
			}

			NonParallelizedTree<T> curr = this;
			NonParallelizedTree<T> next;

			if (key == curr.Key)
			{
				return true;
			}
			else if (key > curr.Key)
			{
				next = curr.Right;
			}
			else
			{
				next = curr.Left;
			}

			while (next is not null)
			{
				curr = next;

				if (key == curr.Key)
				{
					return true;
				}
				else if (key > curr.Key)
				{
					next = curr.Right;
				}
				else
				{
					next = curr.Left;
				}
			}

			return false;
		}
		public bool Insert(int key, T data)
		{
			if (key < 1)
			{
				return false;
			}

			NonParallelizedTree<T> curr = this;
			NonParallelizedTree<T> next;

			if (key == curr.Key || curr.Key == 0)
			{
				curr.Key = key;
				curr.Data = data;

				return true;
			}
			else if (key > curr.Key)
			{
				if (curr.Right is null)
				{
					curr.Right = new NonParallelizedTree<T>(key, data);
					return true;
				}
				next = curr.Right;
			}
			else
			{
				if (curr.Left is null)
				{
					curr.Left = new NonParallelizedTree<T>(key, data);
					return true;
				}
				next = curr.Left;
			}

			while (true)
			{
				curr = next;

				if (key == curr.Key)
				{
					curr.Data = data;
					return true;
				}
				else if (key > curr.Key)
				{
					if (curr.Right is null)
					{
						curr.Right = new NonParallelizedTree<T>(key, data);

						return true;
					}
					next = curr.Right;
				}
				else
				{
					if (curr.Left is null)
					{
						curr.Left = new NonParallelizedTree<T>(key, data);

						return true;
					}
					next = curr.Left;
				}
			}
		}
		public bool Delete(int key)
		{
			if (key < 1)
			{
				return true;
			}

			NonParallelizedTree<T> curr = this;
			NonParallelizedTree<T> next;

			if (key == curr.Key)
			{
				DownNode(curr);

				return true;
			}
			else if (key > curr.Key)
			{
				next = curr.Right;
			}
			else
			{
				next = curr.Left;
			}

			while (next is not null)
			{
				if (key == next.Key)
				{
					DownNode(next, curr);

					return true;
				}
				else
				{
					curr = next;

					if (key > next.Key)
					{
						next = next.Right;
					}
					else
					{
						next = next.Left;
					}
				}
			}

			return true;
		}

		private static void DownNode(NonParallelizedTree<T> node, NonParallelizedTree<T> parent = null)
		{
			if (node.Right is null)
			{
				if (node.Left is not null)
				{
					NonParallelizedTree<T> curr = node.Left;

					node.Key = curr.Key;
					node.Data = curr.Data;

					node.Left = curr.Left;
					node.Right = curr.Right;
				}
				else // if both null
				{
					if (parent is null)
					{
						node.Key = 0;
						node.Data = default;
					}
					else
					{
						if (node.Equals(parent.Right))
						{
							parent.Right = null;
						}
						else
						{
							parent.Left = null;
						}
					}
				}
			}
			else
			{
				NonParallelizedTree<T> pred = parent, curr = node, right = node.Right;

				do // Rotation
				{
					NonParallelizedTree<T> tempNode = new(curr);

					curr.Key = right.Key;
					curr.Data = right.Data;

					curr.Left = tempNode;
					curr.Right = right.Right;

					tempNode.Right = right.Left;

					pred = curr;
					curr = tempNode;
					right = curr.Right;
				}
				while (right is not null);

				NonParallelizedTree<T> left = curr.Left;
				if (left is not null)
				{
					curr.Key = left.Key;
					curr.Data = left.Data;

					curr.Left = left.Left;
					curr.Right = left.Right;
				}
				else
				{
					pred.Left = null;
				}
			}
		}
	}
}
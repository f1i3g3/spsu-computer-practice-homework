using System.Threading;

namespace TreeTask.TreeLib
{
	public class ParallelizedTree<T> : ITree<T> // Parallelized version
	{
		public int Key { get; internal set; }
		public T Data { get; internal set; }

		private readonly Mutex Mutex;

		public ParallelizedTree<T> Left { get; internal set; }
		public ParallelizedTree<T> Right { get; internal set; }

		public ParallelizedTree(int key = 0, T data = default, ParallelizedTree<T> left = null, ParallelizedTree<T> right = null) // Only positive keys, initialization root with 0
		{
			Mutex = new Mutex();

			Left = left;
			Right = right;

			Key = key;
			Data = data;
		}
		public ParallelizedTree(ParallelizedTree<T> node)
		{
			Mutex = new Mutex(); // Mutex for looking

			Left = node.Left;
			Right = node.Right;

			Key = node.Key;
			Data = node.Data;
		}

		public bool Equals(ParallelizedTree<T> secondTree)
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
		} // Checking for equality

		// Mutex methods
		private void Lock()
		{
			Mutex.WaitOne();
		}
		private void Unlock()
		{
			Mutex.ReleaseMutex();
		}

		public bool Search(int key)
		{
			if (key < 1) // Checking for positive key value
			{
				return false;
			}

			// First, we need to check root
			ParallelizedTree<T> curr = null;
			Lock(); // Locking root

			try
			{
				ParallelizedTree<T> next = null;
				curr = this;
				
				if (key == curr.Key) // Found it!
				{
					return true;
				}
				else if (key > curr.Key) // Probably in right subtree
				{
					next = curr.Right;
				}
				else // Probably in left subtree
				{
					next = curr.Left;
				}

				// Then, we need to go deeper
				if (next is not null) // If we can...
				{
					next.Lock(); // Locking next root

					try
					{
						while (next is not null) // While we can
						{
							curr.Unlock(); // Unlocking pred node
							curr = next; // Updating curr node

							if (key == curr.Key) // Found it!
							{
								curr = null;
								return true;
							}
							else if (key > curr.Key) // Probably in right subtree
							{
								next = curr.Right;
							}
							else // Probably in left subtree
							{
								next = curr.Left;
							}
							next?.Lock(); // Trying to lock next
						}
					}
					finally
					{
						next?.Unlock(); // Trying to unlock next
					}
				}
			}
			finally
			{
				curr?.Unlock(); // Trying to unlock curr/root
			}

			return false;
		}
		public bool Insert(int key, T data)
		{
			if (key < 1) // Checking for positive key value
			{
				return false;
			}

			// First, we need to check root
			ParallelizedTree<T> curr = null;
			Lock(); // Locking root

			try
			{
				ParallelizedTree<T> next = null;
				curr = this;

				if (key == curr.Key || curr.Key == 0)  // Found node/empty root, replacing
				{
					curr.Key = key;
					curr.Data = data;

					return true;
				}
				else if (key > curr.Key) // Probably in right subtree
				{
					if (curr.Right is null) // Adding
					{
						curr.Right = new ParallelizedTree<T>(key, data);
						return true;
					}
					next = curr.Right; // Or updating node to check
				}
				else  // Probably in left subtree
				{
					if (curr.Left is null) // Adding
					{
						curr.Left = new ParallelizedTree<T>(key, data);
						return true;
					}
					next = curr.Left; // Or updating node to check
				}
				next.Lock(); // Locking child

				// Go deeper
				try
				{
					while (true) // While we can
					{
						curr.Unlock(); // Unlocking pred node
						curr = next; // Updating curr node

						if (key == curr.Key) // Found it, replacing
						{
							curr.Data = data;

							curr = null;
							return true;
						}
						else if (key > curr.Key) // Probably in right subtree
						{
							if (curr.Right is null) // Adding
							{
								curr.Right = new ParallelizedTree<T>(key, data);

								curr = null;
								return true;
							}
							next = curr.Right; // Or updating node to check
						}
						else // Probably in left subtree
						{
							if (curr.Left is null) // Adding
							{
								curr.Left = new ParallelizedTree<T>(key, data);

								curr = null;
								return true;
							}
							next = curr.Left; // Or updating node to check
						}
						next.Lock(); // Trying to lock next
					}
				}
				finally
				{
					next.Unlock(); // Trying to unlock next
				}
			}
			finally
			{
				curr?.Unlock(); // Trying to unlock curr/root
			}
		}
		public bool Delete(int key)
		{
			if (key < 1) // Checking for positive key value
			{
				return true;
			}

			// Checking root first
			ParallelizedTree<T> curr = null;
			Lock(); // Locking root

			try
			{
				ParallelizedTree<T> next = null;
				curr = this;

				if (key == curr.Key) // Found it!
				{
					DownNode(curr); // Deleting root

					if (curr.Key != 0)
					{
						curr = null;
					}

					return true;
				}
				else if (key > curr.Key) // Probably in right subtree
				{
					next = curr.Right;
				}
				else // Probably in left subtree
				{
					next = curr.Left;
				}

				// If it's not in root, checking other tree
				if (next is not null)
				{
					next.Lock(); // Locking child

					try
					{
						while (next is not null) // While tree has something to delete
						{
							if (key == next.Key) // Found it!
							{
								DownNode(next, curr); // Deleting node

								curr = null; // сделал внутри?
								next = null; // сделал внутри?

								return true;
							}
							else
							{
								curr.Unlock(); // Unlocking curr
								curr = next; // Updating curr

								if (key > next.Key) // Probably in right subtree
								{
									next = next.Right;
								}
								else // Probably in left subtree
								{
									next = next.Left;
								}
								next?.Lock(); // Trying to lock next
							}
						}
					}
					finally
					{
						next?.Unlock(); // Trying to unlock next
					}
				}
			}
			finally
			{
				curr?.Unlock(); // Trying to unlock curr/root
			}

			return true;
		}

		private static void DownNode(ParallelizedTree<T> node, ParallelizedTree<T> parent = null)
		{
			// Both parent and child(node) are locked in external method
			if (node.Right is null) // If right subtree doesn't exist
			{
				ParallelizedTree<T> curr = null;

				try
				{
					curr = node.Left;

					if (curr is not null) // Left subtree exists - updating curr
					{
						curr.Lock();

						node.Key = curr.Key;
						node.Data = curr.Data;

						node.Left = curr.Left;
						node.Right = curr.Right;

						node.Unlock();
					}
					else  // Only if both subtrees null
					{
						curr = node; // Updating curr // сделал внутри?

						if (parent is null) // Root case
						{
							curr.Key = 0;
							curr.Data = default;
						}
						else // Other node case
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
				finally // Unlocking nodes
				{
					parent?.Unlock();
					curr?.Unlock();
				}
			}
			else // If right subtree exists
			{
				ParallelizedTree<T> pred = parent, curr = node, right = node.Right;

				do // Rotation - lowering the node to be removed
				{
					right.Lock(); // Locking right subtree

					try // Rotation itself
					{
						ParallelizedTree<T> tempNode = new(curr);

						curr.Key = right.Key;
						curr.Data = right.Data;

						tempNode.Lock();
						curr.Left = tempNode;

						curr.Right = right.Right;
						tempNode.Right = right.Left;

						pred?.Unlock();
						pred = curr;
						curr = tempNode;
					}
					finally // Unlocking and updaitng right subtree
					{
						right.Unlock();
						right = curr.Right;
					}
				}
				while (right is not null); // While right subtree exists

				// If right subtree doesn't exist - delete curr node
				ParallelizedTree<T> left = curr.Left;
				try
				{
					if (left is not null) // If left subtree exists
					{
						left.Lock();

						curr.Key = left.Key;
						curr.Data = left.Data;

						curr.Left = left.Left;
						curr.Right = left.Right;
					}
					else // If left subtree doesn't exist
					{
						pred.Left = null;
					}
				}
				finally // Trying to unlock unuseful nodes
				{
					pred.Unlock();
					curr.Unlock();

					left?.Unlock();
				}
			}
		}
	}
}
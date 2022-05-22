namespace TreeTask.TreeLib
{
	public interface ITree<T>
	{
		public bool Search(int key);
		public bool Insert(int key, T data);
		public bool Delete(int key);
	}
}
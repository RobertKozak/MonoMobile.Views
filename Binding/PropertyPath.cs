namespace MonoTouch.MVVM
{
	using System;

	public sealed class PropertyPath
	{
		public string Path { get; internal set; }

		public PropertyPath(string path)
		{
			Path = path;
		}
	}
}


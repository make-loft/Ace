#if DESKTOP

using System.Runtime.InteropServices;

namespace Ace
{
    static partial class LE
    {
		public static T To<T>(this byte[] bytes)
		{
			{
				var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

				try
				{
					var structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
					return structure;
				}
				finally
				{
					handle.Free();
				}
			}
		}
	}
}
#endif

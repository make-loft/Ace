using System;

namespace Ace.Patterns
{
	public delegate object ActivationQueryHandler(string key, Type type, params object[] constructorArgs);

	public delegate void DecodeFailedHandler(string key, Type type, Exception exception);
	
	public delegate void EncodeFailedHandler(string key, object item, Exception exception);
	
	public interface IMemoryBox
	{
		event ActivationQueryHandler ActivationRequired;
		event DecodeFailedHandler DecodeFailed;
		event EncodeFailedHandler EncodeFailed;
		
		bool Check<TItem>(string key = null);
		void Destroy<TItem>(string key = null);
		void Keep<TItem>(TItem item, string key = null);
		object Revive(string key, Type type, params object[] constructorArgs);
	}
}
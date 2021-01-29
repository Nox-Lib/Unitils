using System;
using System.Collections.Generic;

namespace Unitils
{
	public class ServiceLocator
	{
		private static ServiceLocator instance = null;
		public static ServiceLocator Instance => instance ??= new ServiceLocator();

		private readonly Dictionary<Type, object> container = new Dictionary<Type, object>();

		public T GetService<T>() where T : class
		{
			return this.container[typeof(T)] as T;
		}

		public void Register<T>(T service) where T : class
		{
			Type type = typeof(T);
			if (this.container.ContainsKey(type)) {
				this.container[type] = service;
			}
			else {
				this.container.Add(type, service);
			}
		}
	}
}
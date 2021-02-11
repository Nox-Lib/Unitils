using System.Collections;

namespace Unitils
{
	public interface ISceneBase
	{
		public MemoryCache ViewCache { get; }

		IEnumerator OnBeforeEnter(object arg, MemoryCache viewCache);
		IEnumerator OnBeforeExit();
		void OnBack();
	}
}
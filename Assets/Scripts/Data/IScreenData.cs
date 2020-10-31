using UnityEngine;

namespace Unitils
{
	public interface IScreenData
	{
		Define.ScreenMode ScreenMode { get; }
		Vector2 BaseScreenSize { get; }
		Vector2 MinScreenSize { get; }
		Vector2 MaxScreenSize { get; }
	}
}
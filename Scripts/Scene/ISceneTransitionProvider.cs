namespace Unitils
{
	public interface ISceneTransitionProvider
	{
		bool IsRunning { get; }
		string BeforeSceneName { get; }

		void Next(string sceneName, object arg = null);
		void Back(object arg = null);
		void AddSubScene(string sceneName);
		void RemoveSubScene(string sceneName);
		void RemoveSubSceneAll();
		void ResetPreviousSceneHistories();
	}
}
namespace Unitils
{
	public interface ISceneTransitionProvider
	{
		bool IsRunning { get; }
		string ActiveSceneName { get; }
		string BeforeSceneName { get; }

		void Next(string sceneName, object arg = null);
		void Back(object arg = null);
		bool IsActiveSubScene(string sceneName);
		void AddSubScene(string sceneName);
		void RemoveSubScene(string sceneName);
		void RemoveSubSceneAll();
		void ResetPreviousSceneHistories();
		void SendBackEvent();
	}
}
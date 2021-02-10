using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unitils
{
	public class SceneTransitioner : MonoBehaviour, ISceneTransitionProvider
	{
		private class History
		{
			public string sceneName;
			public string beforeSceneName;
			public object arg;
			public MemoryCache viewCache;
		}

		private History currentHistory;
		private ISceneBase currentScene;

		private readonly Stack<History> previousSceneHistories = new Stack<History>();
		private readonly List<string> addSubSceneNames = new List<string>();


		#region ISceneTransitionProvider

		public bool IsRunning { get; private set; }
		public string BeforeSceneName => this.currentHistory?.beforeSceneName;

		public void Next(string sceneName, object arg = null)
		{
			History next = new History { sceneName = sceneName, arg = arg };

			if (this.currentHistory != null) {
				next.beforeSceneName = this.currentHistory.beforeSceneName;
				if (this.currentScene != null) {
					this.currentHistory.viewCache = this.currentScene.ViewCache;
				}
			}
			this.previousSceneHistories.Push(this.currentHistory);

			StartCoroutine(this.RunNext(next));
		}

		public void Back(object arg = null)
		{
			if (this.previousSceneHistories.Count <= 0) return;
			History next = this.previousSceneHistories.Pop();
			StartCoroutine(this.RunNext(next));
		}

		public void AddSubScene(string sceneName)
		{
			if (this.addSubSceneNames.Contains(sceneName)) return;
			SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			this.addSubSceneNames.Add(sceneName);
		}

		public void RemoveSubScene(string sceneName)
		{
			if (!this.addSubSceneNames.Contains(sceneName)) return;
			SceneManager.UnloadSceneAsync(sceneName);
			this.addSubSceneNames.Remove(sceneName);
		}

		public void RemoveSubSceneAll()
		{
			if (this.addSubSceneNames.Count <= 0) return;
			this.addSubSceneNames.ForEach(_ => SceneManager.UnloadSceneAsync(_));
			this.addSubSceneNames.Clear();
		}

		public void ResetPreviousSceneHistories()
		{
			this.previousSceneHistories.Clear();
		}

		#endregion


		private IEnumerator RunNext(History next)
		{
			this.IsRunning = true;

			if (this.currentScene != null) {
				yield return this.currentScene.OnBeforeExit();
			}

			yield return SceneManager.LoadSceneAsync(next.sceneName, LoadSceneMode.Additive);

			Scene scene = SceneManager.GetSceneByName(next.sceneName);
			SceneManager.SetActiveScene(scene);

			if (this.currentHistory != null) {
				yield return SceneManager.UnloadSceneAsync(this.currentHistory.sceneName);
			}
			this.currentHistory = next;

			GameObject[] rootObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootObjects.Length && this.currentScene == null; i++) {
				this.currentScene = rootObjects[i].GetComponent<ISceneBase>();
			}

			if (this.currentScene != null) {
				yield return this.currentScene.OnBeforeEnter(next.arg, next.viewCache);
			}
			this.IsRunning = false;
		}
	}
}
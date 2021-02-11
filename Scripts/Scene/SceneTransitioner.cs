using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unitils
{
	public class SceneTransitioner : MonoBehaviour, ISceneTransitionProvider
	{
		private static SceneTransitioner instance = null;

		public static void Activation()
		{
			if (instance == null) {
				instance = Utils.Unity.CreateGameObject<SceneTransitioner>("SceneTransitioner");
				DontDestroyOnLoad(instance.gameObject);
			}
			ServiceLocator.Instance.Register<ISceneTransitionProvider>(instance);
		}


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


		private void Awake()
		{
			Scene scene = SceneManager.GetActiveScene();

			this.currentHistory = new History { sceneName = scene.name };

			GameObject[] rootObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootObjects.Length && this.currentScene == null; i++) {
				this.currentScene = rootObjects[i].GetComponent<ISceneBase>();
			}
		}


		#region ISceneTransitionProvider

		public bool IsRunning { get; private set; }

		public string ActiveSceneName => this.currentHistory?.sceneName;
		public string BeforeSceneName => this.currentHistory?.beforeSceneName;

		public void Next(string sceneName, object arg = null)
		{
			if (this.IsRunning) return;

			History next = new History { sceneName = sceneName, arg = arg };

			if (this.currentHistory != null) {
				next.beforeSceneName = this.currentHistory.sceneName;
				if (this.currentScene != null) {
					this.currentHistory.viewCache = this.currentScene.ViewCache;
				}
			}
			this.previousSceneHistories.Push(this.currentHistory);

			StartCoroutine(this.RunNext(next));
		}

		public void Back(object arg = null)
		{
			if (this.IsRunning || this.previousSceneHistories.Count <= 0) return;
			History next = this.previousSceneHistories.Pop();
			StartCoroutine(this.RunNext(next));
		}

		public bool IsActiveSubScene(string sceneName)
		{
			return this.addSubSceneNames.Contains(sceneName);
		}

		public void AddSubScene(string sceneName)
		{
			if (this.IsActiveSubScene(sceneName)) return;
			SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			this.addSubSceneNames.Add(sceneName);
		}

		public void RemoveSubScene(string sceneName)
		{
			if (!this.IsActiveSubScene(sceneName)) return;
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

		public void SendBackEvent()
		{
			this.currentScene?.OnBack();
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
			this.currentScene = null;
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
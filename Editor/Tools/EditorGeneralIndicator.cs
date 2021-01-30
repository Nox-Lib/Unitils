using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Unitils
{
	public class EditorGeneralIndicator
	{
		public class Task
		{
			public Action Job { get; }
			public string Description { get; }

			public Task(Action job, string description)
			{
				this.Job = job;
				this.Description = description;
			}
		}

		private List<Task> tasks;
		private Action onComplete;
		private string indicatorTitle;
		private int cursor;

		private static EditorGeneralIndicator runtime = null;


		public static void Show(string title, List<Task> tasks, Action onComplete)
		{
			if (runtime != null) {
				throw new Exception("[EditorGeneralIndicator] cannot run multiple progress bars.");
			}
			if (tasks == null || tasks.Count <= 0) {
				onComplete?.Invoke();
				return;
			}

			runtime = new EditorGeneralIndicator
			{
				indicatorTitle = title,
				tasks = tasks,
				onComplete = onComplete,
				cursor = 0
			};
			EditorCoroutine.Start(runtime.Run());
		}


		private void SetProgressBar()
		{
			if (this.cursor < this.tasks.Count) {
				string progressTest = $"({this.cursor}/{this.tasks.Count})";
				string title = string.IsNullOrEmpty(this.indicatorTitle) ? progressTest : $"{this.indicatorTitle} {progressTest}";
				float progress = (float)this.cursor / this.tasks.Count;
				EditorUtility.DisplayProgressBar(title, this.tasks[this.cursor].Description, progress);
			}
			else {
				EditorUtility.ClearProgressBar();
			}
		}


		private IEnumerator Run()
		{
			this.SetProgressBar();
			yield return null;

			while (this.cursor < this.tasks.Count) {
				try {
					this.tasks[this.cursor].Job();
				}
				catch {
					EditorUtility.ClearProgressBar();
					runtime = null;
					throw;
				}
				this.cursor++;
				this.SetProgressBar();
				yield return null;
			}
			this.SetProgressBar();

			runtime = null;
			this.onComplete?.Invoke();

			yield break;
		}
	}
}
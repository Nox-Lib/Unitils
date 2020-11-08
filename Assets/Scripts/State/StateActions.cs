using System;
using System.Collections.Generic;

namespace Unitils
{
	public class StateActions<TState>
	{
		private readonly Dictionary<TState, Action<bool>> stateProcesses;
		private Action<bool> currentProcess;
		private int processCalledCount;
		private bool isStarted;

		public TState ActiveState { get; private set; }
		public TState BeforeState { get; private set; }
		public bool IsPause { get; private set; }

		public event Action<TState> OnNextState;

		public StateActions(Dictionary<TState, Action<bool>> processes)
		{
			this.stateProcesses = processes;
			this.isStarted = this.IsPause = false;
		}

		public void Start(TState firstState)
		{
			if (this.isStarted) return;
			this.isStarted = true;
			this.Next(firstState);
		}

		public void Pause(bool isPause)
		{
			this.IsPause = isPause;
		}

		public bool Equals(TState state)
		{
			return this.ActiveState.Equals(state);
		}

		public void Update()
		{
			if (!this.isStarted || this.IsPause || this.currentProcess == null) return;

			bool isFirst = this.processCalledCount <= 0;
			this.processCalledCount++;
			this.currentProcess(isFirst);
		}

		public void Next(TState nextState)
		{
			if (!this.isStarted || this.IsPause) return;
			this.BeforeState = this.ActiveState;
			this.ActiveState = nextState;
			this.ChangeProcess();
		}

		private void ChangeProcess()
		{
			this.currentProcess = null;
			if (this.stateProcesses != null && this.stateProcesses.ContainsKey(this.ActiveState)) {
				this.currentProcess = this.stateProcesses[this.ActiveState];
				this.processCalledCount = 0;
			}
			this.OnNextState?.Invoke(this.ActiveState);
		}
	}
}
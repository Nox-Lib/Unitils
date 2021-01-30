using System;
using System.Collections.Generic;

namespace Unitils
{
	public abstract class StateClass<TState>
	{
		protected IStateClassSequencer<TState> sequencer;

		protected virtual void Begin() {}
		protected virtual void Update() {}
		protected virtual void End() {}

		public class Sequence<TClass> : IStateClassSequencer<TState> where TClass : StateClass<TState>
		{
			private readonly Dictionary<TState, TClass> stateProcesses;

			private bool isStarted;

			public TClass Current { get; private set; }
			public TState ActiveState { get; private set; }
			public TState BeforeState { get; private set; }
			public bool IsPause { get; private set; }

			public event Action<TState> OnNextState;

			public Sequence(Dictionary<TState, TClass> processes)
			{
				this.stateProcesses = processes;
				if (this.stateProcesses != null) {
					foreach (StateClass<TState> stateClass in this.stateProcesses.Values) {
						stateClass.sequencer = this;
					}
				}
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
				if (this.isStarted && !this.IsPause && this.Current != null) {
					this.Current.Update();
				}
			}

			#region IStateClassSequencer
			public void Next(TState nextState)
			{
				if (!this.isStarted || this.IsPause) return;
				this.BeforeState = this.ActiveState;
				this.ActiveState = nextState;
				this.ChangeState();
			}
			#endregion

			private void ChangeState()
			{
				if (this.Current != null) {
					this.Current.End();
				}
				this.Current = null;
				if (this.stateProcesses != null && this.stateProcesses.ContainsKey(this.ActiveState)) {
					this.Current = this.stateProcesses[this.ActiveState];
				}
				if (this.Current != null) {
					this.Current.Begin();
				}
				this.OnNextState?.Invoke(this.ActiveState);
			}
		}
	}


	public abstract class StateClass<TState, TArg>
	{
		protected IStateClassSequencer<TState, TArg> sequencer;

		protected virtual void Begin(TArg arg) {}
		protected virtual void Update() {}
		protected virtual void End() {}

		public class Sequence<TClass> : IStateClassSequencer<TState, TArg> where TClass : StateClass<TState, TArg>
		{
			private readonly Dictionary<TState, TClass> stateProcesses;

			private bool isStarted;

			public TClass Current { get; private set; }

			public TState ActiveState { get; private set; }
			public TState BeforeState { get; private set; }
			public bool IsPause { get; private set; }

			public event Action<TState> OnNextState;

			public Sequence(Dictionary<TState, TClass> processes)
			{
				this.stateProcesses = processes;
				if (this.stateProcesses != null) {
					foreach (StateClass<TState, TArg> stateClass in this.stateProcesses.Values) {
						stateClass.sequencer = this;
					}
				}
				this.isStarted = this.IsPause = false;
			}

			public void Start(TState firstState, TArg arg)
			{
				if (this.isStarted) return;
				this.isStarted = true;
				this.Next(firstState, arg);
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
				if (this.isStarted && !this.IsPause && this.Current != null) {
					this.Current.Update();
				}
			}

			#region IStateClassSequencer
			public void Next(TState nextState, TArg arg)
			{
				if (!this.isStarted || this.IsPause) return;
				this.BeforeState = this.ActiveState;
				this.ActiveState = nextState;
				this.ChangeState(arg);
			}
			#endregion

			private void ChangeState(TArg arg)
			{
				if (this.Current != null) {
					this.Current.End();
				}
				this.Current = null;
				if (this.stateProcesses != null && this.stateProcesses.ContainsKey(this.ActiveState)) {
					this.Current = this.stateProcesses[this.ActiveState];
				}
				if (this.Current != null) {
					this.Current.Begin(arg);
				}
				this.OnNextState?.Invoke(this.ActiveState);
			}
		}
	}
}
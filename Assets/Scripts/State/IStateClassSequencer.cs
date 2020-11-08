namespace Unitils
{
	public interface IStateClassSequencer<TState>
	{
		void Next(TState nextState);
	}

	public interface IStateClassSequencer<TState, TArgs>
	{
		void Next(TState nextState, TArgs args);
	}
}
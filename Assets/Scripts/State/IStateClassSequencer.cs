namespace Unitils
{
	public interface IStateClassSequencer<TState>
	{
		void Next(TState nextState);
	}

	public interface IStateClassSequencer<TState, TArg>
	{
		void Next(TState nextState, TArg arg);
	}
}
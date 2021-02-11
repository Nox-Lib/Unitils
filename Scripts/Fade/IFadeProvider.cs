namespace Unitils
{
	public interface IFadeProvider
	{
		bool IsRunning { get; }
		void SetConfiguration(object arg);
		void SetFactor(float factor);
		void FadeIn();
		void FadeOut();
	}
}
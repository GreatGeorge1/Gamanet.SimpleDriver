namespace DevStuff.Interfcaces
{
    public interface ITransport<TMessage> : IInput<TMessage>, IOutput
    {
        void Reset();
    }
}
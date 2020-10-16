namespace DevStuff.Interfaces
{
    public interface ITransport<TMessage> : IInput<TMessage>, IOutput
    {
        void Reset();
    }
}
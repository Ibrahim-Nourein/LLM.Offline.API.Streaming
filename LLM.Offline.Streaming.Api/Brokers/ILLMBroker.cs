namespace LLM.Offline.Streaming.Api.Brokers
{
    public interface ILLMBroker
    {
        IAsyncEnumerable<string> PromptAsync(string message);
    }
}

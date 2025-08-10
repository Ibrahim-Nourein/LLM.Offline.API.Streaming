using LLama.Common;
using LLama.Sampling;
using LLama;

namespace LLM.Offline.Streaming.Api.Brokers
{
    public class LLMBroker : ILLMBroker
    {
        // hold onto everything so we can dispose properly
        private readonly LLamaWeights modelWeights;
        private readonly LLamaContext llamaContext;
        private readonly InteractiveExecutor executor;
        private readonly InferenceParams inferenceParams;
        private readonly ChatSession session;
        private readonly ChatHistory history;

        public LLMBroker()
        {
            string modelPath = "mistral-7b-instruct-v0.1.Q8_0.gguf";
            uint? ctxSize = 2048;
            int gpuLayers = 128;

            var mp = new ModelParams(modelPath)
            {
                ContextSize = ctxSize,
                GpuLayerCount = gpuLayers
            };

            modelWeights = LLamaWeights.LoadFromFile(mp);
            llamaContext = modelWeights.CreateContext(mp);
            executor = new InteractiveExecutor(llamaContext);
            history = new ChatHistory();

            history.AddMessage(
                authorRole: AuthorRole.System,
                content: @"You are a helpful assistant.");


            session = new ChatSession(executor, history);

            inferenceParams = new InferenceParams
            {
                MaxTokens = 256,

                SamplingPipeline = new DefaultSamplingPipeline
                {
                    Temperature = 0.6f,
                    TopK = 40
                },

                AntiPrompts = new List<string> { "()" }, // Cut off early
            };
        }

        public void Dispose()
        {
            llamaContext.Dispose();
            modelWeights.Dispose();
        }

        public IAsyncEnumerable<string> PromptAsync(string userMessage)
        {
            var message = new ChatHistory
                .Message(AuthorRole.User, userMessage);

            return session.ChatAsync(message, inferenceParams);
        }
    }
}

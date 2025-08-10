using LLM.Offline.Streaming.Api.Brokers;
using Microsoft.AspNetCore.Mvc;

namespace LLM.Offline.Streaming.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatsController : ControllerBase
    {
        private readonly ILLMBroker llmBroker;

        public ChatsController(ILLMBroker llmBroker) =>
            this.llmBroker = llmBroker;

        [HttpPost]
        public IAsyncEnumerable<string> PostPromptAsync([FromBody] string prompt)
        {
            return this.llmBroker.PromptAsync(prompt);
        }
    }
}

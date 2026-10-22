namespace NizzolaMAFSamples
{
    public interface IMAFService
    {
        Task<string> MCPRunAsync(string userInput);
        Task<string> MCPRunXLSAsync(string userInput);
        Task SingleChat(string question);
        Task WithChatMessages();
        Task WithFunctionTools(string question);
        Task WithStream(string question);
    }
}
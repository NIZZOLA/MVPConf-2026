using Azure.AI.Agents.Persistent;
using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
using ConsoleAgentFramework;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using OpenAI.Responses;
using System.Net;
using System.Security.Cryptography;

namespace NizzolaMAFSamples;

public class MAFService : IMAFService
{
    private AIAgent _agent;
    private string _endpoint;
    private string _deploymentName;

    public MAFService(IConfiguration configuration)
    {
        _endpoint = configuration["AZURE_OPENAI_ENDPOINT"] //Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
                            ?? throw new InvalidOperationException("Configure AZURE_OPENAI_ENDPOINT");
        _deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT_NAME"] //Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME")
                                                                           ?? "gpt-4o-mini";

        var agentInstructions = "Você é um agente que recebe perguntas e responde de forma engraçada";

        _agent = new AIProjectClient(new Uri(_endpoint), new DefaultAzureCredential())
           .AsAIAgent(
               model: _deploymentName,
               instructions: agentInstructions,
               name: "HelloAgent");
    }

    public async Task SingleChat(string question)
    {
        Console.WriteLine($"Pergunta ao agente: {question}");
        Console.Write("Resposta do agente: ");
        Console.WriteLine(await _agent.RunAsync(question));
    }

    public async Task WithStream(string question)
    {
        Console.WriteLine($"Pergunta ao agente: {question}");
        Console.Write("Resposta do agente: ");

        await foreach (var update in _agent.RunStreamingAsync(question))
        {
            Console.Write(update);
        }
    }

    public async Task WithChatMessages()
    {
        ChatMessage message = new(ChatRole.User, [
            new TextContent("Faça uma piada sobre esta imagem ?"),
            new UriContent("https://upload.wikimedia.org/wikipedia/commons/1/11/Joseph_Grimaldi.jpg", "image/jpeg")
        ]);

        Console.WriteLine(await _agent.RunAsync(message));
    }

    public async Task WithFunctionTools(string question)
    {
        AIAgent agent = new AIProjectClient(new Uri(_endpoint), new DefaultAzureCredential())
            .AsAIAgent(
                model: _deploymentName,
                instructions: "Você é um assistente útil.",
                tools: [AIFunctionFactory.Create(FunctionToolsSamples.GetWeather)]);

        Console.WriteLine($"Pergunta ao agente: {question}");
        Console.Write("Resposta do agente: ");

        Console.WriteLine(await agent.RunAsync(question));
    }

    public async Task<string> MCPRunAsync(string userInput)
    {
        const string mcpServerUrl = "https://learn.microsoft.com/api/mcp";
        const string mcpServerLabel = "Microsoft Learn MCP Client";
        var agentInstructions = "You are a helpful assistant that can use the microsoft_docs_search tool" +
            " to search for Microsoft documentation to answer user questions.";

        var clientTransport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(mcpServerUrl),
            Name = mcpServerLabel
        });

        var client = await McpClient.CreateAsync(clientTransport);

        var tools = await client.ListToolsAsync();

        var agent = new AzureOpenAIClient(
                new Uri(_endpoint),
                new DefaultAzureCredential())
            .GetChatClient(_deploymentName)
            .AsIChatClient().AsAIAgent(
                instructions: agentInstructions,
                tools: [.. tools]);

        AgentSession session;
        //if (state is null)
        session = await agent.CreateSessionAsync();
        //  else
        //      session = await agent.DeserializeSessionAsync(state.Value);

        var userMessage = new ChatMessage(ChatRole.User,
            userInput);

        var response = await agent.RunAsync([userMessage], session);

        var serializedResponse = await agent.SerializeSessionAsync(session);
        Console.WriteLine(response);
        return serializedResponse.ToString();
    }

    public async Task<string> MCPRunXLSAsync(string userInput)
    {
        const string mcpServerUrl = "http://127.0.0.1:8000/mcp";
        const string mcpServerLabel = "XLS INFERENCE";
        var agentInstructions = "você é um agente que é capaz de responder perguntas obtendo informações de arquivos XLS";

        var clientTransport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(mcpServerUrl),
            Name = mcpServerLabel
        });

        var client = await McpClient.CreateAsync(clientTransport);

        var tools = await client.ListToolsAsync();

        var agent = new AzureOpenAIClient(
                new Uri(_endpoint),
                new DefaultAzureCredential())
            .GetChatClient(_deploymentName)
            .AsIChatClient().AsAIAgent(
                instructions: agentInstructions,
                tools: [.. tools]);

        AgentSession session;
        //if (state is null)
        session = await agent.CreateSessionAsync();
        //  else
        //      session = await agent.DeserializeSessionAsync(state.Value);

        var userMessage = new ChatMessage(ChatRole.User,
            userInput);

        var response = await agent.RunAsync([userMessage], session);

        var serializedResponse = await agent.SerializeSessionAsync(session);
        Console.WriteLine(response);
        return serializedResponse.ToString();
    }
}

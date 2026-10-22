using Microsoft.Extensions.Configuration;
using NizzolaMAFSamples;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration config = builder.Build();

var service = new MAFService(config);
/*
// amostra 01
await service.SingleChat("Conte uma piada sobre carro da Peugeot");

Console.ReadKey();
// amostra 02
await service.WithStream("Conte uma piada de fusca");
Console.ReadKey();
*/

// amostra 03
Console.WriteLine("Digite a cidade e estado (ex: Itu/SP) para consultar a temperatura atual:");
var cidade = Console.ReadLine();
await service.WithFunctionTools($"Qual a temperatura hoje em {cidade} ?");
/*
// amostra 04
await service.MCPRunAsync("Resuma quais são as principais funcionalidades do Microsoft Agent Framework para conversação");

await service.MCPRunXLSAsync("Quais são os arquivos da pasta e os dados que cada um contem em suas abas?");
await service.MCPRunXLSAsync("Quais são os campos da planilha sample na aba de funcionarios?");
*/
Console.ReadKey();
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// One client for the Foundry resource; each deployment is reached through it by name.
var azure = new AzureOpenAIClient(
    new Uri(config["AzureOpenAI:Endpoint"]!),
    new AzureKeyCredential(config["AzureOpenAI:Key"]!));

// The same Microsoft.Extensions.AI interfaces as CarrierHub: only the provider is different.(
IChatClient chat = azure.GetChatClient(config["AzureOpenAI:ChatDeployment"]!).AsIChatClient();
IEmbeddingGenerator<string, Embedding<float>> embedder =
    azure.GetEmbeddingClient(config["AzureOpenAI:EmbeddingDeployment"]!).AsIEmbeddingGenerator();

// 1. The chat model writes text.
var response = await chat.GetResponseAsync("In one sentence: what is retrieval-augmented generation?");
Console.WriteLine(response.Text);
Console.WriteLine($"Tokens: {response.Usage?.InputTokenCount} in, {response.Usage?.OutputTokenCount} out");
Console.WriteLine();

// 2. The embedding model turns text into numbers that represent meaning.
string[] texts = ["My parcel arrived broken", "Damaged shipment claim", "Invoice payment terms"];
var vectors = await embedder.GenerateAsync(texts);
Console.WriteLine($"Each text became {vectors[0].Vector.Length} numbers.");

// Similar meaning -> similar numbers -> cosine similarity close to 1.
for (var i = 1; i < texts.Length; i++)
    Console.WriteLine($"\"{texts[0]}\" vs \"{texts[i]}\": {Cosine(vectors[0].Vector.Span, vectors[i].Vector.Span):0.000}");

static double Cosine(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
{
    double dot = 0, lengthA = 0, lengthB = 0;
    for (var i = 0; i < a.Length; i++)
    {
        dot += a[i] * b[i];
        lengthA += a[i] * a[i];
        lengthB += b[i] * b[i];
    }
    return dot / (Math.Sqrt(lengthA) * Math.Sqrt(lengthB));
}
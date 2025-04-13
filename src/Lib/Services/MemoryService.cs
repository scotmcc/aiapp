using AIApp.Lib.Entities;
using AIApp.Lib.Interfaces;
using AIApp.Lib.Models;
using OllamaSharp.Models.Chat;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AIApp.Helpers;
using OllamaSharp;
using OllamaSharp.Models;

namespace AIApp.Lib.Services
{
    public class MemoryService(IDbContextFactory<DataContext> factory, IMapper mapper) : IMemoryService
    {
        private readonly IDbContextFactory<DataContext> _factory = factory;
        private readonly IMapper _mapper = mapper;
        public event Action<string>? OnUpdate;
        public Tool SaveMemoryTool => new()
        {
            Function = new()
            {
                Name = "save_memory",
                Description = "Use this tool to save a memory to the database.",
                Parameters = new()
                {
                    Type = "object",
                    Properties = new()
                    {
                        { "text", new() { Type = "string", Description = "Vivid and descriptive text to save as memory." } },
                        { "subject", new() { Type = "string", Description = "Subject of the memory, usually a proper name or event." } }
                    },
                    Required = ["text", "subject"]
                }
            }
        };
        public Tool FindMemoryTool => new()
        {
            Function = new()
            {
                Name = "find_memory",
                Description = "Use this tool to find similar memories in the database.",
                Parameters = new()
                {
                    Type = "object",
                    Properties = new()
                    {
                        { "text", new() { Type = "string", Description = "Text to use to find similar memories." } }
                    },
                    Required = ["text"]
                }
            }
        };
        public async Task CreateMemory(MemoryModel memory)
        {
            using var context = _factory.CreateDbContext();
            MemoryEntity entity = await SimilaritySearch(memory, 1, 0.8) ?? _mapper.Map<MemoryEntity>(memory);
            entity.Vector = await VectorHelper.VectorizeAsync(memory.Content);
            entity.Keywords = await KeywordHelper.ExtractKeywordsAsync(memory.Content);
            await context.Memories.AddAsync(entity);
            await context.SaveChangesAsync();
        }
        public async Task CreateMemories(List<MemoryModel> memories)
        {
            using var context = _factory.CreateDbContext();
            List<MemoryEntity> MemoryList = [];
            foreach (var memory in memories)
            {
                MemoryEntity entity = await SimilaritySearch(memory, 1, 0.8) ?? _mapper.Map<MemoryEntity>(memory);
                entity.Vector = await VectorHelper.VectorizeAsync(memory.Content);
                entity.Keywords = await KeywordHelper.ExtractKeywordsAsync(memory.Content);
                MemoryList.Add(entity);
            }
            await context.Memories.AddRangeAsync(MemoryList);
            await context.SaveChangesAsync();
        }
        private static async Task<string> MergeMemory(string existing, string updated)
        {
            OllamaApiClient ollama = new("http://host.docker.internal:11434", "llama3.2:3b-unc");
            GenerateRequest request = new()
            {
                System = "Merge the following two memories a single coherent memory. Do not repeat any information. Ensure that the merged memory is vivid and descriptive. Do not explain, add any additional information, or provide any context.",
                Prompt = $"**Current Memory**: {existing}\n\n**Text to Include**: {updated}",
                Options = new()
                {
                    Temperature = 0.1f
                }
            };
            var response = await ollama.GenerateAsync(request).StreamToEndAsync();
            return response?.Response ?? existing;
        }
        public async Task<List<MemoryModel>> FindMemories(string content, int count = 5, double threshold = 0.5)
        {
            using var context = _factory.CreateDbContext();
            var memories = await context.Memories.ToListAsync();
            var similarMemories = await VectorHelper.FindSimilarAsync(content, memories, count, threshold);
            return _mapper.Map<List<MemoryModel>>(similarMemories);
        }
        private async Task<MemoryEntity?> SimilaritySearch(MemoryModel memory, int count = 5, double threshold = 0.5)
        {
            using var context = _factory.CreateDbContext();
            var existing = await context.Memories.FirstOrDefaultAsync(m => m.Subject == memory.Subject);
            if (existing != null) return existing;
            var memories = await context.Memories.ToListAsync();
            var vector = await VectorHelper.VectorizeAsync(memory.Content);
            var similarMemories = VectorHelper.FindSimilar(vector, memories, count, threshold);
            if (similarMemories.Count > 0)
            {
                return similarMemories[0];
            }
            return null;
        }
    }
}

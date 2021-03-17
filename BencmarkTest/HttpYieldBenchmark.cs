using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace BencmarkTest
{
    [NativeMemoryProfiler]
    [ThreadingDiagnoser]
    [MemoryDiagnoser]
    /// <summary>
    /// Test http-request perfomance
    /// </summary>
    public class HttpYieldBenchmark
    {
        private  HttpClient _httpClient;

        [GlobalSetup]
        public void Setup()
        {
            _httpClient = new HttpClient 
            { 
                BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")      
            };

            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Benchmark]
        public async Task CollectionYield()
        {

           await foreach (var item in ReturnYield())
            {
            }
        }

        [Benchmark(Baseline =true)]
        public async Task CollectionLoop()
        {      
            foreach (var item in await ReturnList())
            {
            }
        }

        public async Task<IEnumerable<string>> ReturnList()
        {
            List<string> returnedList = new();

            using var response = await _httpClient.GetAsync("comments", HttpCompletionOption.ResponseHeadersRead);
            using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
            using JsonDocument jDocument = await JsonDocument.ParseAsync(streamToReadFrom);
            var root = jDocument.RootElement.EnumerateArray();

            foreach (var item in root)            
                returnedList.Add( item.GetProperty("body").GetString());

            return returnedList;
        }

        public async IAsyncEnumerable<string> ReturnYield()
        {
            using var response = await _httpClient.GetAsync("comments", HttpCompletionOption.ResponseHeadersRead);
            using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
            using JsonDocument jDocument = await JsonDocument.ParseAsync(streamToReadFrom);
            var root = jDocument.RootElement.EnumerateArray();

            foreach (var item in root)
              yield return   item.GetProperty("body").GetString();

        }
    }
}

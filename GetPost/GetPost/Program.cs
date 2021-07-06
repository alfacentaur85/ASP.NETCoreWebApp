using System;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace GetPost
{
    public  class JsonStructure
    {
        public string UserId { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
    class Program
    {
        private static readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private static readonly HttpClient _client = new HttpClient();
        private static readonly String _fileName = "result.txt";
        private static readonly int _startCounter = 4;
        private static readonly int _finishCounter = 13;


        public static async Task Main(string[] args)
        {          
                _cts.CancelAfter(10000);
                for (int i = _startCounter; i <= _finishCounter; i++)
                {
                    await MakeACallToPostAndWriteToFile(i);
                }

                await ReadFromFile();
                
                _cts.Dispose();
        }
        static async Task MakeACallToPostAndWriteToFile(int id)
        {
            try
            {
                var response = await _client.GetAsync($"https://jsonplaceholder.typicode.com/posts/{id}", _cts.Token);
                var content = await response.Content.ReadAsStringAsync();

                var json = JsonConvert.DeserializeObject<JsonStructure>(content);

                await WriteToFile(String.Concat(json.UserId, "\n", json.Id, "\n", json.Title, "\n", json.Body, "\n\n"));
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Fetching posts is cancelled...");
            }
        }


        static async Task  WriteToFile(String content)
        {
            using (FileStream fstream = new FileStream($"{_fileName}", FileMode.Append))
            {
                try
                {
                    // преобразуем строку в байты
                    byte[] array = System.Text.Encoding.Default.GetBytes(content);
                    // асинхронная запись массива байтов в файл
                    await fstream.WriteAsync(array, 0, array.Length);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Cannot write to file...");
                }
                    
            }
        }

        static async Task ReadFromFile()
        {
            using (FileStream fstream = File.OpenRead($"{_fileName}"))
            {
                try
                {
                    byte[] array = new byte[fstream.Length];
                    // асинхронное чтение файла
                    await fstream.ReadAsync(array, 0, array.Length);

                    string textFromFile = System.Text.Encoding.Default.GetString(array);

                    Console.WriteLine($"{textFromFile}");
                }
                catch
                {
                    Console.WriteLine("Cannot read from file...");
                }
            }
        }

    }
}

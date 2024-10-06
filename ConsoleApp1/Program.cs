using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            
            Console.Write("add start: ");
            string inputFile = Console.ReadLine()?.Trim('"');

            
            if (string.IsNullOrWhiteSpace(inputFile))
            {
                Console.WriteLine("Путь к входному файлу не может быть пустым.");
                return;
            }

            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Входной файл \"{inputFile}\" не существует.");
                return;
            }

            
            Console.Write("add final: ");
            string outputFile = Console.ReadLine()?.Trim('"');

            
            if (string.IsNullOrWhiteSpace(outputFile))
            {
                Console.WriteLine("Путь к выходному файлу не может быть пустым.");
                return;
            }

            
            var words = ReadFile(inputFile);

            
            var frequencyDictionary = CalculateWordFrequencies(words);

            
            var sortedDictionary = frequencyDictionary.OrderByDescending(kv => kv.Value);

            
            WriteToFile(outputFile, sortedDictionary);

            Console.WriteLine($"Частотный словарь успешно записан в \"{outputFile}\".");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static string[] ReadFile(string path)
    {
        var fileContent = File.ReadAllText(path, Encoding.GetEncoding("windows-1251"));
        var words = fileContent
            .Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.ToLower())
            .ToArray();
        return words;
    }

    static ConcurrentDictionary<string, int> CalculateWordFrequencies(string[] words)
    {
        var frequencyDictionary = new ConcurrentDictionary<string, int>();

        Parallel.ForEach(words, word =>
        {
            frequencyDictionary.AddOrUpdate(word, 1, (key, oldValue) => oldValue + 1);
        });

        return frequencyDictionary;
    }

    static void WriteToFile(string path, IOrderedEnumerable<KeyValuePair<string, int>> sortedDictionary)
    {
        using (var writer = new StreamWriter(path, false, Encoding.GetEncoding("windows-1251")))
        {
            foreach (var kv in sortedDictionary)
            {
                writer.WriteLine($"{kv.Key},{kv.Value}");
            }
        }
    }
}

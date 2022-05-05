using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;

namespace CodeToData.Domain;

public static class Utilities
{
    public static async Task SaveCsvAsync<T>(string path, IEnumerable<T> items)
    {
        await using var writer = new StreamWriter(path);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(items);
    }

    public static async Task<IEnumerable<T>> ReadCsvAsync<T>(string path)
    {
        return await Task.Run(() =>
        {
            using var writer = new StreamReader(path);
            using var csv = new CsvReader(writer, CultureInfo.InvariantCulture);
            var list = csv.GetRecords<T>().ToList();
            return list;
        });
    }
}
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
        await using StreamWriter writer = new(path);
        await using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(items);
    }

    public static async Task<IEnumerable<T>> ReadCsvAsync<T>(string path)
    {
        return await Task.Run(() =>
        {
            using StreamReader writer = new(path);
            using CsvReader csv = new(writer, CultureInfo.InvariantCulture);
            List<T> list = csv.GetRecords<T>().ToList();
            return list;
        });
    }
}
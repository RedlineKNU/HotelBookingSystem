using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class JsonFileHandler
{
    public static List<Hotel> LoadHotels(string filePath)
    {
        var jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Hotel>>(jsonString);
    }

    public static void SaveHotels(string filePath, List<Hotel> hotels)
    {
        var jsonString = JsonSerializer.Serialize(hotels);
        File.WriteAllText(filePath, jsonString);
    }
}

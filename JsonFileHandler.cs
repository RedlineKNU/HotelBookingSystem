using System.Text.Json;

public class JsonFileHandler
{
    public static List<Hotel> LoadHotels(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return new List<Hotel>(); // Якщо файл не існує, повертаємо порожній список
            }

            string jsonContent = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Hotel>>(jsonContent) ?? new List<Hotel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка під час читання файлу: {ex.Message}");
            return new List<Hotel>();
        }
    }

    public static void SaveHotels(string filePath, List<Hotel> hotels)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(hotels, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка під час збереження файлу: {ex.Message}");
        }
    }
}

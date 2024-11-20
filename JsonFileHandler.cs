using System.Text.Json;

public class JsonFileHandler
{
    public static List<Hotel> LoadHotels(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return new List<Hotel>(); // Якщо файл не існує, повертаємо порожній список.
            }

            string jsonContent = File.ReadAllText(filePath); // Читаємо вміст файлу.
            return JsonSerializer.Deserialize<List<Hotel>>(jsonContent) ?? new List<Hotel>(); // Десеріалізуємо JSON у список готелів.
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

    public static List<Hotel> DeserializeHotels(string jsonContent)
    {
        try
        {
            return JsonSerializer.Deserialize<List<Hotel>>(jsonContent); 
        }
        catch
        {
            return null; 
        }
    }


    public static bool IsValidHotelData(List<Hotel> hotels)
    {
        return hotels != null && hotels.All(h => h.Id > 0 && !string.IsNullOrWhiteSpace(h.Name)); 
    }

    public static void CopyFile(string sourceFilePath, string destinationFilePath)
    {
        try
        {
            File.Copy(sourceFilePath, destinationFilePath, true); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка під час копіювання файлу: {ex.Message}");
        }
    }
}

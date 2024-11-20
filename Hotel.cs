using System.Text.Json.Serialization;

public class Hotel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("availableRooms")]
    public int AvailableRooms { get; set; }

    [JsonPropertyName("pricePerNight")]
    public decimal PricePerNight { get; set; }

}



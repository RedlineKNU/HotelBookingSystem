public class HotelList
{
    private List<Hotel> hotels;

    public HotelList()
    {
        hotels = new List<Hotel>();
    }

    public HotelList(List<Hotel> hotels)
    {
        this.hotels = hotels;
    }

    public void AddHotel(Hotel hotel)
    {
        hotels.Add(hotel);
    }

    public void EditHotel(int id, Hotel updatedHotel)
    {
        var hotel = hotels.FirstOrDefault(h => h.Id == id);
        if (hotel != null)
        {
            hotel.Name = updatedHotel.Name;
            hotel.Location = updatedHotel.Location;
            hotel.AvailableRooms = updatedHotel.AvailableRooms;
            hotel.PricePerNight = updatedHotel.PricePerNight;
        }
    }

    public void RemoveHotel(int id)
    {
        var hotel = hotels.FirstOrDefault(h => h.Id == id);
        if (hotel != null)
        {
            hotels.Remove(hotel);
        }
    }

    public List<Hotel> GetAllHotels()
    {
        return hotels;
    }

    public List<Hotel> SearchByLocation(string location)
    {
        return hotels.Where(h => h.Location.Contains(location)).ToList();
    }

    public List<Hotel> SearchByPriceRange(decimal minPrice, decimal maxPrice)
    {
        return hotels.Where(h => h.PricePerNight >= minPrice && h.PricePerNight <= maxPrice).ToList();
    }

    public List<Hotel> SearchByAvailability(int minRooms)
    {
        return hotels.Where(h => h.AvailableRooms >= minRooms).ToList();
    }
}

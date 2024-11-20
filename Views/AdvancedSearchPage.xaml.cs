using System.Collections.ObjectModel;

namespace HotelBookingSystem.Views
{
    public partial class AdvancedSearchPage : ContentPage
    {
        private HotelList hotelList;

        public AdvancedSearchPage(HotelList hotelList)
        {
            InitializeComponent(); 
            this.hotelList = hotelList; 
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            // Отримуємо введені значення з текстових полів.
            var location = LocationEntry.Text?.Trim();
            var minPriceStr = MinPriceEntry.Text?.Trim();
            var maxPriceStr = MaxPriceEntry.Text?.Trim();
            var minRoomsStr = MinRoomsEntry.Text?.Trim();

            // Перевіряємо введені значення.
            if (!ValidateInputs(minPriceStr, maxPriceStr, minRoomsStr, out decimal? minPrice, out decimal? maxPrice, out int? minRooms))
            {
                await DisplayAlert("Помилка", "Введіть правильні значення параметрів.", "OK"); 
                return;
            }

            // Отримуємо всі готелі та застосовуємо фільтри пошуку.
            var searchResults = hotelList.GetAllHotels().AsEnumerable();

            // Фільтруємо готелі за місцезнаходженням.
            if (!string.IsNullOrEmpty(location))
            {
                searchResults = searchResults.Where(h => h.Location.Contains(location, StringComparison.OrdinalIgnoreCase));
            }
            // Фільтруємо готелі за мінімальною ціною.
            if (minPrice.HasValue)
            {
                searchResults = searchResults.Where(h => h.PricePerNight >= minPrice.Value);
            }
            // Фільтруємо готелі за максимальною ціною.
            if (maxPrice.HasValue)
            {
                searchResults = searchResults.Where(h => h.PricePerNight <= maxPrice.Value);
            }
            // Фільтруємо готелі за мінімальною кількістю кімнат.
            if (minRooms.HasValue)
            {
                searchResults = searchResults.Where(h => h.AvailableRooms >= minRooms.Value);
            }

            // Якщо знайдено результати пошуку, відображаємо їх на новій сторінці.
            if (searchResults.Any())
            {
                await Navigation.PushAsync(new SearchHotelsPage(new ObservableCollection<Hotel>(searchResults.ToList())));
            }
            else
            {
                await DisplayAlert("Результати пошуку", "Нічого не знайдено за заданими критеріями.", "OK");
            }
        }

        private bool ValidateInputs(string minPriceStr, string maxPriceStr, string minRoomsStr, out decimal? minPrice, out decimal? maxPrice, out int? minRooms)
        {
            minPrice = null;
            maxPrice = null;
            minRooms = null;

            decimal minPriceValue = 0;
            decimal maxPriceValue = 0;
            int minRoomsValue = 0;

            bool isValid = true;

            // Перевіряємо мінімальну ціну.
            if (!string.IsNullOrEmpty(minPriceStr))
            {
                isValid &= decimal.TryParse(minPriceStr, out minPriceValue);
            }
            // Перевіряємо максимальну ціну.
            if (!string.IsNullOrEmpty(maxPriceStr))
            {
                isValid &= decimal.TryParse(maxPriceStr, out maxPriceValue);
            }
            // Перевіряємо мінімальну кількість кімнат.
            if (!string.IsNullOrEmpty(minRoomsStr))
            {
                isValid &= int.TryParse(minRoomsStr, out minRoomsValue);
            }

            if (!isValid)
            {
                return false;
            }

            // Встановлюємо валідовані значення.
            minPrice = minPriceValue != 0 ? (decimal?)minPriceValue : null;
            maxPrice = maxPriceValue != 0 ? (decimal?)maxPriceValue : null;
            minRooms = minRoomsValue != 0 ? (int?)minRoomsValue : null;

            return true; 
        }
    }
}

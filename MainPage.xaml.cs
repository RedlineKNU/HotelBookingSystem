using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;

namespace HotelBookingSystem
{
    public partial class MainPage : ContentPage
    {
        private HotelList hotelList;
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "hotels.json"); // шлях до JSON файлу

        public MainPage()
        {
            InitializeComponent();
            hotelList = new HotelList();

            // Завантаження списку готелів з файлу при старті програми
            if (File.Exists(filePath))
            {
                var hotels = JsonFileHandler.LoadHotels(filePath);
                hotelList = new HotelList(hotels);
            }

            // Реєстрація події закриття програми
            Application.Current.ModalPopped += OnAppClose;
        }

        private async void OnViewHotelsClicked(object sender, EventArgs e)
        {
            var hotels = hotelList.GetAllHotels();
            string displayHotels = "Список готелів:\n";

            foreach (var hotel in hotels)
            {
                displayHotels += $"{hotel.Name} в {hotel.Location}, Кімнат: {hotel.AvailableRooms}, Ціна за ніч: {hotel.PricePerNight} USD\n";
            }

            await DisplayAlert("Готелі", displayHotels, "OK");
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            string aboutInfo = "ПІБ: Ваше ім'я\nКурс: Ваш курс\nГрупа: Ваша група\nРік: 2024\nКороткий опис: Це програма для бронювання готелів.";
            await DisplayAlert("Про програму", aboutInfo, "OK");
        }

        // Метод для додавання готелю
        private async void OnAddHotelClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Додати Готель", "Введіть назву готелю:");
            string location = await DisplayPromptAsync("Додати Готель", "Введіть місцезнаходження готелю:");
            string availableRoomsStr = await DisplayPromptAsync("Додати Готель", "Введіть кількість доступних кімнат:");
            int availableRooms = int.Parse(availableRoomsStr);
            string pricePerNightStr = await DisplayPromptAsync("Додати Готель", "Введіть ціну за ніч:");
            decimal pricePerNight = decimal.Parse(pricePerNightStr);

            var newHotel = new Hotel
            {
                Id = hotelList.GetAllHotels().Count + 1, // генерація нового ID
                Name = name,
                Location = location,
                AvailableRooms = availableRooms,
                PricePerNight = pricePerNight
            };

            hotelList.AddHotel(newHotel);
            JsonFileHandler.SaveHotels(filePath, hotelList.GetAllHotels());

            await DisplayAlert("Успіх", "Готель додано", "OK");
        }

        // Метод для редагування готелю
        private async void OnEditHotelClicked(object sender, EventArgs e)
        {
            var hotels = hotelList.GetAllHotels();

            if (hotels.Count > 0)
            {
                string idStr = await DisplayPromptAsync("Редагувати Готель", "Введіть ID готелю:");
                int id = int.Parse(idStr);
                var hotel = hotels.FirstOrDefault(h => h.Id == id);

                if (hotel != null)
                {
                    string name = await DisplayPromptAsync("Редагувати Готель", "Введіть нову назву готелю:", initialValue: hotel.Name);
                    string location = await DisplayPromptAsync("Редагувати Готель", "Введіть нове місцезнаходження готелю:", initialValue: hotel.Location);
                    string availableRoomsStr = await DisplayPromptAsync("Редагувати Готель", "Введіть нову кількість доступних кімнат:", initialValue: hotel.AvailableRooms.ToString());
                    int availableRooms = int.Parse(availableRoomsStr);
                    string pricePerNightStr = await DisplayPromptAsync("Редагувати Готель", "Введіть нову ціну за ніч:", initialValue: hotel.PricePerNight.ToString());
                    decimal pricePerNight = decimal.Parse(pricePerNightStr);

                    hotel.Name = name;
                    hotel.Location = location;
                    hotel.AvailableRooms = availableRooms;
                    hotel.PricePerNight = pricePerNight;

                    hotelList.EditHotel(hotel.Id, hotel);
                    JsonFileHandler.SaveHotels(filePath, hotels);

                    await DisplayAlert("Успіх", "Готель оновлено", "OK");
                }
                else
                {
                    await DisplayAlert("Помилка", "Готель з вказаним ID не знайдено", "OK");
                }
            }
            else
            {
                await DisplayAlert("Помилка", "Список готелів порожній", "OK");
            }
        }

        // Метод для пошуку готелів за місцем розташування
        private async void OnSearchByLocationClicked(object sender, EventArgs e)
        {
            string location = await DisplayPromptAsync("Пошук за місцем розташування", "Введіть місцезнаходження:");
            var searchResults = hotelList.SearchByLocation(location);
            string displayResults = $"Готелі в {location}:\n";

            foreach (var hotel in searchResults)
            {
                displayResults += $"{hotel.Name}, Кімнат: {hotel.AvailableRooms}, Ціна за ніч: {hotel.PricePerNight} USD\n";
            }

            await DisplayAlert("Пошук готелів за місцем розташування", displayResults, "OK");
        }

        // Метод для пошуку готелів за діапазоном цін
        private async void OnSearchByPriceRangeClicked(object sender, EventArgs e)
        {
            string minPriceStr = await DisplayPromptAsync("Пошук за діапазоном цін", "Введіть мінімальну ціну за ніч:");
            decimal minPrice = decimal.Parse(minPriceStr);
            string maxPriceStr = await DisplayPromptAsync("Пошук за діапазоном цін", "Введіть максимальну ціну за ніч:");
            decimal maxPrice = decimal.Parse(maxPriceStr);
            var searchResults = hotelList.SearchByPriceRange(minPrice, maxPrice);
            string displayResults = $"Готелі з ціною від {minPrice} до {maxPrice} USD за ніч:\n";

            foreach (var hotel in searchResults)
            {
                displayResults += $"{hotel.Name} в {hotel.Location}, Кімнат: {hotel.AvailableRooms}, Ціна за ніч: {hotel.PricePerNight} USD\n";
            }

            await DisplayAlert("Пошук готелів за діапазоном цін", displayResults, "OK");
        }

        // Метод для пошуку готелів за кількістю доступних кімнат
        private async void OnSearchByAvailabilityClicked(object sender, EventArgs e)
        {
            string minRoomsStr = await DisplayPromptAsync("Пошук за кількістю кімнат", "Введіть мінімальну кількість доступних кімнат:");
            int minRooms = int.Parse(minRoomsStr);
            var searchResults = hotelList.SearchByAvailability(minRooms);
            string displayResults = $"Готелі з щонайменше {minRooms} доступними кімнатами:\n";

            foreach (var hotel in searchResults)
            {
                displayResults += $"{hotel.Name} в {hotel.Location}, Кімнат: {hotel.AvailableRooms}, Ціна за ніч: {hotel.PricePerNight} USD\n";
            }

            await DisplayAlert("Пошук готелів за кількістю доступних кімнат", displayResults, "OK");
        }

        // Метод для видалення готелю
        private async void OnRemoveHotelClicked(object sender, EventArgs e)
        {
            string idStr = await DisplayPromptAsync("Видалити Готель", "Введіть ID готелю для видалення:");
            int id = int.Parse(idStr);
            var hotel = hotelList.GetAllHotels().FirstOrDefault(h => h.Id == id);

            if (hotel != null)
            {
                hotelList.RemoveHotel(hotel.Id);
                JsonFileHandler.SaveHotels(filePath, hotelList.GetAllHotels());

                await DisplayAlert("Успіх", "Готель видалено", "OK");
            }
            else
            {
                await DisplayAlert("Помилка", "Готель з вказаним ID не знайдено", "OK");
            }
        }

        // Метод для збереження даних при закритті програми
        private void OnAppClose(object sender, ModalPoppedEventArgs e)
        {
            JsonFileHandler.SaveHotels(filePath, hotelList.GetAllHotels());
        }
    }
}

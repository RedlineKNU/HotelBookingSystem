using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HotelBookingSystem
{
    public partial class MainPage : ContentPage
    {
        private HotelList hotelList;
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "hotels.json");

        // Властивість для прив'язки до CollectionView
        public List<Hotel> Hotels { get; set; }

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

            // Ініціалізація списку для прив'язки
            Hotels = hotelList.GetAllHotels();
            BindingContext = this;

        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            string aboutInfo = "ПІБ: Ваше ім'я\nКурс: Ваш курс\nГрупа: Ваша група\nРік: 2024\nКороткий опис: Це програма для бронювання готелів.";
            await DisplayAlert("Про програму", aboutInfo, "OK");
        }

        private async void OnAddHotelClicked(object sender, EventArgs e)
        {
            try
            {
                string name = await DisplayPromptAsync("Додати готель", "Назва:");
                if (string.IsNullOrWhiteSpace(name))
                {
                    await DisplayAlert("Помилка", "Назва готелю не може бути порожньою.", "OK");
                    return;
                }

                string location = await DisplayPromptAsync("Додати готель", "Місцезнаходження:");
                if (string.IsNullOrWhiteSpace(location))
                {
                    await DisplayAlert("Помилка", "Місцезнаходження готелю не може бути порожнім.", "OK");
                    return;
                }

                string availableRoomsStr = await DisplayPromptAsync("Додати готель", "Доступні кімнати:");
                if (!int.TryParse(availableRoomsStr, out int availableRooms) || availableRooms < 0)
                {
                    await DisplayAlert("Помилка", "Кількість доступних кімнат має бути числом більше або рівним 0.", "OK");
                    return;
                }

                string pricePerNightStr = await DisplayPromptAsync("Додати готель", "Ціна за ніч:");
                if (!decimal.TryParse(pricePerNightStr, out decimal pricePerNight) || pricePerNight <= 0)
                {
                    await DisplayAlert("Помилка", "Ціна за ніч має бути додатним числом.", "OK");
                    return;
                }

                var newHotel = new Hotel
                {
                    Id = Hotels.Count + 1,
                    Name = name,
                    Location = location,
                    AvailableRooms = availableRooms,
                    PricePerNight = pricePerNight
                };

                hotelList.AddHotel(newHotel);
                SaveHotelsToFile();

                await DisplayAlert("Успіх", $"Готель \"{newHotel.Name}\" додано успішно.", "OK");
                RefreshHotels();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Не вдалося додати готель: {ex.Message}", "OK");
            }
        }

        private async void OnEditHotelClicked(object sender, EventArgs e)
        {
            try
            {
                string idStr = await DisplayPromptAsync("Редагувати Готель", "Введіть ID готелю:");
                if (!int.TryParse(idStr, out int id))
                {
                    await DisplayAlert("Помилка", "ID має бути числовим значенням.", "OK");
                    return;
                }

                var hotel = Hotels.FirstOrDefault(h => h.Id == id);
                if (hotel == null)
                {
                    await DisplayAlert("Помилка", "Готель з таким ID не знайдено.", "OK");
                    return;
                }

                string name = await DisplayPromptAsync("Редагувати Готель", "Введіть нову назву готелю:", initialValue: hotel.Name);
                string location = await DisplayPromptAsync("Редагувати Готель", "Введіть нове місцезнаходження готелю:", initialValue: hotel.Location);
                string availableRoomsStr = await DisplayPromptAsync("Редагувати Готель", "Введіть нову кількість доступних кімнат:", initialValue: hotel.AvailableRooms.ToString());
                if (!int.TryParse(availableRoomsStr, out int availableRooms) || availableRooms < 0)
                {
                    await DisplayAlert("Помилка", "Кількість доступних кімнат має бути числом більше або рівним 0.", "OK");
                    return;
                }

                string pricePerNightStr = await DisplayPromptAsync("Редагувати Готель", "Введіть нову ціну за ніч:", initialValue: hotel.PricePerNight.ToString());
                if (!decimal.TryParse(pricePerNightStr, out decimal pricePerNight) || pricePerNight <= 0)
                {
                    await DisplayAlert("Помилка", "Ціна за ніч має бути додатним числом.", "OK");
                    return;
                }

                hotel.Name = name;
                hotel.Location = location;
                hotel.AvailableRooms = availableRooms;
                hotel.PricePerNight = pricePerNight;

                hotelList.EditHotel(hotel.Id, hotel);
                SaveHotelsToFile();

                await DisplayAlert("Успіх", "Готель оновлено.", "OK");
                RefreshHotels();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Не вдалося редагувати готель: {ex.Message}", "OK");
            }
        }

        private void SaveHotelsToFile()
        {
            JsonFileHandler.SaveHotels(filePath, hotelList.GetAllHotels());
        }
        private async void OnSearchByLocationClicked(object sender, EventArgs e)
        {
            try
            {
                // Запит місцезнаходження
                string location = await DisplayPromptAsync("Пошук за місцезнаходженням", "Введіть місцезнаходження:");
                if (string.IsNullOrWhiteSpace(location)) return; // Якщо натиснуто "Відміна" або залишено пусте поле

                // Пошук готелів за місцезнаходженням
                var searchResults = hotelList.SearchByLocation(location);

                if (searchResults.Any())
                {
                    string resultBuilder = "Знайдені готелі:\n";
                    foreach (var hotel in searchResults)
                    {
                        resultBuilder += $"- {hotel.Name}, {hotel.Location}: {hotel.AvailableRooms} кімнат, {hotel.PricePerNight} USD/ніч\n";
                    }

                    await DisplayAlert("Результати пошуку", resultBuilder, "OK");
                }
                else
                {
                    await DisplayAlert("Результати пошуку", "Готелів за вказаним місцезнаходженням не знайдено.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Обробка будь-яких непередбачених помилок
                await DisplayAlert("Помилка", $"Виникла помилка під час пошуку: {ex.Message}", "OK");
            }
        }


        private async void OnSearchByPriceRangeClicked(object sender, EventArgs e)
        {
            try
            {
                string minPriceStr = await DisplayPromptAsync("Пошук за діапазоном цін", "Введіть мінімальну ціну за ніч:");
                if (string.IsNullOrWhiteSpace(minPriceStr)) return; // Якщо натиснуто "Відміна"

                if (!decimal.TryParse(minPriceStr, out decimal minPrice) || minPrice < 0)
                {
                    await DisplayAlert("Помилка", "Мінімальна ціна має бути додатним числовим значенням.", "OK");
                    return;
                }

                string maxPriceStr = await DisplayPromptAsync("Пошук за діапазоном цін", "Введіть максимальну ціну за ніч:");
                if (string.IsNullOrWhiteSpace(maxPriceStr)) return; // Якщо натиснуто "Відміна"

                if (!decimal.TryParse(maxPriceStr, out decimal maxPrice) || maxPrice < minPrice)
                {
                    await DisplayAlert("Помилка", "Максимальна ціна має бути числом більше або рівним мінімальній ціні.", "OK");
                    return;
                }

                var searchResults = hotelList.SearchByPriceRange(minPrice, maxPrice);

                if (searchResults.Any())
                {
                    string displayResults = $"Готелі з ціною від {minPrice} до {maxPrice} USD за ніч:\n";
                    foreach (var hotel in searchResults)
                    {
                        displayResults += $"{hotel.Name} в {hotel.Location}, Кімнат: {hotel.AvailableRooms}, Ціна за ніч: {hotel.PricePerNight} USD\n";
                    }
                    await DisplayAlert("Результати пошуку", displayResults, "OK");
                }
                else
                {
                    await DisplayAlert("Результати пошуку", "Готелів за вказаним діапазоном цін не знайдено.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
            }
        }

        private async void OnSearchByAvailabilityClicked(object sender, EventArgs e)
        {
            try
            {
                string minRoomsStr = await DisplayPromptAsync("Пошук за кількістю кімнат", "Введіть мінімальну кількість доступних кімнат:");
                if (string.IsNullOrWhiteSpace(minRoomsStr)) return; // Якщо натиснуто "Відміна"

                if (!int.TryParse(minRoomsStr, out int minRooms) || minRooms < 0)
                {
                    await DisplayAlert("Помилка", "Кількість кімнат має бути додатним числовим значенням.", "OK");
                    return;
                }

                var searchResults = hotelList.SearchByAvailability(minRooms);

                if (searchResults.Any())
                {
                    string displayResults = $"Готелі з щонайменше {minRooms} доступними кімнатами:\n";
                    foreach (var hotel in searchResults)
                    {
                        displayResults += $"{hotel.Name} в {hotel.Location}, Кімнат: {hotel.AvailableRooms}, Ціна за ніч: {hotel.PricePerNight} USD\n";
                    }
                    await DisplayAlert("Результати пошуку", displayResults, "OK");
                }
                else
                {
                    await DisplayAlert("Результати пошуку", "Готелів із вказаною кількістю кімнат не знайдено.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
            }
        }

        private async void OnRemoveHotelClicked(object sender, EventArgs e)
        {
            try
            {
                string idStr = await DisplayPromptAsync("Видалити Готель", "Введіть ID готелю для видалення:");
                if (string.IsNullOrWhiteSpace(idStr)) return; // Якщо натиснуто "Відміна"

                if (!int.TryParse(idStr, out int id) || id <= 0)
                {
                    await DisplayAlert("Помилка", "ID має бути додатним числовим значенням.", "OK");
                    return;
                }

                var hotel = Hotels.FirstOrDefault(h => h.Id == id);

                if (hotel != null)
                {
                    hotelList.RemoveHotel(hotel.Id);
                    JsonFileHandler.SaveHotels(filePath, hotelList.GetAllHotels());

                    await DisplayAlert("Успіх", $"Готель \"{hotel.Name}\" у місті \"{hotel.Location}\" видалено успішно.", "OK");
                    RefreshHotels();
                }
                else
                {
                    await DisplayAlert("Помилка", $"Готель з ID {id} не знайдено.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
            }
        }



        private void OnAppClose(object sender, ModalPoppedEventArgs e)
        {
            JsonFileHandler.SaveHotels(filePath, hotelList.GetAllHotels());
        }
        private void RefreshHotels()
        {
            Hotels = hotelList.GetAllHotels();
            BindingContext = null;
            BindingContext = this;
        }
    }
}



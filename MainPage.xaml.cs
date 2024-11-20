
using System.Collections.ObjectModel;

namespace HotelBookingSystem
{
    public partial class MainPage : ContentPage
    {
        private HotelList hotelList;
        private readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "hotels.json");

        // Властивість для прив'язки до CollectionView
        public List<Hotel> Hotels { get; set; }

        public MainPage()
        {
            InitializeComponent();
            LoadHotels();

            BindingContext = this;
        }

        private void LoadHotels()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var hotels = JsonFileHandler.LoadHotels(filePath);
                    hotelList = new HotelList(hotels);
                }
                catch (Exception ex)
                {
                    DisplayAlert("Помилка", $"Не вдалося завантажити дані: {ex.Message}", "OK");
                    hotelList = new HotelList();
                }
            }
            else
            {
                hotelList = new HotelList();
            }

            Hotels = hotelList.GetAllHotels();
        }

        private void SaveHotelsToFile()
        {
            try
            {
                JsonFileHandler.SaveHotels(filePath, hotelList.GetAllHotels());
            }
            catch (Exception ex)
            {
                DisplayAlert("Помилка", $"Не вдалося зберегти дані: {ex.Message}", "OK");
            }
        }

        private async void OnChooseFileClicked(object sender, EventArgs e)
        {
            try
            {
                var jsonFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.json" } },
                    { DevicePlatform.Android, new[] { "application/json" } },
                    { DevicePlatform.WinUI, new[] { ".json" } },
                    { DevicePlatform.macOS, new[] { "json" } }
                });

                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Виберіть JSON файл",
                    FileTypes = jsonFileType // Обмеження вибору лише файлами JSON
                });

                if (result != null)
                {
                    // Читання вмісту вибраного файлу
                    string fileContent = await File.ReadAllTextAsync(result.FullPath);

                    // Десеріалізація JSON в об'єкти
                    var hotels = JsonFileHandler.DeserializeHotels(fileContent);

                    if (hotels != null)
                    {
                        hotelList = new HotelList(hotels);
                        RefreshHotels(); // Оновлення списку
                        await DisplayAlert("Успіх", "Дані успішно завантажені!", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Помилка", "Файл JSON містить некоректні дані.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Не вдалося завантажити файл: {ex.Message}", "OK");
            }
        }


        private void RefreshHotels()
        {
            Hotels = hotelList.GetAllHotels();
            BindingContext = null;
            BindingContext = this;
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage());
        }

        private async void OnSearchByLocationClicked(object sender, EventArgs e)
        {
            try
            {
                string location = await DisplayPromptAsync("Пошук за місцезнаходженням", "Введіть місцезнаходження:");
                if (string.IsNullOrWhiteSpace(location)) return;

                var searchResults = hotelList.SearchByLocation(location);

                if (searchResults.Any())
                {
                    // Перехід на сторінку з результатами
                    await Navigation.PushAsync(new SearchHotelsPage(new ObservableCollection<Hotel>(searchResults)));
                }
                else
                {
                    await DisplayAlert("Результати пошуку", "Готелів за вказаним місцезнаходженням не знайдено.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
            }
        }

        private async void OnAddHotelClicked(object sender, EventArgs e)
        {
            var newHotel = new Hotel
            {
                Id = Hotels.Count + 1
            };

            await Navigation.PushAsync(new AddEditHotelPage(newHotel, "Додати готель", hotel =>
            {
                hotelList.AddHotel(hotel);
                SaveHotelsToFile();
                RefreshHotels();
            }));
        }

        private async void OnEditHotelClicked(object sender, EventArgs e)
        {
            try
            {
                string idStr = await DisplayPromptAsync("Редагувати готель", "Введіть ID готелю:");
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

                await Navigation.PushAsync(new AddEditHotelPage(hotel, "Редагувати готель", updatedHotel =>
                {
                    hotelList.EditHotel(updatedHotel.Id, updatedHotel);
                    SaveHotelsToFile();
                    RefreshHotels();
                }));
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
                string idStr = await DisplayPromptAsync("Видалити готель", "Введіть ID готелю для видалення:");
                if (!int.TryParse(idStr, out int id) || id <= 0)
                {
                    await DisplayAlert("Помилка", "ID має бути додатним числовим значенням.", "OK");
                    return;
                }

                var hotel = Hotels.FirstOrDefault(h => h.Id == id);
                if (hotel != null)
                {
                    bool confirm = await DisplayAlert("Підтвердження", $"Ви дійсно хочете видалити готель \"{hotel.Name}\"?", "Так", "Ні");
                    if (!confirm) return;

                    hotelList.RemoveHotel(hotel.Id);
                    SaveHotelsToFile();
                    await DisplayAlert("Успіх", $"Готель \"{hotel.Name}\" видалено успішно.", "OK");
                    RefreshHotels();
                }
                else
                {
                    await DisplayAlert("Помилка", "Готель з таким ID не знайдено.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
            }
        }
        private async void OnSearchByPriceRangeClicked(object sender, EventArgs e)
        {
            try
            {
                string minPriceStr = await DisplayPromptAsync("Пошук за ціною", "Введіть мінімальну ціну:");
                if (!decimal.TryParse(minPriceStr, out decimal minPrice) || minPrice < 0)
                {
                    await DisplayAlert("Помилка", "Мінімальна ціна має бути додатним числом.", "OK");
                    return;
                }

                string maxPriceStr = await DisplayPromptAsync("Пошук за ціною", "Введіть максимальну ціну:");
                if (!decimal.TryParse(maxPriceStr, out decimal maxPrice) || maxPrice < minPrice)
                {
                    await DisplayAlert("Помилка", "Максимальна ціна має бути числом більше мінімальної.", "OK");
                    return;
                }

                var searchResults = hotelList.SearchByPriceRange(minPrice, maxPrice);

                if (searchResults.Any())
                {
                    // Перехід на сторінку SearchHotelsPage з результатами пошуку за ціною
                    await Navigation.PushAsync(new SearchHotelsPage(new ObservableCollection<Hotel>(searchResults)));
                }
                else
                {
                    await DisplayAlert("Результати пошуку", "Готелів у вказаному діапазоні цін не знайдено.", "OK");
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
                string roomsStr = await DisplayPromptAsync("Пошук за кімнатами", "Введіть мінімальну кількість кімнат:");
                if (!int.TryParse(roomsStr, out int minRooms) || minRooms < 0)
                {
                    await DisplayAlert("Помилка", "Кількість кімнат має бути додатним числом.", "OK");
                    return;
                }

                var searchResults = hotelList.SearchByAvailability(minRooms);

                if (searchResults.Any())
                {
                    // Перехід на сторінку SearchHotelsPage з результатами пошуку за кількістю кімнат
                    await Navigation.PushAsync(new SearchHotelsPage(new ObservableCollection<Hotel>(searchResults)));
                }
                else
                {
                    await DisplayAlert("Результати пошуку", "Готелів із такою кількістю кімнат не знайдено.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
            }
        }
    }
}

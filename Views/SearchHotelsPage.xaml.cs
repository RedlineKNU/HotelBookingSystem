using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HotelBookingSystem
{
    public partial class SearchHotelsPage : ContentPage
    {
        public ObservableCollection<Hotel> Hotels { get; set; }
        public ObservableCollection<Hotel> FilteredHotels { get; set; }

        public string SearchQuery { get; set; }
        public ICommand SearchCommand { get; }

        public SearchHotelsPage(ObservableCollection<Hotel> hotels)
        {
            InitializeComponent();

            // Ініціалізація колекцій
            Hotels = hotels;
            FilteredHotels = new ObservableCollection<Hotel>(Hotels);

            // Ініціалізація команди пошуку
            SearchCommand = new Command(ExecuteSearch);

            // Встановлення прив'язки даних
            BindingContext = this;
        }

        // Метод для виконання пошуку
        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                // Якщо запит порожній, показуємо всі готелі
                FilteredHotels.Clear();
                foreach (var hotel in Hotels)
                    FilteredHotels.Add(hotel);
            }
            else
            {
                var query = SearchQuery.ToLower();
                var results = Hotels.Where(h =>
                    h.Name.ToLower().Contains(query) ||
                    h.Location.ToLower().Contains(query)
                ).ToList();

                // Очищаємо поточний список і додаємо нові результати
                FilteredHotels.Clear();
                foreach (var hotel in results)
                    FilteredHotels.Add(hotel);
            }
        }
    }
}

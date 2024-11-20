using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HotelBookingSystem
{
    public partial class SearchHotelsPage : ContentPage
    {
        public ObservableCollection<Hotel> Hotels { get; set; }

        public ObservableCollection<Hotel> FilteredHotels { get; set; }

        // Властивість SearchQuery для зберігання тексту пошукового запиту.
        public string SearchQuery { get; set; }

        // Команда для виконання пошуку.
        public ICommand SearchCommand { get; }

        public SearchHotelsPage(ObservableCollection<Hotel> hotels)
        {
            InitializeComponent(); 

            Hotels = hotels; 
            FilteredHotels = new ObservableCollection<Hotel>(Hotels); 

            SearchCommand = new Command(ExecuteSearch); 

            BindingContext = this; // Встановлюємо контекст прив'язки для інтерфейсу.
        }

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
                var query = SearchQuery.ToLower(); // Приводимо пошуковий запит до нижнього регістру.
                var results = Hotels.Where(h =>
                    h.Name.ToLower().Contains(query) || // Пошук по назві готелю.
                    h.Location.ToLower().Contains(query) // Пошук по місцезнаходженню готелю.
                ).ToList();

                // Очищаємо поточний список і додаємо нові результати
                FilteredHotels.Clear(); 
                foreach (var hotel in results)
                    FilteredHotels.Add(hotel); 
            }
        }
    }
}

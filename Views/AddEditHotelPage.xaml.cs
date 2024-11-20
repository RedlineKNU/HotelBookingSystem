using Microsoft.Maui.Controls;
using System;
using System.Windows.Input;

namespace HotelBookingSystem
{
    public partial class AddEditHotelPage : ContentPage
    {
        public Hotel Hotel { get; set; }
        public string PageTitle { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private Action<Hotel> _onSaveCallback;

        public AddEditHotelPage(Hotel hotel, string title, Action<Hotel> onSaveCallback)
        {
            InitializeComponent();
            Hotel = hotel;
            PageTitle = title;
            _onSaveCallback = onSaveCallback;

            SaveCommand = new Command(Save);
            CancelCommand = new Command(Cancel);

            BindingContext = this;
        }

        private async void Save()
        {
            if (string.IsNullOrWhiteSpace(Hotel.Name) || string.IsNullOrWhiteSpace(Hotel.Location) ||
                Hotel.AvailableRooms <= 0 || Hotel.PricePerNight <= 0)
            {
                await DisplayAlert("Помилка", "Перевірте всі поля. Вони мають бути заповнені правильно.", "OK");
                return;
            }

            _onSaveCallback?.Invoke(Hotel);
            await Navigation.PopAsync();
        }

        private async void Cancel()
        {
            await Navigation.PopAsync();
        }
    }
}

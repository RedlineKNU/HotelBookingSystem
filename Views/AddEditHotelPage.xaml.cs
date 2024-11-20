﻿using System.Windows.Input;

namespace HotelBookingSystem
{
    public partial class AddEditHotelPage : ContentPage
    {
        public Hotel Hotel { get; set; }
        public string PageTitle { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private Action<Hotel> _onSaveCallback;

        // Конструктор AddEditHotelPage, який ініціалізує компоненти та встановлює початкові значення.
        public AddEditHotelPage(Hotel hotel, string title, Action<Hotel> onSaveCallback)
        {
            InitializeComponent(); 
            Hotel = hotel; 
            PageTitle = title;
            _onSaveCallback = onSaveCallback; 

            SaveCommand = new Command(Save); 
            CancelCommand = new Command(Cancel); 

            BindingContext = this; // Встановлюємо контекст прив'язки для інтерфейсу.
        }

        private async void Save()
        {
            var validationResult = ValidateHotelData(); // Перевіряємо дані готелю.
            if (!validationResult.IsValid)
            {
                await DisplayAlert("Помилка", validationResult.ErrorMessage, "OK"); 
                return;
            }

            _onSaveCallback?.Invoke(Hotel); // Викликаємо callback для збереження.
            await Navigation.PopAsync(); 
        }

        private ValidationResult ValidateHotelData()
        {
            if (string.IsNullOrWhiteSpace(Hotel.Name))
                return ValidationResult.Failed("Назва готелю не може бути порожньою.");

            if (string.IsNullOrWhiteSpace(Hotel.Location))
                return ValidationResult.Failed("Місцезнаходження не може бути порожнім.");

            if (Hotel.AvailableRooms <= 0)
                return ValidationResult.Failed("Кількість доступних кімнат має бути більше 0.");

            if (Hotel.PricePerNight <= 0)
                return ValidationResult.Failed("Ціна за ніч має бути більше 0.");

            return ValidationResult.Success();
        }

        private async void Cancel()
        {
            await Navigation.PopAsync(); 
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public string ErrorMessage { get; private set; }

        private ValidationResult(bool isValid, string errorMessage)
        {
            IsValid = isValid; 
            ErrorMessage = errorMessage; 
        }

        // Метод для створення успішного результату перевірки.
        public static ValidationResult Success()
        {
            return new ValidationResult(true, string.Empty);
        }

        // Метод для створення результату перевірки з помилкою.
        public static ValidationResult Failed(string errorMessage)
        {
            return new ValidationResult(false, errorMessage);
        }
    }
}

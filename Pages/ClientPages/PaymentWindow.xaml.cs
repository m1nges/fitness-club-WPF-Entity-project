using fitness_club.Model;
using fitness_club.Classes;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using fitness_club.Windows;

namespace fitness_club.Pages.ClientPages
{
    public partial class PaymentWindow : Window
    {
        public bool PaymentSuccess { get; private set; } = false;
        string paymentType;
        int paymentSum;
        decimal clientBalance = 0;


        DateTime todayUTC = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        public PaymentWindow(string paymentType, int paymentSum)
        {
            InitializeComponent();
            GetClientsBalance();
            proceedAmoutTbox.Text = 0.ToString();
            this.paymentType = paymentType;
            this.paymentSum = paymentSum;
        }

        public void GetClientsBalance()
        {
            using(var db = new AppDbContext())
            {
                clientBalance = db.Clients.Where(c => c.ClientId == AuthorizationWin.currentUser.Client.ClientId).Select(c => c.Balance).FirstOrDefault();
            }

            clientsBalanceTb.Text = $"Ваш баланс: {clientBalance}";
        }

        private string GenerateRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }

        private void GenerateRandomQrCode()
        {
            string randomData = GenerateRandomString();

            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(randomData, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new QRCode(qrCodeData))
            {
                var qrCodeBitmap = qrCode.GetGraphic(20);
                QrCodeImage.Source = ConvertToBitmapImage(qrCodeBitmap);
            }
        }

        private BitmapImage ConvertToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public static bool IsValidCardNumber(string cardNumber)
        {
            string cleanCardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());

            if (cleanCardNumber.Length < 13 || cleanCardNumber.Length > 19)
                return false;

            int sum = 0;
            bool alternate = false;

            for (int i = cleanCardNumber.Length - 1; i >= 0; i--)
            {
                int digit = cleanCardNumber[i] - '0';

                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit = (digit % 10) + 1;
                }

                sum += digit;
                alternate = !alternate;
            }

            return sum % 10 == 0;
        }

        private void GenerateQr_Click(object sender, RoutedEventArgs e)
        {
            GenerateRandomQrCode();
        }

        
        private void payBtn_Click(object sender, RoutedEventArgs e)
        {
            if (cardRb.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(cardNumberTb.Text) || string.IsNullOrWhiteSpace(cvvTb.Text) ||
                    string.IsNullOrWhiteSpace(expiryDateMonthTb.Text) || string.IsNullOrWhiteSpace(expiryDateYearTb.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (IsValidCardNumber(cardNumberTb.Text))
                    {
                        if (CheckExpirationDate())
                        {
                            if (cvvTb.Text.Length == 3 && int.TryParse(cvvTb.Text, out int cvv) && cvv >= 1 && cvv <= 999)
                            {
                                using (var db = new AppDbContext())
                                {
                                    var clientId = AuthorizationWin.currentUser.Client.ClientId;

                                    db.ClientTransactions.Add(new ClientTransaction
                                    {
                                        ClientId = clientId,
                                        OperationDescription = $"Оплата: {paymentType}",
                                        PaymentWay = "Карта",
                                        Amount = -paymentSum,
                                        TransactionType = "списание",
                                        TransactionDate = todayUTC
                                    });

                                    db.SaveChanges();
                                }

                                MessageBoxResult result = MessageBox.Show(
                                    $"Вы успешно оплатили {paymentType}!",
                                    "Успех",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                                if (result == MessageBoxResult.OK)
                                {
                                    PaymentSuccess = true;
                                    this.DialogResult = true;
                                    this.Close();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Некорректный CVV код. Пожалуйста, проверьте введенные данные.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста, проверьте номер карты!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (qrCodeRb.IsChecked == true)
            {
                using (var db = new AppDbContext())
                {
                    var clientId = AuthorizationWin.currentUser.Client.ClientId;

                    db.ClientTransactions.Add(new ClientTransaction
                    {
                        ClientId = clientId,
                        OperationDescription = $"Оплата: {paymentType}",
                        PaymentWay = "QR-код",
                        Amount = -paymentSum,
                        TransactionType = "списание",
                        TransactionDate = todayUTC
                    });

                    db.SaveChanges();
                }

                MessageBoxResult result = MessageBox.Show(
                    $"Вы успешно оплатили {paymentType}!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.OK)
                {
                    PaymentSuccess = true;
                    this.DialogResult = true;
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите способ оплаты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        public bool CheckExpirationDate()
        {
            DateTime today = DateTime.Today;
            int currentYear = today.Year;
            int currentMonth = today.Month;

            if (string.IsNullOrWhiteSpace(expiryDateMonthTb.Text) ||
                string.IsNullOrWhiteSpace(expiryDateYearTb.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля даты", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(expiryDateMonthTb.Text, out int expirationMonth) ||
                !int.TryParse(expiryDateYearTb.Text, out int expirationYearShort))
            {
                MessageBox.Show("Некорректный формат даты. Используйте только цифры", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            int expirationYear = 2000 + expirationYearShort;

            if (expirationMonth < 1 || expirationMonth > 12)
            {
                MessageBox.Show("Месяц должен быть от 01 до 12", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (expirationYearShort < 0 || expirationYearShort > 99)
            {
                MessageBox.Show("Год должен быть от 00 до 99", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (expirationYear > currentYear)
            {
                return true;
            }
            else if (expirationYear == currentYear && expirationMonth >= currentMonth)
            {
                return true;
            }

            MessageBox.Show("Срок действия карты истек", "Ошибка",
                          MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        private void payMethods_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton selectedRadio = (RadioButton)sender;

            switch (selectedRadio.Name)
            {
                case "cardRb":
                    payByBalanceSp.Visibility = Visibility.Collapsed;
                    payByQRCodeSp.Visibility = Visibility.Collapsed;
                    payByCardSp.Visibility = Visibility.Visible;
                    payBtn.Visibility = Visibility.Visible;
                    payBtn.Content = $"Оплатить {paymentSum} руб.";
                    break;
                case "qrCodeRb":
                    payByBalanceSp.Visibility = Visibility.Collapsed;
                    payByCardSp.Visibility = Visibility.Collapsed;
                    GenerateRandomQrCode();
                    payByQRCodeSp.Visibility = Visibility.Visible;
                    payBtn.Visibility = Visibility.Visible;
                    payBtn.Content = $"Я оплатил {paymentSum} руб.";
                    break;
                case "balanceRb":
                    payByBalanceSp.Visibility = Visibility.Visible;
                    payByCardSp.Visibility = Visibility.Collapsed;
                    payByQRCodeSp.Visibility = Visibility.Collapsed;
                    payBtn.Visibility = Visibility.Collapsed;
                    payByBalanceBtn.Content = $"Оплатить {paymentSum} руб.";
                    break;
            }
        }

        private void payByBalanceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(proceedAmoutTbox.Text, out int proceedAmount) || proceedAmount <= 0)
            {
                MessageBox.Show("Некорректный ввод. Введите положительное число.");
                return;
            }

            using var db = new AppDbContext();
            var client = db.Clients
                .FirstOrDefault(c => c.ClientId == AuthorizationWin.currentUser.Client.ClientId);

            if (client == null)
            {
                MessageBox.Show("Клиент не найден.");
                return;
            }

            if (proceedAmount > client.Balance)
            {
                MessageBox.Show("У вас недостаточно средств на балансе.");
                return;
            }

            if (paymentSum > proceedAmount) // Частичная оплата
            {
                paymentSum -= proceedAmount;
                client.Balance -= proceedAmount;

                db.ClientTransactions.Add(new ClientTransaction
                {
                    ClientId = client.ClientId,
                    OperationDescription = $"Частичная оплата: {paymentType}",
                    PaymentWay = "С баланса",
                    Amount = -proceedAmount,
                    TransactionType = "списание",
                    TransactionDate = todayUTC
                });

                db.SaveChanges();
                MessageBox.Show($"Списано {proceedAmount} руб. Остаток: {client.Balance} руб. " +
                                "Для оплаты оставшейся части заказа выберите другой способ оплаты!");
                payByBalanceBtn.Content = $"Оплатить {paymentSum} руб.";
            }
            else // Полная оплата
            {
                client.Balance -= paymentSum;

                db.ClientTransactions.Add(new ClientTransaction
                {
                    ClientId = client.ClientId,
                    OperationDescription = $"Полная оплата: {paymentType}",
                    PaymentWay = "С баланса",
                    Amount = -paymentSum,
                    TransactionType = "списание",
                    TransactionDate = todayUTC
                });

                db.SaveChanges();

                MessageBox.Show($"Списано {paymentSum} руб. Остаток: {client.Balance} руб.");

                MessageBoxResult result = MessageBox.Show(
                    $"Вы успешно оплатили {paymentType}! С баланса списано {paymentSum} руб.\nОстаток: {client.Balance} руб.",
                    "Успешная оплата",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                if (result == MessageBoxResult.OK)
                {
                    PaymentSuccess = true;
                    this.DialogResult = true;
                    this.Close();
                }
            }
        }



        private void fullAmountOfPrice_Click(object sender, RoutedEventArgs e)
        {
            proceedAmoutTbox.Text = paymentSum.ToString();
        }

        private void proceedAmoutTbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            payByBalanceBtn.Content = $"Оплатить {proceedAmoutTbox.Text} руб.";
        }
    }
}

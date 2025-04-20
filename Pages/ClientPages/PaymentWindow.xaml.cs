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
        DateTime today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        public PaymentWindow()
        {
            InitializeComponent();
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

        //public void SuccessfullPayment(int membershipId)
        //{
        //    MessageBoxResult result = MessageBox.Show(
        //                    "Вы успешно оплатили абонемент!",
        //                    "Успех",
        //                    MessageBoxButton.OK,
        //                    MessageBoxImage.Information);

        //    if (result == MessageBoxResult.OK)
        //    {
        //        using (var db = new AppDbContext())
        //        {
        //            var currentClientMembership = db.ClientMemberships
        //                .Where(cm => cm.ClientId == AuthorizationWin.currentUser.Client.ClientId &&
        //                                      cm.EndDate >= today)
        //                .OrderByDescending(cm => cm.EndDate)
        //                .FirstOrDefault();

        //            var membership = db.Memberships
        //                .FirstOrDefault(m => m.MembershipId == membershipId);

        //            int? membershipDuration = db.MembershipTypes.Where(mt => membership.MembershipTypeId == mt.MembershipTypeId).Select(mt => mt.DurationMonths).FirstOrDefault();
        //            if (currentClientMembership == null)
        //            {

        //                var newClientMembership = new ClientMembership
        //                {
        //                    MembershipId = membershipId,
        //                    ClientId = AuthorizationWin.currentUser.Client.ClientId,
        //                    StartDate = today,
        //                    EndDate = today.AddMonths((int)membershipDuration)
        //                };
        //                db.ClientMemberships.Add(newClientMembership);
        //                db.SaveChanges();

        //            }
        //            else
        //            {
        //                var newClientMembership = new ClientMembership
        //                {
        //                    MembershipId = membershipId,
        //                    ClientId = AuthorizationWin.currentUser.Client.ClientId,
        //                    StartDate = DateTime.SpecifyKind(currentClientMembership.EndDate.AddDays(1), DateTimeKind.Utc),
        //                    EndDate = DateTime.SpecifyKind(
        //                    currentClientMembership.EndDate.AddDays(1).AddMonths((int)membershipDuration),
        //                    DateTimeKind.Utc)
        //                };

        //                db.ClientMemberships.Add(newClientMembership);
        //                db.SaveChanges();
        //            }

        //        }
        //        this.Close();
        //    }
        //}

        private void payBtn_Click(object sender, RoutedEventArgs e)
        {
            if(cardRb.IsChecked == true)
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
                                //SuccessfullPayment();
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
            else if(qrCodeRb.IsChecked == true)
            {
                //SuccessfullPayment();
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
                    payByQRCodeSp.Visibility = Visibility.Collapsed;
                    payByCardSp.Visibility = Visibility.Visible;
                    payBtn.Visibility = Visibility.Visible;
                    payBtn.Content = $"Оплатить  руб.";
                    break;
                case "qrCodeRb":
                    payByCardSp.Visibility = Visibility.Collapsed;
                    GenerateRandomQrCode();
                    payByQRCodeSp.Visibility = Visibility.Visible;
                    payBtn.Visibility = Visibility.Visible;
                    payBtn.Content = $"Я оплатил  руб.";
                    break;
            }
        }
    }
}

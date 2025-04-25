using fitness_club.Model;
using fitness_club.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace fitness_club.Pages.ClientPages
{
    /// <summary>
    /// Логика взаимодействия для ClientTransactions.xaml
    /// </summary>
    public partial class ClientTransactionsPage : Page
    {
        public ClientTransactionsPage()
        {
            InitializeComponent();
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            try
            {
                using var db = new AppDbContext();
                int clientId = AuthorizationWin.currentUser.Client.ClientId;

                var transactions = db.ClientTransactions
                    .Where(t => t.ClientId == clientId)
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(t => new
                    {
                        t.OperationDescription,
                        t.PaymentWay,
                        t.Amount,
                        t.TransactionType,
                        t.TransactionDate
                    })
                    .ToList();

                TransactionsListView.ItemsSource = transactions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке транзакций: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

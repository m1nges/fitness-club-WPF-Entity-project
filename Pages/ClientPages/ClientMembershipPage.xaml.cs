using fitness_club.Model;
using fitness_club.Classes;
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
using System.Windows.Navigation;
using Microsoft.EntityFrameworkCore;
using System.Windows.Shapes;

namespace fitness_club.Pages.ClientPages
{
    /// <summary>
    /// Логика взаимодействия для ClientMembershipPage.xaml
    /// </summary>
    public partial class ClientMembershipPage : Page
    {
        DateTime today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        public ClientMembershipPage()
        {
            InitializeComponent();
            LoadMemberships();
            LoadMembershipTypes();
        }

        public void LoadMembershipTypes()
        {
            using(var db = new AppDbContext())
            {
                var membershipTypes = db.MembershipTypes.Where(mt=>mt.MembershipTypeName != "Пробная тренировка").OrderBy(mt=>mt.MembershipTypeId).ToList();
                membershipDurationCmb.ItemsSource = membershipTypes;
                membershipDurationCmb.DisplayMemberPath = "MembershipTypeName";
                membershipDurationCmb.SelectedValuePath = "MembershipTypeId";
                membershipDurationCmb.SelectedValue = 1;


            }
        }

        public void SuccessfullPayment(int membershipId)
        {
            MessageBoxResult result = MessageBox.Show(
                            "Вы успешно приобрели абонемент. Для оплаты перейдите на страницу оплаты!",
                            "Успех",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            if (result == MessageBoxResult.OK)
            {
                using (var db = new AppDbContext())
                {
                    var currentClientMembership = db.ClientMemberships
                        .Where(cm => cm.ClientId == AuthorizationWin.currentUser.Client.ClientId &&
                                              cm.EndDate >= today)
                        .OrderByDescending(cm => cm.EndDate)
                        .FirstOrDefault();

                    var membership = db.Memberships
                        .FirstOrDefault(m => m.MembershipId == membershipId);

                    int? membershipDuration = db.MembershipTypes.Where(mt => membership.MembershipTypeId == mt.MembershipTypeId).Select(mt => mt.DurationMonths).FirstOrDefault();
                    if (currentClientMembership == null)
                    {

                        var newClientMembership = new ClientMembership
                        {
                            MembershipId = membershipId,
                            ClientId = AuthorizationWin.currentUser.Client.ClientId,
                            StartDate = today,
                            EndDate = today.AddMonths((int)membershipDuration)
                        };
                        db.ClientMemberships.Add(newClientMembership);
                        db.SaveChanges();

                        db.MembershipPayments.Add(new MembershipPayments
                        {
                            MembershipId = membershipId,
                            PaymentDate = null,
                            Price = membership.Price,
                            ClientId = AuthorizationWin.currentUser.Client.ClientId,
                        });
                        db.SaveChanges();

                    }
                    else
                    {
                        var newClientMembership = new ClientMembership
                        {
                            MembershipId = membershipId,
                            ClientId = AuthorizationWin.currentUser.Client.ClientId,
                            StartDate = DateTime.SpecifyKind(currentClientMembership.EndDate.AddDays(1), DateTimeKind.Utc),
                            EndDate = DateTime.SpecifyKind(
                            currentClientMembership.EndDate.AddDays(1).AddMonths((int)membershipDuration),
                            DateTimeKind.Utc)
                        };
                        db.ClientMemberships.Add(newClientMembership);
                        db.SaveChanges();

                        db.MembershipPayments.Add(new MembershipPayments
                        {
                            MembershipId = membershipId,
                            PaymentDate = null,
                            Price = membership.Price,
                            ClientId = AuthorizationWin.currentUser.Client.ClientId,
                        });
                        db.SaveChanges();
                    }

                }
                LoadMemberships();
            }
        }

        public void LoadMemberships()
        {
            using (var db = new AppDbContext())
            {

                var userId = AuthorizationWin.currentUser.UserId;

                var client = db.Clients.FirstOrDefault(c => c.UserId == userId);
                if (client == null) return;

                var data = db.ClientMemberships
                    .Where(cm => cm.ClientId == client.ClientId)
                    .Include(cm => cm.Membership)
                        .ThenInclude(m => m.MembershipType)
                    .ToList()
                    .Select(cm => new
                    {
                        cm.Membership.MembershipName,
                        cm.Membership.MembershipDescription,
                        MembershipType = cm.Membership.MembershipType.MembershipTypeName,
                        cm.StartDate,
                        cm.EndDate,
                        Price = cm.Membership.Price + " ₽",
                        Status = cm.EndDate >= DateTime.Today ? "Активный" : "Истёкший"
                    })
                    .ToList();

                MembershipGrid.ItemsSource = data;
            }
        }

        public void LoadAvailableMemberships()
        {
            using (var db = new AppDbContext())
            {
                if (membershipDurationCmb.SelectedValue is int selectedTypeId)
                {
                    var memberships = db.Memberships
                        .Include(m => m.MembershipType)
                        .Where(m => m.MembershipTypeId == selectedTypeId)
                        .OrderByDescending(m => m.Price)
                        .ToList();

                    membershipList.ItemsSource = memberships;
                }
            }
        }


        private void membershipDurationCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            costTb.Visibility = Visibility.Collapsed;
            LoadAvailableMemberships();
        }

        private void buyMemembershipBtn_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = membershipList.SelectedItem;
            SuccessfullPayment(selectedItem.MembershipId);

        }

        private void membershipList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic selectedItem = membershipList.SelectedItem;
            if(selectedItem != null)
            {
                costTb.Visibility = Visibility.Visible;
                costTb.Text = $"К оплате: {selectedItem.Price} руб.";
                costTb.Foreground = Brushes.Green;
            }
        }

    }
}
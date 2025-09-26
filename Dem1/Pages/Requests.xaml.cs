using Dem1.DB;
using Dem1.Pages;
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
using System.Windows.Shapes;

namespace Dem1
{
    /// <summary>
    /// Логика взаимодействия для Requests.xaml
    /// </summary>
    public partial class Requests : Page
    {
        private List <PartnerStruct> partnersList;
        public Requests()
        {
            InitializeComponent();

            partnersList = new List<PartnerStruct>();
            AllPartnerProductRequest();
        }

        private void btnAddRequest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddRequest());
        }

        private void LViewProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LViewProduct.SelectedItem != null)
            {
                btnChangePartner.Visibility = Visibility.Visible;
                btnShowRequests.Visibility = Visibility.Visible;
            }
        }

        private void btnChangePartner_Click(object sender, RoutedEventArgs e)
        {
            if (LViewProduct.SelectedItem is PartnerStruct selectedItem)
            {
                var db = BuildCompanyEntities.GetContext();
                var partner = db.partners.First(x => x.partner_id == selectedItem.Id_partner);
                NavigationService.Navigate(new ChangePartner(partner));
            }
            else
                return;
        }

        public struct PartnerStruct
        {
            public string PartnerType { get; set; }
            public string PartnerName { get; set; }
            public string URIAddress { get; set; }
            public string PhoneNumber { get; set; }
            public int Ratio { get; set; }
            public int Id_partner { get; set; }
            public double Cost { get; set; }
        }

        private void AllPartnerProductRequest()
        {
            BuildCompanyEntities db = BuildCompanyEntities.GetContext();

            var partners = BuildCompanyEntities.GetContext().partners.ToList();

            foreach (var item in partners)
            {
                var partner = db.partners.First(x => x.partner_id == item.partner_id);
                string partnerType = db.typeOfPartner.First(x => x.typeOfPartner_id == item.typeOfPartner_id).typeOfPartner1;

                var partnerRequests = db.requests.Where(x => x.partner_id == item.partner_id).ToList();

                double fullPrice = 0;
                foreach (var request in partnerRequests)
                    fullPrice += db.products.First(x => x.product_id == request.product_id).minPrice * request.amountOfProduct / 100;

                PartnerStruct partnerStruct = new PartnerStruct()
                {
                    PartnerType = partnerType,
                    PartnerName = partner.nameOfPartner,
                    URIAddress = partner.addressOfPartner,
                    PhoneNumber = partner.phoneOfPartner,
                    Ratio = (int)partner.rate,
                    Cost = fullPrice,
                    Id_partner = partner.partner_id
                };
                partnersList.Add(partnerStruct);
            }
            LViewProduct.ItemsSource = partnersList;
        }

        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LViewProduct.SelectedItem is PartnerStruct selectedItem)
            {
                var db = BuildCompanyEntities.GetContext();
                var partner = db.partners.First(x => x.partner_id == selectedItem.Id_partner);
                NavigationService.Navigate(new ChangePartner(partner));
            }
            else
                return;
        }

        private void btnShowRequests_Click(object sender, RoutedEventArgs e)
        {
            if (LViewProduct.SelectedItem is PartnerStruct selectedItem)
            {
                var db = BuildCompanyEntities.GetContext();
                var partnerRequests = db.requests.Where(x => x.partner_id == selectedItem.Id_partner).ToList();
                StringBuilder sb = new StringBuilder();

                foreach (var item in partnerRequests)
                {
                    var product = db.products.First(x => x.product_id == item.product_id);
                    sb.AppendLine($"Товар: {product.nameOfProduct} количество: {item.amountOfProduct}");
                }

                MessageBox.Show(sb.ToString(), "Заявки партнёра", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                return;
        }
    }
}

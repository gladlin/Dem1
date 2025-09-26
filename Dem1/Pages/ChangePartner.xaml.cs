using Dem1.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

namespace Dem1.Pages
{
    /// <summary>
    /// Логика взаимодействия для ChangePartner.xaml
    /// </summary>
    public partial class ChangePartner : Page
    {
        partners selectedPartner;
        public ChangePartner(partners partner)
        {
            InitializeComponent();

            selectedPartner = partner;
            var db = BuildCompanyEntities.GetContext();

            cmbPartnerType.ItemsSource = db.typeOfPartner.Select(x => x.typeOfPartner1).ToList();

            cmbPartnerType.SelectedItem = db.typeOfPartner.First(x => x.typeOfPartner_id == partner.typeOfPartner_id).typeOfPartner1;

            tbPartnerName.Text = partner.nameOfPartner;
            tbDirector.Text = partner.directorName;
            tbPartnerAddress.Text = partner.addressOfPartner;
            tbRatio.Text = partner.rate.ToString();
            tbProneNumber.Text = partner.phoneOfPartner;
            tbEmail.Text = partner.mailOfPartner;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int rate_out = -1;
            if(!Int32.TryParse(tbRatio.Text, out rate_out))
            {
                MessageBox.Show("Рейтинг должен быть числом!");
                return;
            }
            if(rate_out < 0 || rate_out > 10)
            {
                MessageBox.Show("Рейтинг должен быть целым числом от 0 до 10");
                return;
            }
            if(String.IsNullOrWhiteSpace(tbPartnerName.Text) ||
               String.IsNullOrWhiteSpace(tbDirector.Text) ||
               String.IsNullOrWhiteSpace(tbPartnerAddress.Text) ||
              String.IsNullOrWhiteSpace(tbProneNumber.Text) ||
               String.IsNullOrWhiteSpace(tbEmail.Text))
            {
                MessageBox.Show($"Все поля должны быть заполнены!");
                return;
            }
            var db = BuildCompanyEntities.GetContext();
            partners partner = db.partners.First(x => x.partner_id == selectedPartner.partner_id);

            partner.typeOfPartner_id = db.typeOfPartner.First(x => x.typeOfPartner1 == cmbPartnerType.SelectedItem.ToString()).typeOfPartner_id;
            partner.nameOfPartner = tbPartnerName.Text;
            partner.directorName = tbDirector.Text;
            partner.addressOfPartner = tbPartnerAddress.Text;
            partner.rate = rate_out;
            partner.phoneOfPartner = tbProneNumber.Text;
            partner.mailOfPartner = tbEmail.Text;

            var context = new ValidationContext(partner);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var partnerValid = Validator.TryValidateObject(partner, context, results, true);

            if (!partnerValid)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in results)
                    sb.AppendLine(error.ToString());

                MessageBox.Show(sb.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Успешное изменение данных партнёра", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new Requests());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
    }
}

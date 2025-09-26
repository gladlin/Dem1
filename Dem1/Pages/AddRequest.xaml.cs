using Dem1.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Dem1
{
    /// <summary>
    /// Логика взаимодействия для AddRequest.xaml
    /// </summary>
    public partial class AddRequest : Page
    {
        public AddRequest()
        {
            InitializeComponent();

            var db = BuildCompanyEntities.GetContext();

            cmbPartner.ItemsSource = db.partners.Select(x => x.nameOfPartner).ToList();
            cmbProduct.ItemsSource = db.products.Select(x => x.nameOfProduct).ToList();
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {

            var db = BuildCompanyEntities.GetContext();

            requests request = new requests();

            if (Int32.TryParse(tbCount.Text, out int value))
            {
                if (value > 0 || value > int.MaxValue)
                    request.amountOfProduct = value;
                else
                {
                    MessageBox.Show("Количество продукта не может быть отрицательно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Введите количество продукта в виде целого числа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(cmbPartner.SelectedIndex == -1 || cmbProduct.SelectedIndex == -1)
            {
                MessageBox.Show("Поля продавца и продватм должны быть выбраны!");
                return;
            }

            request.partner_id = db.partners.First(x => x.nameOfPartner == cmbPartner.SelectedItem.ToString()).partner_id;
            request.product_id = db.products.First(x => x.nameOfProduct == cmbProduct.SelectedItem.ToString()).product_id;


            var context = new ValidationContext(request);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var partnerValid = Validator.TryValidateObject(request, context, results, true);

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
                    db.requests.Add(request);
                    db.SaveChanges();
                    MessageBox.Show("Успешное добавление заявки", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    NavigationService.Navigate(new Requests());
                }


            }
        }
    }
}

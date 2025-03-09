using Olympiad.Controllers;
using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace Olympiad.View
{
    /// <summary>
    /// Логика взаимодействия для CertificatesPage.xaml
    /// </summary>
    public partial class CertificatesPage : Page
    {
        private int _olympiadId;
        private Core db = new Core();
        private List<Certificates> certificates;
        CertificatesController certificatesController = new CertificatesController();
        private bool isNewEntry = false;

        public CertificatesPage(int olympiadId)
        {
            InitializeComponent();
            _olympiadId = olympiadId;
            LoadCertificates();
            InitializeComboBox();
        }

        private void LoadCertificates()
        {
            certificates = certificatesController.LoadAllCertificates(_olympiadId);

        }

        private void InitializeComboBox()
        {
            CertificatesComboBox.ItemsSource = Enumerable.Range(1, certificates.Count).ToList();
            CertificatesComboBox.SelectedIndex = certificates.Any() ? 0 : -1;
            DisplaySelectedCertificate();
        }

        private void DisplaySelectedCertificate()
        {
            if (CertificatesComboBox.SelectedIndex >= 0 && !isNewEntry)
            {
                var selectedCert = certificates[CertificatesComboBox.SelectedIndex];
                LinkTextBox.Text = selectedCert.FilePath;
                DescriptionTextBox.Text = selectedCert.Description;
            }
            else
            {
                LinkTextBox.Text = string.Empty;
                DescriptionTextBox.Text = string.Empty;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (certificatesController.CheckNewCertificate(_olympiadId, LinkTextBox.Text, DescriptionTextBox.Text))
                {
                    if (isNewEntry || CertificatesComboBox.SelectedIndex == -1)
                    {
                        certificatesController.AddNewCertificate(_olympiadId, LinkTextBox.Text, DescriptionTextBox.Text);
                        MessageBox.Show("Сертификат добавлен");
                    }
                    else
                    {
                        var certId = certificates[CertificatesComboBox.SelectedIndex].CertificateId;
                        certificatesController.UpdateDataCertificate(certId, _olympiadId, LinkTextBox.Text, DescriptionTextBox.Text);
                    }

                    LoadCertificates();
                    InitializeComboBox();
                    isNewEntry = false;
                    MessageBox.Show("Данные успешно сохранены!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddMoreBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(LinkTextBox.Text))
            {
                SaveBtn_Click(sender, e);
            }


            isNewEntry = true;
            CertificatesComboBox.SelectedIndex = -1;
            DisplaySelectedCertificate();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void CertificatesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isNewEntry = false;
            DisplaySelectedCertificate();
        }
    }
}

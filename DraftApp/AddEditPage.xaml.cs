using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace DraftApp
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        Materials _currentMaterials = new Materials();
        public AddEditPage(Materials selected)
        {
            InitializeComponent();
            if (selected != null)
            {
                BtnDelete.Visibility = Visibility.Visible;
                _currentMaterials = selected;
            }
            DataContext = _currentMaterials;
            ComboTM.ItemsSource = DraftDatabaseEntities.GetContext().TypesMaterials.ToList();
            ComboUM.ItemsSource = DraftDatabaseEntities.GetContext().UnitsMeasurement.ToList();
            ComboSuppliers.ItemsSource = DraftDatabaseEntities.GetContext().Suppliers.ToList();
        }

        private void BtnChangePicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog GetImageDialog = new OpenFileDialog();
            GetImageDialog.Filter = "Файлы изображений: (*.png, *.jpg)|*.png;*.jpg";
            if (GetImageDialog.ShowDialog() == true)
            {
                _currentMaterials.Image = File.ReadAllBytes(GetImageDialog.FileName);
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(GetImageDialog.FileName);
                myBitmapImage.EndInit();
                image.Source = myBitmapImage;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentMaterials.Name))
                errors.AppendLine("Укажите наименования материала");
            if (_currentMaterials.TypesMaterials == null)
                errors.AppendLine("Выберите тип материала");
            if (_currentMaterials.Cost < 0)
                errors.AppendLine("Цена материала не может быть отрицательной");
            if (_currentMaterials.MinimumAllowableQuantity < 0)
                errors.AppendLine("Минимальное количество не может быть отрицательным");
            if(string.IsNullOrWhiteSpace(_currentMaterials.QuantityStock.ToString()))
                errors.AppendLine("Укажите количество на складе");
            if(_currentMaterials.UnitsMeasurement == null)
                errors.AppendLine("Выберите единицу измерения");
            if (_currentMaterials.QuantityPackage < 1)
                errors.AppendLine("Количество в упаковке не может быть меньше единицы");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (_currentMaterials.IdM == 0)
                DraftDatabaseEntities.GetContext().Materials.Add(_currentMaterials);
            try
            {
                DraftDatabaseEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена!");
                BtnDelete.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BtnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (ComboSuppliers.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите поставщика");
                return;
            }  
            _currentMaterials.Suppliers.Add(ComboSuppliers.SelectedItem as Suppliers);
            ListBoxSuppliers.ItemsSource = _currentMaterials.SuppliersName.ToList();
        }

        private void BtnDeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxSuppliers.SelectedItem == null)
            {
                MessageBox.Show("Выберите поставщика для удаления");
                return;
            }
            var pk = DraftDatabaseEntities.GetContext().Suppliers.FirstOrDefault(x => x.Name == ListBoxSuppliers.SelectedValue.ToString()).IdS;
            var supplierForRemoving = DraftDatabaseEntities.GetContext().Suppliers.Find(pk);
            if (MessageBox.Show($"Вы точно хотите удалить следующего {supplierForRemoving.Name} поставщика?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _currentMaterials.Suppliers.Remove(supplierForRemoving);
                ListBoxSuppliers.ItemsSource = _currentMaterials.SuppliersName.ToList();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы точно хотите удалить следующий {_currentMaterials.Name} материал?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    DraftDatabaseEntities.GetContext().Materials.Remove(_currentMaterials);
                    DraftDatabaseEntities.GetContext().SaveChanges();
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}

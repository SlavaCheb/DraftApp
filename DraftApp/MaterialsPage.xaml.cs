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

namespace DraftApp
{
    /// <summary>
    /// Логика взаимодействия для MaterialsPage.xaml
    /// </summary>
    public partial class MaterialsPage : Page
    {
        public int MaterialsSelectedCount = 0;
        int materialsCount = DraftDatabaseEntities.GetContext().MaterialsView.ToList().Count;
        IEnumerable<MaterialsView> materials = DraftDatabaseEntities.GetContext().MaterialsView.ToList();
        public string[] SortList { get; set; } = {
            "Без сортировки",
            "Наименование по возрастанию",
            "Наименование по убыванию",
            "Остаток на складе по возрастанию",
            "Остаток на складе по убыванию",
            "Стоимость по возрастанию",
            "Стоимость по убыванию"};
        void UpdateMaterials()
        {
            var currentMaterials = DraftDatabaseEntities.GetContext().MaterialsView.ToList();
            if (ComboFiltr.SelectedIndex > 0)
                currentMaterials = currentMaterials.Where(p=>p.IdTM.ToString().Contains(ComboFiltr.SelectedValue.ToString())).ToList();
            currentMaterials = currentMaterials.Where(p => p.Name.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            TextBlock1.Text = currentMaterials.Count().ToString();
            switch (ComboSort.SelectedIndex)
            {
                case 0:
                    LViewMaterials.ItemsSource = currentMaterials;
                    break;
                case 1:
                    LViewMaterials.ItemsSource = currentMaterials.OrderBy(p => p.Name);
                    break;
                case 2:
                    LViewMaterials.ItemsSource = currentMaterials.OrderByDescending(p => p.Name);
                    break;
                case 3:
                    LViewMaterials.ItemsSource = currentMaterials.OrderBy(p => p.QuantityStock);
                    break;
                case 4:
                    LViewMaterials.ItemsSource = currentMaterials.OrderByDescending(p => p.QuantityStock);
                    break;
                case 5:
                    LViewMaterials.ItemsSource = currentMaterials.OrderBy(p => p.Cost);
                    break;
                case 6:
                    LViewMaterials.ItemsSource = currentMaterials.OrderByDescending(p => p.Cost);
                    break;
            }
        }
        public MaterialsPage()
        {
            InitializeComponent();
            ComboSort.ItemsSource = SortList;
            var allTypes = DraftDatabaseEntities.GetContext().TypesMaterials.ToList();
            allTypes.Insert(0, new TypesMaterials
            {
                Name = "Все типы"
            });
            ComboFiltr.ItemsSource = allTypes;
            UpdateMaterials();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComboFiltr.SelectedIndex > 0)
                TextBlock2.Text = materials.Where(p => p.IdTM.ToString().Contains(ComboFiltr.SelectedValue.ToString())).ToList().Count.ToString();
            else
                TextBlock2.Text = materialsCount.ToString();
            UpdateMaterials();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMaterials();
        }

        private void ComboFiltr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TBoxSearch.Text != "")
                TextBlock2.Text = materials.Where(p => p.Name.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList().Count.ToString();
            else
                TextBlock2.Text = materialsCount.ToString();
            UpdateMaterials();
        }

        private void LViewMaterials_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MaterialsSelectedCount = LViewMaterials.SelectedItems.Count;
            if (MaterialsSelectedCount > 1) ChangeMinimumQuantityButton.Visibility = Visibility.Visible;
            else ChangeMinimumQuantityButton.Visibility = Visibility.Collapsed;
        }

        private void ChangeMinimumQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            decimal Max = -1;
            List<int> idList = new List<int>();
            foreach (MaterialsView item in LViewMaterials.SelectedItems)
            {
                if (item.MinimumAllowableQuantity > Max)
                    Max = item.MinimumAllowableQuantity;
                idList.Add(item.IdM);
            }   
            var NewWindow = new ChangeMinimumQuantityWindow(Max);
            if ((bool)NewWindow.ShowDialog())
            {
                foreach (int item in idList)
                {
                    DraftDatabaseEntities.GetContext().Materials.Find(item).MinimumAllowableQuantity = NewWindow.Result;
                }
                try
                {
                    DraftDatabaseEntities.GetContext().SaveChanges();
                    LViewMaterials.ItemsSource = DraftDatabaseEntities.GetContext().MaterialsView.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                DraftDatabaseEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                LViewMaterials.ItemsSource = DraftDatabaseEntities.GetContext().MaterialsView.ToList();
            }
        }

        private void LViewMaterials_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MaterialsView m = LViewMaterials.SelectedItem as MaterialsView;
            Manager.MainFrame.Navigate(new AddEditPage(DraftDatabaseEntities.GetContext().Materials.Find(m.IdM)));
        }
    }
}

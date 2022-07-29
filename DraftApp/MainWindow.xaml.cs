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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Page1());
            Manager.MainFrame = MainFrame;
            //import();
        }

        private void import()
        {
            var file = File.ReadAllLines(@"C:\Users\slava\OneDrive\Рабочий стол\задание\Сессия 1\materials_b_import.txt");
            foreach (var line in file)
            {
                var data = line.Split('\t');
                var temp = new Materials
                {
                    Name = data[0],
                    IdTM = int.Parse(data[1]),
                    Image = (data[2] == "") ? null : File.ReadAllBytes(data[2]),
                    Cost = decimal.Parse(data[3]),
                    QuantityStock = int.Parse(data[4]),
                    MinimumAllowableQuantity = int.Parse(data[5]),
                    QuantityPackage = int.Parse(data[6]),
                    IdUM = int.Parse(data[7])
                };
                DraftDatabaseEntities.GetContext().Materials.Add(temp);
                DraftDatabaseEntities.GetContext().SaveChanges();
            }
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if(MainFrame.CanGoBack)
            {
                BtnBack.Visibility = Visibility.Visible;
            }
            else
            {
                BtnBack.Visibility = Visibility.Hidden;
            }
        }
    }
}

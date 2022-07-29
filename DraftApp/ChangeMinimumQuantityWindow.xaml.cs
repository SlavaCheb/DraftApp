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

namespace DraftApp
{
    /// <summary>
    /// Логика взаимодействия для ChangeMinimumQuantityWindow.xaml
    /// </summary>
    public partial class ChangeMinimumQuantityWindow : Window
    {
        public int Result;
        public ChangeMinimumQuantityWindow(decimal Max)
        {
            InitializeComponent();
            textBoxMin.Text = Max.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Result = Convert.ToInt32(textBoxMin.Text);
                DialogResult = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Количество должно быть целым числом");
            }
        }
    }
}

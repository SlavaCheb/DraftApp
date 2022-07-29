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
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        static int counter = 0;
        public Page1()
        {
            InitializeComponent();
        }

        private void ShowPasswordCharsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MyPasswordBox.Visibility = System.Windows.Visibility.Collapsed;
            MyTextBox.Text = MyPasswordBox.Password;
            MyTextBox.Visibility = System.Windows.Visibility.Visible;
            MyTextBox.Focus();
        }

        private void ShowPasswordCharsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MyPasswordBox.Visibility = System.Windows.Visibility.Visible;
            MyTextBox.Visibility = System.Windows.Visibility.Collapsed;
            MyPasswordBox.Focus();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string login = textBoxLogin.Text;
            string pass = MyPasswordBox.Password;
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(login))
                errors.AppendLine("Укажите логин");
            if (string.IsNullOrWhiteSpace(pass))
                errors.AppendLine("Укажите пароль");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (login == "admin" && pass == "admin")
                Manager.MainFrame.Navigate(new MaterialsPage());
            else
            {
                counter++;
                MessageBox.Show("Вы ввели что-то некорректно!");
            }
            if (counter >= 3)
            {
                textBoxLogin.IsEnabled = false;
                MyPasswordBox.IsEnabled = false;
                MyTextBox.IsEnabled = false;
                await Task.Delay(60000);
                counter = 0;
                textBoxLogin.IsEnabled = true;
                MyPasswordBox.IsEnabled = true;
                MyTextBox.IsEnabled = true;
            }
        }
    }
}

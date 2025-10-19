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
using System.Windows.Shapes;
using Library_ProkoshevYP;
using Microsoft.Win32;

namespace proYP04
{
    /// <summary>
    /// Логика взаимодействия для EmployeeWin.xaml
    /// </summary>
    public partial class EmployeeWin : Window
    {
        Entities db = new Entities();
        Сотрудник User;
        string FILENAME = "";
        Права_доступа Access;
        public EmployeeWin(Сотрудник user)
        {
            InitializeComponent();
            try
            {
                User = user;
                var Users = db.Сотрудник.FirstOrDefault(x => x.Логин == user.Логин) as Сотрудник;
                User = Users;
                var access = db.Права_доступа.FirstOrDefault(x => x.Логин == User.Логин) as Права_доступа;
                Access = access;

                string imageName = $"{Users.Фото}";
                string imagePath = $"UsersPhoto/{imageName}";
                BitmapImage userbitmap = new BitmapImage();
                userbitmap.BeginInit();
                userbitmap.UriSource = new Uri(imagePath, UriKind.Relative);
                userbitmap.EndInit();
                Iphoto.Source = userbitmap;

                userName.Text = User.Имя;
                userSurname.Text = User.Фамилия;
                userPatronymic.Text = User.Отчество;
                userBorn.Text = User.Дата_рождения?.ToString("dd.MM.yyyy");
                userPhone.Text = User.Номер_телефона;
                userEmail.Text = User.Email;
                userLogin.Text = User.Логин;
                userPasswordSerias.Text = User.Серия_и_номер.Substring(0, 4);
                userPassportNumber.Text = User.Серия_и_номер.Substring(User.Серия_и_номер.Length - 6);


                TBuserName.Text = User.Имя;
                TBuserSurname.Text = User.Фамилия;
                TBuserPatronymic.Text = User.Отчество;
                DPuserBorn.Text = User.Дата_рождения?.ToString("dd.MM.yyyy");
                TBuserPhone.Text = User.Номер_телефона;
                TBuserEmail.Text = User.Email;
                TBuserLogin.Text = User.Логин;
                TBuserPasswordSerias.Text = User.Серия_и_номер.Substring(0, 4);
                TBuserPassportNumber.Text = User.Серия_и_номер.Substring(User.Серия_и_номер.Length - 6);
            }
            catch
            {
                MessageBox.Show("Ошибка при инициализации");
            }
            
        }

        private void btnEditImg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.PNG;*.JPG;*.JPEG;*.BMP)|*.PNG;*.JPG;*.JPEG;*.BMP";
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    string fileName = System.IO.Path.GetFileName(filePath);
                    MessageBox.Show("Выбрано изображение: " + fileName, "Загруженный файл");
                    FILENAME = fileName;
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new System.Uri(openFileDialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    Iphoto.Source = bitmap;
                    string relativePath = $"{fileName}";
                    User.Фото = FILENAME;
                    File.Copy(filePath, @"..\..\UsersPhoto\" + fileName, overwrite: true);
                    db.SaveChanges();
                }
            }
            catch 
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void BTNsaveChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = TBuserLogin.Text;
                if (db.Сотрудник.Any(u => u.Логин == login && (User.Код == null || u.Код != User.Код)))
                {
                    MessageBox.Show("Логин уже занят");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TBuserName.Text) ||
                    string.IsNullOrWhiteSpace(TBuserSurname.Text) ||
                    string.IsNullOrWhiteSpace(TBuserEmail.Text) ||
                    string.IsNullOrWhiteSpace(TBuserLogin.Text) ||
                    string.IsNullOrWhiteSpace(TBuserPhone.Text) ||
                    string.IsNullOrWhiteSpace(DPuserBorn.Text) ||
                    string.IsNullOrWhiteSpace(TBuserPasswordSerias.Text) ||
                    string.IsNullOrWhiteSpace(TBuserPassportNumber.Text)
                    )
                {
                    MessageBox.Show("Необходимо заполнить все поля");
                    return;
                }

                if (!Class1.ValidPhone(TBuserPhone.Text))
                {
                    MessageBox.Show("Номер телефона должен начинаться на 8 или +7 и должно быть 11 цифр");
                    return;
                }
                if (!Class1.ValidEmail(TBuserEmail.Text))
                {
                    MessageBox.Show("Email не прошел проверку");
                    return;
                }

                if (DPuserBorn.SelectedDate == null)
                {
                    MessageBox.Show("Введите дату рождения");
                    return;
                }
                if (DateTime.Now.Year - DPuserBorn.SelectedDate.Value.Year > 90)
                {
                    MessageBox.Show("Вы слишком стары :)");
                    return;
                }
                if (DPuserBorn.SelectedDate > DateTime.Now.Date)
                {
                    MessageBox.Show("Ошибка, выберите другую дату");
                    return;
                }

                if (TBuserSurname.Text.Length > 50)
                {
                    MessageBox.Show("Слишком длинная фамилия. (макс.50)");
                    return;
                }
                if (TBuserName.Text.Length > 50)
                {
                    MessageBox.Show("Слишком длинное имя. (макс.50)");
                    return;
                }
                if (TBuserPatronymic.Text.Length > 50)
                {
                    MessageBox.Show("Слишком длинное отчество. (макс.50)");
                    return;
                }
                if (TBuserEmail.Text.Length > 40)
                {
                    MessageBox.Show("Слишком длинный Email. (макс. 40)");
                    return;
                }


                DateTime BirthDate = DPuserBorn.SelectedDate.Value;
                DateTime Now = DateTime.Today;
                int age = Now.Year - BirthDate.Year;
                if (BirthDate.Date > Now.AddYears(-age))
                {
                    age--;
                }
                if (age < 18)
                {
                    MessageBox.Show($"Вам нет 18 лет");
                    return;
                }
                

                if (TBuserPasswordSerias.Text.Length != 4)
                {
                    MessageBox.Show("Серия паспорта должна состоять из 4 цифр");
                    return;
                }
                if (TBuserPassportNumber.Text.Length != 6)
                {
                    MessageBox.Show("Номер паспорта должн состоять из 6 цифр");
                    return;
                }


                User.Имя = TBuserName.Text;
                User.Фамилия = TBuserSurname.Text;
                User.Отчество = TBuserPatronymic.Text;
                User.Дата_рождения = DPuserBorn.SelectedDate;
                User.Номер_телефона = TBuserPhone.Text;
                User.Email = TBuserEmail.Text;
                User.Логин = TBuserLogin.Text;
                Access.Логин = TBuserLogin.Text;
                User.Серия_и_номер = TBuserPasswordSerias.Text + " " + TBuserPassportNumber.Text;
                string Passpost = User.Серия_и_номер;
                db.SaveChanges();
                MessageBox.Show("Данные сохранены");

                userName.Text = User.Имя;
                userSurname.Text = User.Фамилия;
                userPatronymic.Text = User.Отчество;
                userBorn.Text = User.Дата_рождения?.ToString("dd.MM.yyyy");
                userPhone.Text = User.Номер_телефона;
                userEmail.Text = User.Email;
                userLogin.Text = User.Логин;
                userPasswordSerias.Text = User.Серия_и_номер.Substring(0, 4);
                userPassportNumber.Text = User.Серия_и_номер.Substring(User.Серия_и_номер.Length - 6);

                userName.Visibility = Visibility.Visible;
                userSurname.Visibility = Visibility.Visible;
                userPatronymic.Visibility = Visibility.Visible;
                userBorn.Visibility = Visibility.Visible;
                userPhone.Visibility = Visibility.Visible;
                userEmail.Visibility = Visibility.Visible;
                userLogin.Visibility = Visibility.Visible;
                userPasswordSerias.Visibility = Visibility.Visible;
                userPassportNumber.Visibility = Visibility.Visible;

                TBuserName.Visibility = Visibility.Collapsed;
                TBuserSurname.Visibility = Visibility.Collapsed;
                TBuserPatronymic.Visibility = Visibility.Collapsed;
                DPuserBorn.Visibility = Visibility.Collapsed;
                TBuserPhone.Visibility = Visibility.Collapsed;
                TBuserEmail.Visibility = Visibility.Collapsed;
                TBuserLogin.Visibility = Visibility.Collapsed;
                TBuserPasswordSerias.Visibility = Visibility.Collapsed;
                TBuserPassportNumber.Visibility = Visibility.Collapsed;
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            
        }

        private void BTNeditUser_Click(object sender, RoutedEventArgs e)
        {
            userName.Visibility = Visibility.Collapsed;
            userSurname.Visibility = Visibility.Collapsed;
            userPatronymic.Visibility = Visibility.Collapsed;
            userBorn.Visibility = Visibility.Collapsed;
            userPhone.Visibility = Visibility.Collapsed;
            userEmail.Visibility = Visibility.Collapsed;
            //userLogin.Visibility = Visibility.Collapsed;

            TBuserName.Visibility = Visibility.Visible;
            TBuserSurname.Visibility = Visibility.Visible;
            TBuserPatronymic.Visibility= Visibility.Visible;
            DPuserBorn.Visibility = Visibility.Visible;
            TBuserPhone.Visibility = Visibility.Visible;
            TBuserEmail.Visibility = Visibility.Visible;
            //TBuserLogin.Visibility = Visibility.Visible;
            TBuserPasswordSerias.Visibility= Visibility.Visible;
            TBuserPassportNumber.Visibility= Visibility.Visible;
        }

        private void TBuserPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, 0) || "+".Contains(e.Text)))
            {
                e.Handled = true;
            }
        }

        private void TBuserName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void TBuserSurname_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void TBuserPatronymic_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true; 
            }
        }

        private void TBuserPasswordSerias_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void TBuserPassportNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}

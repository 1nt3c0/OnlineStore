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
using Library_ProkoshevYP;

namespace proYP04
{
    /// <summary>
    /// Логика взаимодействия для AdminWin.xaml
    /// </summary>
    public partial class AdminWin : Window
    {
        Entities db = new Entities();
        Сотрудник User;
        Права_доступа Access;
        public AdminWin()
        {
            InitializeComponent();
            try
            {
                DGEmployee.ItemsSource = db.История_входа.ToList();
                CBEmployee.ItemsSource = db.Сотрудник.ToList();
                CBrole.ItemsSource = db.Должность.Where(x => x.Код == 2 || x.Код == 1).ToList();
                CBsubdivision.ItemsSource = db.Подразделение.ToList();
            }
            catch
            {
                MessageBox.Show("Ошибка при инициализации");
            }

        }

        private void CBEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var user = CBEmployee.SelectedItem as Сотрудник;
            User = user;
            var access = db.Права_доступа.FirstOrDefault(x => x.Логин == User.Логин) as Права_доступа;
            Access = access;

            userName.Text = user.Имя;
            userSurname.Text = user.Фамилия;
            userPatronymic.Text = user.Отчество;
            userBorn.Text = user.Дата_рождения?.ToString("dd.MM.yyyy");
            userPhone.Text = user.Номер_телефона;
            userEmail.Text = user.Email;
            userLogin.Text = user.Логин;
            userPasswordSerias.Text = user.Серия_и_номер.Substring(0, 4);
            userPassportNumber.Text = user.Серия_и_номер.Substring(user.Серия_и_номер.Length - 6);


            TBuserName.Text = user.Имя;
            TBuserSurname.Text = user.Фамилия;
            TBuserPatronymic.Text = user.Отчество;
            DPuserBorn.Text = user.Дата_рождения?.ToString("dd.MM.yyyy");
            TBuserPhone.Text = user.Номер_телефона;
            TBuserEmail.Text = user.Email;
            TBuserLogin.Text = user.Логин;
            TBuserPasswordSerias.Text = user.Серия_и_номер.Substring(0, 4);
            TBuserPassportNumber.Text = user.Серия_и_номер.Substring(user.Серия_и_номер.Length - 6);
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
            TBuserPatronymic.Visibility = Visibility.Visible;
            DPuserBorn.Visibility = Visibility.Visible;
            TBuserPhone.Visibility = Visibility.Visible;
            TBuserEmail.Visibility = Visibility.Visible;
            //TBuserLogin.Visibility = Visibility.Visible;
            TBuserPasswordSerias.Visibility = Visibility.Visible;
            TBuserPassportNumber.Visibility = Visibility.Visible;
        }

        private void BTNsaveChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = TBuserLogin.Text;

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

                if (!Class1.ValidEmail(TBuserEmail.Text))
                {
                    MessageBox.Show("Email не прошел проверку");
                    return;
                }

                if (!Class1.ValidPhone(TBuserPhone.Text))
                {
                    MessageBox.Show("Номер телефона должен начинаться на 8 или +7 и должно быть 11 цифр");
                    return;
                }

                if (!Class1.ValidLogin(TBuserLogin.Text))
                {
                    MessageBox.Show("Логин не прошел валидность");
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
                CBEmployee.ItemsSource = db.Сотрудник.ToList();
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

        private void BTNreg_Click(object sender, RoutedEventArgs e)
        {
            string Surname = TBsurname.Text;
            string Name = TBname.Text;
            string Patronymic = TBpatronymic.Text;
            string Email = TBemail.Text;
            string Login = TBloginReg.Text;
            string Password = PBpassword.Password;
            string invisPassword = regTBpassword.Text;

            if (!Class1.ValidPassword(Password))
            {
                MessageBox.Show("Пароль не прошел проверку (от 8 символов и минимум 1 цифра)");
                return;
            }

            if (!Class1.ValidEmail(Email))
            {
                MessageBox.Show("Email не прошел проверку");
                return;
            }

            if (!Class1.ValidPhone(TBnumPhone.Text))
            {
                MessageBox.Show("Номер телефона должен начинаться на 8 или +7 и должно быть 11 цифр");
                return;
            }

            if (db.Сотрудник.Any(u => u.Логин == Login))
            {
                MessageBox.Show("Логин уже занят");
                return;
            }

            if (Surname == "")
            {
                MessageBox.Show("Поле фамилии не может быть пустым");
                return;
            }
            if (Surname.Length > 30)
            {
                MessageBox.Show("Фамилия не может быть больше 50 букв");
                return;
            }

            if (Name == "")
            {
                MessageBox.Show("Поле имени не может быть пустым");
                return;
            }
            if (Name.Length > 50)
            {
                MessageBox.Show("Имя не может быть больше 50 букв");
                return;
            }

            if (Patronymic == "")
            {
                MessageBox.Show("Поле отчества не может быть пустым");
                return;
            }
            if (Patronymic.Length > 50)
            {
                MessageBox.Show("Отчество не может быть больше 50 букв");
                return;
            }

            if (Email == "")
            {
                MessageBox.Show("Email не может быть пустым");
                return;
            }
            if (Email.Length > 50)
            {
                MessageBox.Show("Email не может быть больше 50 символов");
                return;
            }

            if (Login == "")
            {
                MessageBox.Show("Логин не может быть пустым");
                return;
            }

            if (Password == "")
            {
                MessageBox.Show("Пароль не может быть пустым");
                return;
            }


            if (CBrole.SelectedItem == null)
            {
                MessageBox.Show("Выберите должность");
                return;
            }

            if (CBsubdivision.SelectedItem == null)
            {
                MessageBox.Show("Выберите подразделение");
                return;
            }

            if (TBserPas.Text == "")
            {
                MessageBox.Show("Заполните серию паспорта");
                return;
            }
            if (TBserPas.Text.Length != 4)
            {
                MessageBox.Show("Серия паспорта должна состоять из 4 цифр");
                return;
            }

            if (TBnumPas.Text == "")
            {
                MessageBox.Show("Заполните номер паспорта");
                return;
            }
            if (TBnumPas.Text.Length != 6)
            {
                MessageBox.Show("Номер паспорта должн состоять из 6 цифр");
                return;
            }

            if (TBnumPhone.Text == "")
            {
                MessageBox.Show("Введите номер телефона");
                return;
            }

            if (DPborn.SelectedDate == null)
            {
                MessageBox.Show("Введите дату рождения");
                return;
            }
            if (DateTime.Now.Year - DPborn.SelectedDate.Value.Year > 90)
            {
                MessageBox.Show("Вы слишком стары :)");
                return;
            }
            if (DPborn.SelectedDate > DateTime.Now.Date)
            {
                MessageBox.Show("Ошибка, выберите другую дату");
                return;
            }

            DateTime BirthDate = DPborn.SelectedDate.Value;
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

            string hashedPassword = HashPassword(Password);
            var Role = CBrole.SelectedItem as Должность;
            var Subdivision = CBsubdivision.SelectedItem as Подразделение;
            string Passport = TBserPas.Text + " " + TBnumPas.Text;
            string NumPhone = TBnumPhone.Text;

            var newAccess = new Права_доступа
            {
                Логин = Login,
                Пароль = hashedPassword
            };
            db.Права_доступа.Add(newAccess);

            var newUser = new Сотрудник
            {
                Фамилия = Surname,
                Имя = Name,
                Отчество = Patronymic,
                Email = Email,
                Логин = newAccess.Логин,
                Должность = Role.Код,
                Подразделение = Subdivision.Код_подразделения,
                Серия_и_номер = Passport,
                Номер_телефона = NumPhone,
                Дата_рождения = BirthDate.Date,
            };
            db.Сотрудник.Add(newUser);
            db.SaveChanges();
            CBEmployee.ItemsSource = db.Сотрудник.ToList();
            MessageBox.Show("Вы успешно зарегистрировали сотрудника");
        }
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        private void regCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (regCheckBox.IsChecked == true)
            {
                regTBpassword.Text = PBpassword.Password;
                regTBpassword.Visibility = Visibility.Visible;
                PBpassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                PBpassword.Password = regTBpassword.Text;
                PBpassword.Visibility = Visibility.Visible;
                regTBpassword.Visibility = Visibility.Collapsed;
            }
        }
        private void PBpassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            regTBpassword.Text = PBpassword.Password;
        }

        private void regTBpassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            PBpassword.Password = regTBpassword.Text;
        }
        private void TBsurname_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void TBname_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void TBpatronymic_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true;
            }
        }


        private void TBserPas_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void TBnumPas_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void TBnumPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, 0) || "+".Contains(e.Text)))
            {
                e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            string User = TBfindUser.Text;
            var Data = db.История_входа.Where(x => x.Сотрудник1.Логин.Contains(User)).ToList();
            DGEmployee.ItemsSource = Data;
        }
    }
}

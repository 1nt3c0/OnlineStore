using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
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
    /// Логика взаимодействия для UserRegAut.xaml
    /// </summary>
    public partial class UserRegAut : Window
    {
        Entities db = new Entities();
        public UserRegAut()
        {
            InitializeComponent();
        }

        private void BTNreg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Surname = TBsurname.Text;
                string Name = TBname.Text;
                string Patronymic = TBpatronymic.Text;
                string NumPhone = TBnumPhone.Text;
                string Gender = CBgender.SelectedItem.ToString();
                string Born = DPborn.SelectedDate.ToString();
                string Login = TBloginReg.Text;
                string Password = PBpassword.Visibility == Visibility.Visible ? PBpassword.Password : regTBpassword.Text;
                string Email = TBemail.Text;

                if (string.IsNullOrWhiteSpace(Gender) ||
                    string.IsNullOrWhiteSpace(Name) ||
                    string.IsNullOrWhiteSpace(Patronymic)||
                    string.IsNullOrWhiteSpace(Surname)||
                    string.IsNullOrWhiteSpace(NumPhone) ||
                    string.IsNullOrWhiteSpace(Login) ||
                    string.IsNullOrWhiteSpace(Password) ||
                    string.IsNullOrWhiteSpace(Email)
                    )
                {
                    MessageBox.Show("Заполните все поля");
                    return;
                }


                if (db.Клиенты.Any(u => u.Логин == Login))
                {
                    MessageBox.Show("Логин уже занят");
                    return;
                }

                if (!Class1.ValidEmail(Email))
                {
                    MessageBox.Show("Email не прошел проверку");
                    return;
                }

                if (!Class1.ValidPassword(Password))
                {
                    MessageBox.Show("Пароль не прошел проверку (от 8 символов и минимум 1 цифра)");
                    return;
                }

                if (!Class1.ValidPhone(NumPhone))
                {
                    MessageBox.Show("Номер телефона должен начинаться на 8 или +7");
                    return;
                }

                if (!Class1.ValidLogin(Login))
                {
                    MessageBox.Show("Логин не прошел валидность");
                    return;
                }

                if (Login == "")
                {
                    MessageBox.Show("Логин не может быть пустым");
                    return;
                }

                if (Surname == "")
                {
                    MessageBox.Show("Поле фамилии не может быть пустым");
                    return;
                }
                if (Surname.Length > 50)
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
                if (!Class1.ValidDate(DPborn.Text)) //проверка на соответствие XX.XX.XXXX
                {
                    MessageBox.Show("Некорректная дата"); 
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

                if(CBgender.SelectedItem == null)
                {
                    MessageBox.Show("Выберите пол");
                    return;
                }


                if(CBgender.SelectedItem is ComboBoxItem selectedTagGender)
                {
                    string tagValue = selectedTagGender.Tag?.ToString(); //Пол Ж | М

                    string hashedPassword = HashPassword(Password);

                    var NewUser = new Клиенты
                    {
                        Фамилия = Surname,
                        Имя = Name,
                        Отчество = Patronymic,
                        Номер_телефона = NumPhone,
                        Дата_рождения = BirthDate.Date,
                        Пол = tagValue,
                        Логин = Login,
                        Пароль = hashedPassword,
                        Email = Email
                    };
                    db.Клиенты.Add(NewUser);
                    db.SaveChanges();
                    MessageBox.Show("Вы зарегистрировались");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
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

        private void TBnumPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, 0) || "+".Contains(e.Text)))
            {
                e.Handled = true;
            }
        }

        private void regTBpassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            PBpassword.Password = regTBpassword.Text;
        }

        private void PBpassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            regTBpassword.Text = PBpassword.Password;
        }

        private void PBpassword1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TBpassword.Text = PBpassword1.Password;
        }

        private void TBpassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            PBpassword1.Password = TBpassword.Text;
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
        private void CBpassword_Checked(object sender, RoutedEventArgs e)
        {
            if (CBpassword.IsChecked == true)
            {
                TBpassword.Text = PBpassword1.Password;
                TBpassword.Visibility = Visibility.Visible;
                PBpassword1.Visibility = Visibility.Collapsed;
            }
            else
            {
                PBpassword1.Password = TBpassword.Text;
                PBpassword1.Visibility = Visibility.Visible;
                TBpassword.Visibility = Visibility.Collapsed;
            }
        }
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private void insert_Click(object sender, RoutedEventArgs e)
        {
            string login = TBlogin.Text;
            string password = PBpassword1.Visibility == Visibility.Visible ? PBpassword1.Password : TBpassword.Text;
            string verifyPassword = HashPassword(password);
            var user = db.Клиенты.FirstOrDefault(x => x.Логин == login && x.Пароль == verifyPassword);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }
            else
            {
                if (user != null)
                {
                    //HistoryEntry(user);
                    UserWin userwin = new UserWin(user);
                    userwin.Show();
                    this.Close();
                }
            }
        }
    }
}

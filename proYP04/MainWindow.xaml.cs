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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Library_ProkoshevYP;

namespace proYP04
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Entities db = new Entities();
        public MainWindow()
        {
            InitializeComponent();
            try 
            {
                CBrole.ItemsSource = db.Должность.Where(x => x.Код == 2 || x.Код ==1).ToList();
                CBsubdivision.ItemsSource = db.Подразделение.ToList();
            }
            catch
            {
                MessageBox.Show("Ошибка при инициализации");
            }
        }

        private void insert_Click(object sender, RoutedEventArgs e)
        {
            string login = TBlogin.Text;
            string password = PBpassword1.Visibility == Visibility.Visible ? PBpassword1.Password : TBpassword.Text;
            string verifyPassword = HashPassword(password);
            var access = db.Права_доступа.FirstOrDefault(x => x.Логин == login && x.Пароль == verifyPassword);

            if(access == null)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }
            if(verifyPassword == access.Пароль)
            {
                var user = db.Сотрудник.FirstOrDefault(x => x.Логин == access.Логин);

                if (user != null)
                {
                    if (user.Должность == 1)
                    {
                        HistoryEntry(user);
                        AdminWin admin = new AdminWin();
                        admin.Show();
                        this.Close();
                    }
                    if (user.Должность == 2)
                    {
                        HistoryEntry(user);
                        EmployeeWin employeeWin = new EmployeeWin(user);
                        employeeWin.Show();
                        this.Close();
                    }
                }
            }
        }
        void HistoryEntry(Сотрудник user)
        {
            try
            {
                var historyEntry = new История_входа
                {
                    Сотрудник = user.Код,
                    Дата_и_время = DateTime.Now
                };
                db.История_входа.Add(historyEntry);
                db.SaveChanges();
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
            string Password = PBpassword.Visibility == Visibility.Visible ? PBpassword.Password : regTBpassword.Text;

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

            if(CBsubdivision.SelectedItem == null)
            {
                MessageBox.Show("Выберите подразделение");
                return;
            }

            if (TBserPas.Text == "")
            {
                MessageBox.Show("Заполните серию паспорта");
                return;
            }
            if(TBserPas.Text.Length != 4)
            {
                MessageBox.Show("Серия паспорта должна состоять из 4 цифр");
                return;
            }

            if (TBnumPas.Text == "" )
            {
                MessageBox.Show("Заполните номер паспорта");
                return;
            }
            if (TBnumPas.Text.Length != 6)
            {
                MessageBox.Show("Номер паспорта должн состоять из 6 цифр");
                return;
            }

            if(TBnumPhone.Text == "")
            {
                MessageBox.Show("Введите номер телефона");
                return;
            }
            

            if(DPborn.SelectedDate == null)
            {
                MessageBox.Show("Введите дату рождения");
                return;
            }
            if(DateTime.Now.Year  - DPborn.SelectedDate.Value.Year > 90)
            {
                MessageBox.Show("Вы слишком стары :)");
                return;
            }
            if(DPborn.SelectedDate > DateTime.Now.Date)
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
            MessageBox.Show("Вы успешно зарегистрировались");
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            return HashPassword(inputPassword) == storedPassword;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }


        private void CBpassword_Checked(object sender, RoutedEventArgs e)
        {
           if(CBpassword.IsChecked == true)
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

        private void insertUser_Click(object sender, RoutedEventArgs e)
        {
            UserRegAut userWin = new UserRegAut();
            userWin.Show();
            this.Close();
        }
    }
}

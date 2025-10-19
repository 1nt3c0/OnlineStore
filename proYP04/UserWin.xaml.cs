using Library_ProkoshevYP;
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
using System.Windows.Shapes;

namespace proYP04
{
    /// <summary>
    /// Логика взаимодействия для UserWin.xaml
    /// </summary>
    public partial class UserWin : Window
    {
        Entities db = new Entities();
        Клиенты User;
        string FILENAME = "";
        Категории_товаров SelectedCategory;

        public UserWin(Клиенты user)
        {
            try
            {
                InitializeComponent();
                User = user;
                var Users = db.Клиенты.FirstOrDefault(x => x.Логин == user.Логин) as Клиенты;
                User = Users;
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

                if(User.Улица != null)
                {
                    userStreet.Text = User.Улица;
                    TBuserStreet.Text = User.Улица;
                }
                if(User.Номер_дома != null)
                {
                    userNumHome.Text = User.Номер_дома;
                    TBuserNumHome.Text = User.Номер_дома;
                }
                if(User.Номер_квартиры != null)
                {
                    userApartNum.Text = User.Номер_квартиры.ToString();
                    TBuserApartNum.Text = User.Номер_квартиры.ToString();
                }
                if(User.Номер_подъезда != null)
                {
                    userEntranceNum.Text = User.Номер_подъезда.ToString();
                    TBuserEntranceNum.Text = User.Номер_подъезда.ToString();
                }
                if(User.Номер_этажа != null)
                {
                    userFloorNum.Text = User.Номер_этажа.ToString();
                    TBuserFloorNum.Text = User.Номер_этажа.ToString();
                }

                TBuserName.Text = User.Имя;
                TBuserSurname.Text = User.Фамилия;
                TBuserPatronymic.Text = User.Отчество;
                DPuserBorn.Text = User.Дата_рождения?.ToString("dd.MM.yyyy");
                TBuserPhone.Text = User.Номер_телефона;
                TBuserEmail.Text = User.Email;
                TBuserLogin.Text = User.Логин;

                CategoriesList.ItemsSource = db.Категории_товаров.ToList();
                RefreshCost();
            }
            catch
            {
                MessageBox.Show("Ошибка при инициализации");
                return;
            }
        }
        void RefreshCost()
        {
            var allProductsInBasket = db.Товары_в_корзине.ToList().Where(x => x.Клиент == User.Код);
            CartGrid.ItemsSource = allProductsInBasket;
            TBfinalSum.Text = "Общая стоимость корзины: " + TotalSum(allProductsInBasket.ToList()).ToString();
            TIbasket.Header = $"Корзина: ({TotalSum(allProductsInBasket.ToList()).ToString()})";
        }
        public double TotalSum(List<Товары_в_корзине> products)
        {
            double total = 0;

            foreach (var product in products)
            {
                total += (double)product.Стоимость;
            }
            return total;
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

        private void BTNsaveChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = TBuserLogin.Text;
                if (db.Клиенты.Any(u => u.Логин == login && (User.Код == null || u.Код != User.Код)))
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
                    string.IsNullOrWhiteSpace(TBuserStreet.Text) ||
                    string.IsNullOrWhiteSpace(TBuserNumHome.Text) ||
                    string.IsNullOrWhiteSpace(TBuserApartNum.Text)
                    )
                {
                    MessageBox.Show("Необходимо заполнить все поля, (Подъезд и этаж - опциональны)");
                    return;
                }

                if (!Class1.ValidNumHouse(TBuserNumHome.Text))
                {
                    MessageBox.Show("Введите корректный номер дома");
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

                if(TBuserSurname.Text.Length > 50)
                {
                    MessageBox.Show("Слишком длинная фамилия. (макс.50)");
                    return;
                }
                if(TBuserName.Text.Length > 50)
                {
                    MessageBox.Show("Слишком длинное имя. (макс.50)");
                    return;
                }
                if(TBuserPatronymic.Text.Length > 50)
                {
                    MessageBox.Show("Слишком длинное отчество. (макс.50)");
                    return;
                }
                if(TBuserEmail.Text.Length > 40)
                {
                    MessageBox.Show("Слишком длинный Email. (макс. 40)");
                    return;
                }
                
                if(TBuserStreet.Text.Length > 50)
                {
                    MessageBox.Show("Слишком длинная улица. (макс. 50)");
                    return;
                }
                if(TBuserNumHome.Text.Length > 4)
                {
                    MessageBox.Show("Слишком длинный номер дома. (макс. 4)");
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
                
                

                User.Имя = TBuserName.Text;
                User.Фамилия = TBuserSurname.Text;
                User.Отчество = TBuserPatronymic.Text;
                User.Дата_рождения = DPuserBorn.SelectedDate;
                User.Номер_телефона = TBuserPhone.Text;
                User.Email = TBuserEmail.Text;
                User.Логин = TBuserLogin.Text;
                User.Улица = TBuserStreet.Text;
                User.Номер_дома = TBuserNumHome.Text;
                User.Номер_квартиры =TBuserApartNum.Text;
                if(TBuserEntranceNum.Text != null)
                {
                    User.Номер_подъезда = TBuserEntranceNum.Text;
                }
                if(TBuserFloorNum.Text != null)
                {
                    User.Номер_этажа = TBuserFloorNum.Text;
                }

                db.SaveChanges();
                MessageBox.Show("Данные сохранены");

                userName.Text = User.Имя;
                userSurname.Text = User.Фамилия;
                userPatronymic.Text = User.Отчество;
                userBorn.Text = User.Дата_рождения?.ToString("dd.MM.yyyy");
                userPhone.Text = User.Номер_телефона;
                userEmail.Text = User.Email;
                userLogin.Text = User.Логин;
                userStreet.Text = User.Улица;
                userNumHome.Text = User.Номер_дома;
                userApartNum.Text = User.Номер_квартиры;
                userEntranceNum.Text = User.Номер_подъезда;
                userFloorNum.Text = User.Номер_этажа;


                userName.Visibility = Visibility.Visible;
                userSurname.Visibility = Visibility.Visible;
                userPatronymic.Visibility = Visibility.Visible;
                userBorn.Visibility = Visibility.Visible;
                userPhone.Visibility = Visibility.Visible;
                userEmail.Visibility = Visibility.Visible;
                userLogin.Visibility = Visibility.Visible;
                userStreet.Visibility = Visibility.Visible;
                userNumHome.Visibility = Visibility.Visible;
                userApartNum.Visibility = Visibility.Visible;
                userEntranceNum.Visibility = Visibility.Visible;
                userFloorNum.Visibility = Visibility.Visible;

                TBuserName.Visibility = Visibility.Collapsed;
                TBuserSurname.Visibility = Visibility.Collapsed;
                TBuserPatronymic.Visibility = Visibility.Collapsed;
                DPuserBorn.Visibility = Visibility.Collapsed;
                TBuserPhone.Visibility = Visibility.Collapsed;
                TBuserEmail.Visibility = Visibility.Collapsed;
                TBuserLogin.Visibility = Visibility.Collapsed;
                TBuserStreet.Visibility = Visibility.Collapsed;
                TBuserNumHome.Visibility = Visibility.Collapsed;
                TBuserApartNum.Visibility = Visibility.Collapsed;
                TBuserEntranceNum.Visibility = Visibility.Collapsed;
                TBuserFloorNum.Visibility = Visibility.Collapsed;

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
            userStreet.Visibility = Visibility.Collapsed;
            userNumHome.Visibility = Visibility.Collapsed;
            userApartNum.Visibility = Visibility.Collapsed;
            userEntranceNum.Visibility = Visibility.Collapsed;
            userFloorNum.Visibility = Visibility.Collapsed;
            //userLogin.Visibility = Visibility.Collapsed;

            TBuserName.Visibility = Visibility.Visible;
            TBuserSurname.Visibility = Visibility.Visible;
            TBuserPatronymic.Visibility = Visibility.Visible;
            DPuserBorn.Visibility = Visibility.Visible;
            TBuserPhone.Visibility = Visibility.Visible;
            TBuserEmail.Visibility = Visibility.Visible;
            TBuserStreet.Visibility = Visibility.Visible;
            TBuserNumHome.Visibility = Visibility.Visible;
            TBuserApartNum.Visibility = Visibility.Visible;
            TBuserEntranceNum.Visibility = Visibility.Visible;
            TBuserFloorNum.Visibility = Visibility.Visible;
            //TBuserLogin.Visibility = Visibility.Visible;
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

        private void CategoriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoriesList.SelectedItem is Категории_товаров selectedCategory)
            {
                // Фильтруем товары по выбранной категории
                List<Товары> filteredProducts = db.Товары.ToList().FindAll(p => p.Категория == selectedCategory.Код);
                SelectedCategory = selectedCategory;
                ProductsList.ItemsSource = filteredProducts;
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button.DataContext as Товары;

            if (product == null)
            {
                MessageBox.Show("Не удалось определить товар.");
                return;
            }
            if (product.Остаток_товара == 0)
            {
                MessageBox.Show("товара нет в наличии");
                return;
            }

            var existingItem = db.Товары_в_корзине.FirstOrDefault(c => c.Товар == product.Код && c.Клиент == User.Код);

            if (existingItem == null)
            {
                var newProductInBasket = new Товары_в_корзине
                {
                    Товар = product.Код,
                    Клиент = User.Код,
                    Количество = 1,
                    Стоимость = (double)product.Цена * 1,
                };
                db.Товары_в_корзине.Add(newProductInBasket);
                db.SaveChanges();
                MessageBox.Show("Товар добавлен в корзину!");
                RefreshCost();

            }
            else
            {
                MessageBox.Show("Такой товар уже есть в корзине");
                RefreshCost();

            }
        }

        private void TBuserFloorNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, 0)))
            {
                e.Handled = true;
            }
        }

        private void TBuserEntranceNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, 0)))
            {
                e.Handled = true;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image && image.Tag is string url)
            {
                System.Diagnostics.Process.Start(url);
            }
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image && image.Tag is string url)
            {
                System.Diagnostics.Process.Start(url);
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.Tag as Товары_в_корзине;
            if (item.Количество > 1)
            {
                item.Количество--;
                item.Стоимость = ((double)item.Товары.Цена * (double)item.Количество);
                db.SaveChanges();
                RefreshCost();
            }
            else
            {
                MessageBoxResult res = MessageBox.Show("Вы хотите удалить товар из корзины?", "Предупреждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                        );
                if(res == MessageBoxResult.Yes)
                {
                    db.Товары_в_корзине.Remove(item);
                    db.SaveChanges();
                    RefreshCost();
                    MessageBox.Show("Товар удален");
                }
                else { }
            }
        }
        

        private void IncreaseQuantit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.Tag as Товары_в_корзине;
            if(item.Товары.Остаток_товара > item.Количество)
            {
                item.Количество++;
                item.Стоимость = ((double)item.Товары.Цена * (double)item.Количество);
                db.SaveChanges();
                RefreshCost();
            }
            else
            {
                item.Количество = item.Товары.Остаток_товара;
                MessageBox.Show("На складе нет столько товара");
                return;
            }
           

        }

        private void BTNdesignOrder_Click(object sender, RoutedEventArgs e)
        {
            if(CartGrid.Items.Count <= 0)
            {
                MessageBox.Show("Добавьте товар в корзину");
                return;
            }
            if(User.Улица == null)
            {
                MessageBox.Show("Укажите улицу в профиле");
                return;
            }
            if(User.Номер_дома == null)
            {
                MessageBox.Show("Укажите номер дома в профиле");
                return;
            }
            if (User.Номер_квартиры == null)
            {
                MessageBox.Show("Укажите номер квартиры в профиле");
                return;
            }

            var allProductsInBasket = db.Товары_в_корзине.ToList().Where(x => x.Клиент == User.Код);
            CartGrid.ItemsSource = allProductsInBasket;
            double Sum = TotalSum(allProductsInBasket.ToList());

            Order order = new Order(User,Sum);
            if(order.ShowDialog() == true)
            {
                RefreshCost();
                db = new Entities();
                List<Товары> filteredProducts = db.Товары.ToList().FindAll(p => p.Категория == SelectedCategory.Код);
                ProductsList.ItemsSource = filteredProducts;
            }
        }

        private void TBuserApartNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, 0)))
            {
                e.Handled = true;
            }
        }
    }
}

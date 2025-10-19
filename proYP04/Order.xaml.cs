using MailKit.Security;
using Microsoft.Office.Interop.Word;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using WpfWindow = System.Windows.Window;

namespace proYP04
{
    /// <summary>
    /// Логика взаимодействия для Order.xaml
    /// </summary>
    public partial class Order : WpfWindow
    {
        Entities db = new Entities();

        double Sum;
        Клиенты User;
        int Discount;
        double TotalSum;
        double TotalDiscount;
        Товары_в_заказе ProductsInOrder;
        public Order(Клиенты user, double sum)
        {
            InitializeComponent();
            User= user;
            Sum = sum;

            TBName.Text = "Имя: " + User.Имя;
            TBSurname.Text = "Фамилия: " + User.Фамилия;
            TBPatronymic.Text = "Отчество: " + User.Отчество;
            TBadress.Text = "Адрес: " + User.Улица + " " + User.Номер_дома + " " + User.Номер_квартиры;
            TBEntrance.Text = "Подъезд: " + User.Номер_подъезда;
            TBFloor.Text = "Этаж: " + User.Номер_этажа;
            TBName.Text = "Имя: " + User.Имя;
            TBEmail.Text = "Email: " + User.Email;

            if (Sum < 1000)
            {
                TBDiscount.Text = "Скидка: " + "0%";
            }
            if(Sum > 1000 && Sum < 2000)
            {
                Discount = 5;
                TBDiscount.Text = "Скидка: " + Discount + "%"; 
            }

            if (Sum > 2000 && Sum < 5000)
            {
                Discount = 10;
                TBDiscount.Text = "Скидка: " + Discount + "%";
            }

            if (Sum > 5000)
            {
                Discount = 15;
                TBDiscount.Text = "Скидка: " + Discount + "%";
            }

            if(Discount == 0)
            {
                TBSum.Text = "Сумма к оплате: " + Sum;
                TotalSum = Sum;
            }
            if(Discount != 0)
            {
                TotalDiscount = Sum * Discount / 100;
                TBSum.Text = "Сумма к оплате: " + (Sum - TotalDiscount);
                TotalSum = Sum - TotalDiscount;
            }
        }

        private void BTNdesignOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allProductsInBasket = db.Товары_в_корзине.Where(x => x.Клиент == User.Код).ToList();

                var newOrder = new Заказ
                {
                    Клиент = User.Код,
                    Дата = DateTime.Now.Date,
                    Сумма = TotalSum,
                };
                db.Заказ.Add(newOrder);
                db.SaveChanges();


                foreach (var item in allProductsInBasket)
                {
                    var newProductsInOrder = new Товары_в_заказе
                    {
                        Товар = item.Товар,
                        Количество = item.Количество,
                        Стоимость = item.Стоимость,
                        Заказ = newOrder.Код
                    };
                    db.Товары_в_заказе.Add(newProductsInOrder);
                    db.SaveChanges();
                }
                db.SaveChanges();

                //.....................................................................................
                string FIO = User.Фамилия + " " + User.Имя + " " + User.Отчество;
                string Adress = TBadress.Text;
                string Entrance = TBEntrance.Text;
                string Floor = TBFloor.Text;
                string Email = TBEmail.Text;
                string discount = TBDiscount.Text;
                string Sum = TBSum.Text;

                var WordApp = new Word.Application();
                WordApp.Visible = false;
                // делаем диалог выбора папки, в которую будет сохранятся билет
                var path = Environment.CurrentDirectory + @"\новый Заказ.docx";
                var Worddoc = WordApp.Documents.Open(Environment.CurrentDirectory + @"/Заказ.docx");
                ReplaceTextOrInsertTable("{ФИО}", FIO, Worddoc);
                ReplaceTextOrInsertTable("{date}", DateTime.Now.ToString(), Worddoc);
                ReplaceTextOrInsertTable("{Адрес}", Adress, Worddoc);
                ReplaceTextOrInsertTable("{Подъезд}", Entrance, Worddoc);
                ReplaceTextOrInsertTable("{Этаж}", Floor, Worddoc);
                ReplaceTextOrInsertTable("{Товар}", allProductsInBasket, Worddoc);
                ReplaceTextOrInsertTable("{Email}", Email, Worddoc);
                ReplaceTextOrInsertTable("{Скидка}", discount, Worddoc);
                ReplaceTextOrInsertTable("{Сумма}", Sum, Worddoc);
                Worddoc.SaveAs2(path);
                Worddoc.Close();
                WordApp.Quit();
                MessageBox.Show("Чек сохранен!");

                foreach (var item in allProductsInBasket)
                {
                    db.Товары_в_корзине.Remove(item);
                }
                db.SaveChanges();
                MessageBox.Show("Заказ оформлен");


               
                //.................................................................................... 

                var message = new MimeMessage();

                // Отправитель
                message.From.Add(new MailboxAddress("Прокошев ИС-22-2", "prokoshev.anferov@yandex.ru"));
                // Получатель
                message.To.Add(new MailboxAddress("Получатель", $"{User.Email}"));
                // Тема
                message.Subject = "Чек (Прокошев ИС-22-2)";

                // Создаем multipart/mixed для текста + вложений
                var multipart = new MimeKit.Multipart("mixed");

                // Текст письма
                var textPart = new MimeKit.TextPart("plain")
                {
                    Text = "Здравствуйте, вот ваш чек :)"
                };
                multipart.Add(textPart);

                // Вложение
                var attachment = new MimeKit.MimePart("application", "vnd.openxmlformats-officedocument.wordprocessingml.document")
                {
                    Content = new MimeKit.MimeContent(File.OpenRead(path)),
                    ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment),
                    ContentTransferEncoding = MimeKit.ContentEncoding.Base64,
                    FileName = "чек.docx"
                };
                multipart.Add(attachment);

                // Устанавливаем тело сообщения
                message.Body = multipart;

                // Отправляем письмо
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.yandex.ru", 465, SecureSocketOptions.SslOnConnect);
                    client.Authenticate("prokoshev.anferov@yandex.ru", "ybulupgirtxikhjb");
                    client.Send(message);
                    client.Disconnect(true);
                }
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            
        }
        private void ReplaceTextOrInsertTable(string subToReplace, object data, Word.Document wordDoc)
        {
            var range = wordDoc.Content;
            range.Find.ClearFormatting();

            if (range.Find.Execute(subToReplace))
            {
                range.Text = ""; // Очищаем плейсхолдер

                if (data is string text)
                {
                    range.InsertAfter(text);
                }
                else if (data is List<Товары_в_корзине> productList)
                {
                    int rows = productList.Count + 1;
                    int columns = 3;

                    Word.Table table = wordDoc.Tables.Add(range, rows, columns);

                    // Настройка границ
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineWidth = WdLineWidth.wdLineWidth050pt;
                    table.Borders.InsideLineWidth = WdLineWidth.wdLineWidth050pt;

                    // Заголовки
                    table.Rows[1].Cells[1].Range.Text = "Наименование";
                    table.Rows[1].Cells[2].Range.Text = "Количество";
                    table.Rows[1].Cells[3].Range.Text = "Цена";
                    table.Rows[1].Range.Font.Bold = 1;

                    // Данные
                    for (int i = 0; i < productList.Count; i++)
                    {
                        var product = productList[i];
                        table.Rows[i + 2].Cells[1].Range.Text = product.Товары.Наименование;
                        table.Rows[i + 2].Cells[2].Range.Text = product.Количество.ToString();
                        table.Rows[i + 2].Cells[3].Range.Text = product.Стоимость.ToString();
                    }
                }
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
    }
}

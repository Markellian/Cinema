using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
//пара номер 4
namespace Cinema
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string Error = "Ошибка!"; //Заголовок для MessageBox
        List<PosterClass> PostersList = new List<PosterClass>();

        RoleName role = new RoleName(); //для указаная роли для посетителей при регистрации
        Users user;
        public MainWindow()
        {
            InitializeComponent();
            LoadPosters();
        }
        /// <summary>
        /// Загрузка постеров фильмов на главный экран
        /// </summary>
        private void LoadPosters()
        {
            PostersList = new List<PosterClass>();
            using (var db = new CinemaEntities())
            {
                int i = -1;
                foreach (var v in from f in db.Films select f)
                {
                    
                        i++;
                        Image image = new Image()
                        {
                            Width = 150,
                            Height = 200,
                            Source = byteArrayToImage(v.Poster),
                            Margin = new Thickness(0, 0, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Stretch = Stretch.Fill
                        };

                        TextBlock textBlock = new TextBlock()
                        {
                            Text = v.Film_name,
                            FontSize = 20,
                            Width = 150,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(0, 200, 0, 0),
                            TextWrapping = TextWrapping.Wrap,
                            TextAlignment = TextAlignment.Center
                        };

                        Grid grid = new Grid()
                        {
                            Width = 150,
                            Height = 275,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(50 + 250 * (i % 3), 300 * (i / 3), 0, 0)                            
                        };
                        grid.MouseMove += Poster_MouseMove;
                        grid.MouseLeave += Poster_MouseLeave;
                        grid.MouseLeftButtonUp += Poster_MouseLeftButtonUp;

                        grid.Children.Add(image);
                        grid.Children.Add(textBlock);
                        PosterGrid.Children.Add(grid);
                        //PosterGrid.Children.Add(image);
                        //PosterGrid.Children.Add(textBlock);
                        PosterClass pp = new PosterClass();
                        pp.film = v;
                        pp.Poster = grid;
                        PostersList.Add(pp);
                    
                }
            }            
        }
        /// <summary>
        /// На постер нажали правой кнопкой мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Poster_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PosterClass poster = new PosterClass();
            Films film = new Films();
            foreach(var v in PostersList)
            {
                if (v.Poster == sender)
                {
                    film = v.film;
                    break;
                }
            }
            PosterScrollViewer.Visibility = Visibility.Hidden;
            FilmInformationScrollViewer.Visibility = Visibility.Visible;
            FilmInformationPosterImage.Source = byteArrayToImage(film.Poster);
            FilmInformationNameTextBlock.Text = film.Film_name;
            FilmInformationDurationLabel.Content = film.Duration;
            FilmInformationDescriptionTextBlock.Text = film.Description;

            using (var db = new CinemaEntities())
            {
                var s = from f in db.Sessions where f.Film == film.Film_id select new {
                    Time = f.Session_time.Hour.ToString() +":"+ f.Session_time.Minute.ToString(),
                    Price = f.Price
                };
                FilmInformationSessionDataGrid.ItemsSource = s.ToList();
            }
        }
        /// <summary>
        /// Возвращение основных цветов при покидании курсором постера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Poster_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = (Grid)sender;
            grid.Background = Brushes.White;
        }
        /// <summary>
        /// Выделение цветом постера, на который направлен курсор
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Poster_MouseMove(object sender, MouseEventArgs e)
        {
            Grid grid = (Grid)sender;
            grid.Background = Brushes.LightGray;
        }

        /// <summary>
        /// Регистрация нового посетителя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrationUserButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginRegistrationTextBox.Text.ToString(); //для более легкого обращения к введеному логину
            string password = PasswordRegistrationPasswordBox.Password.ToString();//для более легкого обращения к введенному паролю
            if (login == "") MessageBox.Show("Введите логин", Error);
            else if (password == "") MessageBox.Show("Введите пароль", Error);
            else if (password.Length < 6) MessageBox.Show("Пароль не должен быть короче 6 символов", Error);
            else if (password != PasswordRegistration2PasswordBox.Password.ToString()) MessageBox.Show("Пароли не сопадают", Error);
            else
            {
                using (var db = new CinemaEntities())
                {
                    bool enter = false; //существует ли логин в БД
                    foreach (var log in from f in db.Users select f)
                    {
                        if (log.Login.ToString() == login)
                        {
                            enter = true;
                            MessageBox.Show("Такой логин уже существует", Error);
                            break;
                        }
                    }
                    if (!enter)
                    {
                        user = new Users
                        {
                            Login = login,
                            Password = password,
                            Role = role.Visiter
                        };
                        if (LastNameRegistrationTextBox.Text.ToString() == "") user.Last_name = null;
                        else user.Last_name = LastNameRegistrationTextBox.Text.ToString();
                        if (FirstNameRegistrationTextBox.Text.ToString() == "") user.First_name = null;
                        else user.First_name = FirstNameRegistrationTextBox.Text.ToString();
                        db.Users.Add(user);
                        db.SaveChanges();
                        MessageBox.Show("Готово");
                    }
                }
            }
        }
        /// <summary>
        /// из изображения в строку байтов
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Из строки байтов в изображение
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        public BitmapImage byteArrayToImage(byte[] byteArrayIn)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(byteArrayIn);
            bitmap.EndInit();
            return bitmap;
        }
        
        private void GoToAuthButton_Click(object sender, RoutedEventArgs e)
        {
            //PosterScrollViewer.Visibility = Visibility.Hidden;
        }
    } 
}

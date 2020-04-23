using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
//using Image = System.Windows.Controls.Image;
//пара номер 4
namespace Cinema
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string Error = "Ошибка!"; //Заголовок для MessageBox
        private List<PosterClass> PostersList = new List<PosterClass>();
        readonly RoleName role = new RoleName(); //для указаная роли для посетителей при регистрации
        private WorkWithImage ImageWork = new WorkWithImage();
        private Users user;
        private Films film;
        private Cinemas Cinema;
        private List<GetPlaces_Result> ListPlacesToBuy = new List<GetPlaces_Result>();//места, выбранные для покупки
        private List<GetFreeHalls_Result> ListHalls;
        private List<Sessions> ListSessions;
        private decimal price = 0;//цена 1 билета
        private decimal AmountTickets = 0;//цена всех выбранных билетов
        private List<Place> listPlaces;//места на сеансе
        private readonly string reg2 = @"\d\d";
        private readonly string reg1 = @"\d";
        private DateTime dateTime;
        string posterWay;
        private Grid lastGrid; //чтобы возвразаться на последний грид
        public MainWindow()
        {
            InitializeComponent();
            LoadPosters();
            using (var db = new CinemaEntities())
            {
                var l = from f in db.Films where f.Film_id == 1 select f;
                foreach (var t in l) film = t;
                var c = from f in db.Cinemas where f.Cinema_id == 1 select f;
                foreach (var c1 in c) Cinema = c1;
            }
            LoadSessions();
        }
        
        /// <summary>
        /// Загрузка постеров фильмов на главный экран
        /// </summary>
        private void LoadPosters()
        {
            foreach (var p in PostersList)
            {
                PosterGrid.Children.Remove(p.Poster);
            }
            PostersList = new List<PosterClass>();
            using (var db = new CinemaEntities())
            {
                int i = -1;
                var q = from f in db.Films select f;
                foreach (var v in q)
                {

                    i++;
                    System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                    {
                        Width = 150,
                        Height = 200,
                        Source = ImageWork.ByteArrayToImage(v.Poster),
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
                    PosterClass pp = new PosterClass
                    {
                        film = v,
                        Poster = grid
                    };
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
            film = new Films();
            foreach (var v in PostersList)
            {
                if (v.Poster == sender)
                {
                    film = v.film;
                    break;
                }
            }
            ListPosterGrid.Visibility = Visibility.Hidden;
            FilmInformationGrid.Visibility = Visibility.Visible;          
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
                        MessageBox.Show("Пользователь зарегистрирован.");

                        lastGrid.Visibility = Visibility.Visible;
                        RegistrationGrid.Visibility = Visibility.Hidden;
                        ChangeVisibilityUserButtons();

                    }
                }
            }
        }
        
        /// <summary>
        /// переход на страницу авторизации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToAuthButton_Click(object sender, RoutedEventArgs e)
        {
            lastGrid = ((Grid)((Button)sender).Parent);
            lastGrid.Visibility = Visibility.Hidden;
            AuthGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Переход на страницу выбора мест
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToChoosePlacesGrid(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Session session = (Session)button.DataContext;
            lastGrid = PlacesGrid;
            PlacesGrid.Visibility = Visibility.Visible;
            FilmInformationGrid.Visibility = Visibility.Hidden;
            using (var db = new CinemaEntities())
            {
                var ses = from f in db.Sessions where f.Session_id == session.sessionId select f;
                Sessions sessions = new Sessions();
                foreach (var k in ses)
                {
                    sessions = k;
                    price = k.Price;
                }
                AmountLabel.Content = "0";
                AmountTickets = 0;
                ListChoosedPlacesTextBlock.Text = "";
                switch (sessions.Halls.Type_hall)
                {

                    //одинаковые типы залы имеют одинаковую схему зала.
                    case 1:
                        {
                            listPlaces = new List<Place>();
                            foreach (var p in db.GetPlaces(session.sessionId))
                            {

                                Place place = new Place();
                                listPlaces.Add(place);
                                place.placeInfo = p;
                                place.label = new Label()
                                {
                                    Width = 25,
                                    Height = 25,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Foreground = Brushes.White,
                                    Content = place.placeInfo.Place_number,
                                    HorizontalContentAlignment = HorizontalAlignment.Center,
                                    VerticalContentAlignment = VerticalAlignment.Center,
                                    Margin = new Thickness(50 + place.placeInfo.Place_number * 35, 325 - 35 * place.placeInfo.Row_number, 0, 0)
                                };
                                if (place.placeInfo.Ticket == null) place.label.Background = Brushes.Green;
                                else place.label.Background = Brushes.Red;
                                place.label.MouseLeftButtonUp += ChooseOnePlace;
                                PlacesGrid.Children.Add(place.label);
                            }
                            Label label = new Label()
                            {
                                Width = 25 * 8 + 70,
                                Height = 25,
                                Background = Brushes.LightGray,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                Content = "Экран",
                                Foreground = Brushes.Black,
                                Margin = new Thickness(85, 45 + 8 * 35, 0, 0)
                            };
                            PlacesGrid.Children.Add(label);
                            break;
                        }

                        
                }
            }

        }
        
        /// <summary>
        /// Пользователь нажал на метку 1 места
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseOnePlace(object sender, MouseButtonEventArgs e)
        {
            GetPlaces_Result place = new GetPlaces_Result();
            foreach (var l in listPlaces)
            {
                if (l.label == sender)
                {
                    place = l.placeInfo;
                    break;
                }
            }
            if (((Label)sender).Background != Brushes.Red)
            {
                if (((Label)sender).Background == Brushes.Orange)
                {
                    ListPlacesToBuy.Remove(place);
                    ((Label)sender).Background = Brushes.Green;
                    AmountTickets -= price;
                    ListChoosedPlacesTextBlock.Text = "";
                    foreach (var l in ListPlacesToBuy)
                    {
                        ListChoosedPlacesTextBlock.Text += "Ряд: " + l.Row_number + " Место: " + l.Place_number + "\n";
                    }
                }
                else
                {
                    ListPlacesToBuy.Add(place);
                    ((Label)sender).Background = Brushes.Orange;
                    AmountTickets += price;
                    ListChoosedPlacesTextBlock.Text += "Ряд: " + place.Row_number + " Место: " + place.Place_number + "\n";
                }
            }
            AmountLabel.Content = AmountTickets;
        }

        /// <summary>
        /// Вернуться из страницы выбора к просмотру сеансов фильма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToFilmInformationFromChoosePlaces(object sender, RoutedEventArgs e)
        {
            PlacesGrid.Visibility = Visibility.Hidden;
            FilmInformationGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Вернуться из страницы просмотра сеансов к списку фильмов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToPosterGrid(object sender, RoutedEventArgs e)
        {
            FilmInformationGrid.Visibility = Visibility.Hidden;
            ListPosterGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Нажата кнопка Оплатить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToPayTickets(object sender, RoutedEventArgs e)
        {
            if (user == null)
            {
                AuthGrid.Visibility = Visibility.Visible;
                PlacesGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                if (ListPlacesToBuy == null)
                {
                    MessageBox.Show("Выберите места для покупки", "Внимание!");
                }
                else
                {
                    PlacesGrid.Visibility = Visibility.Hidden;
                    PayGrid.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// В окне авторизации нажата кнопка Назад
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackFromAuth(object sender, RoutedEventArgs e)
        {
            lastGrid.Visibility = Visibility.Visible;
            AuthGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Перейти к окну регистрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToRegistration(object sender, RoutedEventArgs e)
        {
            RegistrationGrid.Visibility = Visibility.Visible;
            AuthGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Авторизироваться
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            string log = AuthLoginTextBox.Text;
            string pas = AuthPasswordPasswordBox.Password;
            if (log == "") MessageBox.Show("Введите логин", Error);
            else if (pas == "") MessageBox.Show("Введите пароль", Error);
            else
            {
                using (var db = new CinemaEntities())
                {
                    foreach (var u in from f in db.Users where f.Login == log || f.Password == pas select f)
                    {
                        user = u;
                    }
                    if (user == null) MessageBox.Show("Такого пользователя не существует.", Error);
                    else
                    {
                        lastGrid.Visibility = Visibility.Visible;
                        AuthGrid.Visibility = Visibility.Hidden;
                        ChangeVisibilityUserButtons();
                    }
                }
            }
        }

        /// <summary>
        /// Когда видимость окна авторизации изменилась 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AuthGrid.Visibility == Visibility.Visible)
            {
                AuthLoginTextBox.Text = "";
                AuthPasswordPasswordBox.Password = "";
            }
        }

        /// <summary>
        /// Вернуться с окна регистрации к авторизации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToAuth(object sender, RoutedEventArgs e)
        {
            RegistrationGrid.Visibility = Visibility.Hidden;
            AuthGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Перейти к окну пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            UserGrid.Visibility = Visibility.Visible;
            lastGrid = ((Grid)((Button)sender).Parent);
            lastGrid.Visibility = Visibility.Hidden;
            using (var db = new CinemaEntities())
            {
                var p = db.GetPursheDetails(user.Login);
                PursheDetailsDataGrid.ItemsSource = p.ToList();
            }
        }

        /// <summary>
        /// Вернуться со страницы пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoOutFromUserGrid(object sender, RoutedEventArgs e)
        {
            UserGrid.Visibility = Visibility.Hidden;
            lastGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Выйти из профиля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitFromUser(object sender, RoutedEventArgs e)
        {
            user = null;
            GoOutFromUserGrid(null, null);
            ChangeVisibilityUserButtons();
            ChangeVisibilityAdminButtons(Visibility.Hidden);
        }

        private void RegistrationGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (RegistrationGrid.Visibility == Visibility.Visible)
            {
                LoginRegistrationTextBox.Text = "";
                PasswordRegistration2PasswordBox.Password = "";
                PasswordRegistrationPasswordBox.Password = "";
                FirstNameRegistrationTextBox.Text = "";
                LastNameRegistrationTextBox.Text = "";
            }
        }

        /// <summary>
        /// Вернуться из окна оплаты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromPayToMenu(object sender, RoutedEventArgs e)
        {
            PayGrid.Visibility = Visibility.Hidden;
            lastGrid.Visibility = Visibility.Visible;
        }

        private void PayGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PayGrid.Visibility == Visibility.Visible)
            {
                PayNumberCardTextBox1.Text = "";
                PayNumberCardTextBox2.Text = "";
                PayNumberCardTextBox3.Text = "";
                PayNumberCardTextBox4.Text = "";
                PayMonthCardTextBox.Text = "";
                PayYearCardTextBox.Text = "";
                PayUserCardTextBox.Text = "";
                PayCVC2CardTextBox.Text = "";
            }
        }

        /// <summary>
        /// Оплатить билеты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PayTickets(object sender, RoutedEventArgs e)
        {
            string regNum = @"\d\d\d\d";
            string regDate = @"\d\d";

            if (!Regex.IsMatch(PayNumberCardTextBox1.Text, regNum) ||
                !Regex.IsMatch(PayNumberCardTextBox2.Text, regNum) ||
                !Regex.IsMatch(PayNumberCardTextBox3.Text, regNum) ||
                !Regex.IsMatch(PayNumberCardTextBox4.Text, regNum)) MessageBox.Show("Неверный номер карты", Error);
            else if (!Regex.IsMatch(PayMonthCardTextBox.Text, regDate) ||
                    !Regex.IsMatch(PayYearCardTextBox.Text, regDate) ||
                    int.Parse(PayMonthCardTextBox.Text) > 12 ||
                    int.Parse(PayMonthCardTextBox.Text) < 1) MessageBox.Show("Неверная дата", Error);
            else if (PayUserCardTextBox.Text == "") MessageBox.Show("Укажите владельца карты", Error);
            else if (!Regex.IsMatch(PayCVC2CardTextBox.Text, @"\d\d\d")) MessageBox.Show("Неверный код CVC2", Error);
            else
            {
                using (var db = new CinemaEntities())
                {
                    Purchases purchases = new Purchases()
                    {
                        Buyer = user.Login,
                        Purchase_date = DateTime.Now
                    };
                    db.Purchases.Add(purchases);
                    foreach (var t in ListPlacesToBuy)
                    {
                        var o = from f in db.Tickets where f.Ticket_id == t.Ticket_id select f;
                        foreach (var p in o)
                        {
                            purchases.Tickets.Add(p);
                        }
                    }
                    db.SaveChanges();
                }
                MessageBox.Show("Билеты куплены");
                PayGrid.Visibility = Visibility.Hidden;
                ListPosterGrid.Visibility = Visibility.Visible;
            }
        }

        private void PayNumberCardTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox == PayNumberCardTextBox1)
            {
                if (textBox.Text.Length == 4) PayNumberCardTextBox2.Focus();
            }
            else if (textBox == PayNumberCardTextBox2)
            {
                if (textBox.Text.Length == 4) PayNumberCardTextBox3.Focus();
            }
            else if (textBox == PayNumberCardTextBox3)
            {
                if (textBox.Text.Length == 4) PayNumberCardTextBox4.Focus();
            }
        }

        /// <summary>
        /// Перейти к окну добавления фильма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromMenuToAddFilm(object sender, RoutedEventArgs e)
        {
            PosterGrid.Visibility = Visibility.Hidden;
            FilmAddGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Выйти от создания фильма к списку фильмов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromAddFilmToManu(object sender, RoutedEventArgs e)
        {
            PosterGrid.Visibility = Visibility.Visible;
            FilmAddGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Добавить постер к новому фильму
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPosterNewFilm(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                posterWay = openFileDialog.FileName;
                string regex = @"\.[(png)(jpg)]";
                if (Regex.IsMatch(openFileDialog.FileName, regex))
                {
                    ImageBrush image = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Relative))
                    };
                    NewFilmPosterLabel.Background = image;
                }
                else MessageBox.Show("Неверный файл изображения", Error);

            }
        }
        
        /// <summary>
        /// Добавить новый фильм
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewFilm(object sender, RoutedEventArgs e)
        {
            
            if (NewFilmNameTextBox.Text == "") MessageBox.Show("Неверное название фильма", Error);
            else if (NewFilmDateReleaseDatePicker.SelectedDate == null) MessageBox.Show("Укажите дату релиза", Error);
            else if (NewFilmDateEndDatePicker.SelectedDate != null &&
                     NewFilmDateReleaseDatePicker.SelectedDate > NewFilmDateEndDatePicker.SelectedDate) MessageBox.Show("Прокат фильма не может закончиться раньше, чем он начнется", Error);
            else if (!(Regex.IsMatch(NewFilmDurationHourTextBox.Text, reg2) || Regex.IsMatch(NewFilmDurationHourTextBox.Text, reg1)) ||
                     !(Regex.IsMatch(NewFilmDurationMinuteTextBox.Text, reg2) || Regex.IsMatch(NewFilmDurationMinuteTextBox.Text, reg1)) ||
                     int.Parse(NewFilmDurationMinuteTextBox.Text) > 59 ||
                     (int.Parse(NewFilmDurationMinuteTextBox.Text) == 0 &&
                      int.Parse(NewFilmDurationHourTextBox.Text) == 0)) MessageBox.Show("Неверная длительность фильма", Error);
            else if (NewFilmPosterLabel.Background == null) MessageBox.Show("Ваберите постер", Error);
            else
            {
                Films film = new Films()
                {
                    Film_name = NewFilmNameTextBox.Text,
                    Date_release = (DateTime)NewFilmDateReleaseDatePicker.SelectedDate,
                    Date_end = NewFilmDateEndDatePicker.SelectedDate,
                    Duration = new TimeSpan(int.Parse(NewFilmDurationHourTextBox.Text), int.Parse(NewFilmDurationMinuteTextBox.Text), 0),
                    Description = NewFilmDescriptionTextBox.Text,
                    Poster = ImageWork.ImageToByteArray(System.Drawing.Image.FromFile(posterWay))
                };
                using (var db = new CinemaEntities())
                {
                    db.Films.Add(film);
                    db.SaveChanges();
                    MessageBox.Show("Фильм добавлен");
                }
                LoadPosters();
            }
            FilmAddGrid.Visibility = Visibility.Hidden;
            PosterGrid.Visibility = Visibility.Visible;
        }
      
        private void FilmAddGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            NewFilmNameTextBox.Text = "";
            NewFilmDateReleaseDatePicker.SelectedDate = null;
            NewFilmDateEndDatePicker.SelectedDate = null;
            NewFilmDurationHourTextBox.Text = "";
            NewFilmDurationMinuteTextBox.Text = "";
            NewFilmDescriptionTextBox.Text = "";
            NewFilmPosterLabel.Background = null;
        }

        /// <summary>
        /// Перейти к окну просмаотра пользователей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToUsersRoleGrid(object sender, RoutedEventArgs e)
        {
            lastGrid = (Grid)((Button)sender).Parent;
            lastGrid.Visibility = Visibility.Hidden;
            UsersRoleGrid.Visibility = Visibility.Visible;
            LoadUsersRole();
        }

        /// <summary>
        /// Выйти из окна просмотра пользователей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoBackFromUsersRoleGrid(object sender, RoutedEventArgs e)
        {
            lastGrid.Visibility = Visibility.Visible;
            UsersRoleGrid.Visibility = Visibility.Hidden;
            if (user.Role != role.Admin) ChangeVisibilityAdminButtons(Visibility.Hidden);
        }

        /// <summary>
        /// Нажата одна из 2х кнопок изменения прав пользователя 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeUserRole(object sender, RoutedEventArgs e)
        {
            if ((Button)sender == ChangeRoleOnAdminButton) ChangeUserRole(role.Admin);
            else ChangeUserRole(role.Visiter);
            LoadUsersRole();
        }

        /// <summary>
        /// изменение роли
        /// </summary>
        /// <param name="role"></param>
        private void ChangeUserRole(string role)
        {
            UserRole userRole = new UserRole();
            var o = UsersRoleDataGrid.SelectedCells;
            foreach (var l in o)
            {
                userRole = (UserRole)l.Item;
            }
            using (var db = new CinemaEntities())
            {
                var u = from f in db.Users where f.Login == userRole.Login select f;
                foreach (var u2 in u)
                {
                    u2.Role = role;
                    if (user.Login == u2.Login) user.Role = role;
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Изменить видимость пользовательских кнопок
        /// </summary>
        void ChangeVisibilityUserButtons()
        {
            Visibility visibility1;
            Visibility visibility2;
            if (user != null)
            {
                visibility1 = Visibility.Visible;
                visibility2 = Visibility.Hidden;
                UserButton.Content = user.Last_name + " " + user.First_name;
                UserButton2.Content = UserButton.Content;
                if (user.Role == role.Admin)
                {
                    ChangeVisibilityAdminButtons(Visibility.Visible);
                }
            }
            else
            {
                visibility1 = Visibility.Hidden;
                visibility2 = Visibility.Visible;
                ChangeVisibilityAdminButtons(Visibility.Hidden);
            }
            UserButton.Visibility = visibility1;
            UserButton2.Visibility = visibility1;
            AuthUserButton.Visibility = visibility2;
            AuthUserButton2.Visibility = visibility2;
        }
        
        /// <summary>
        /// Изменить видимость кнопок администратора
        /// </summary>
        /// <param name="visibility"></param>
        private void ChangeVisibilityAdminButtons(Visibility visibility)
        {
            AddNewFilmButton.Visibility = visibility;
            UsersRoleButton.Visibility = visibility;
            SessionSetupButton.Visibility = visibility;
        }
        
        /// <summary>
        /// Вывести всех пользователей и их права
        /// </summary>
        private void LoadUsersRole()
        {
            using (var db = new CinemaEntities())
            {
                var k = from f in db.Users select new UserRole { Login = f.Login, Role = f.Role };
                UsersRoleDataGrid.ItemsSource = k.ToList();
            }
        }

        /// <summary>
        /// Перейти к настройке сеансов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SessionSetupButton_Click(object sender, RoutedEventArgs e)
        {
            FilmInformationGrid.Visibility = Visibility.Hidden;
            SessionSetupGrid.Visibility = Visibility.Visible;
        }
        
        /// <summary>
        /// вывести сессии в окне настроек сессий
        /// </summary>
        private void LoadSessions()
        {
            ListSessions = new List<Sessions>();
            using (var db = new CinemaEntities())
            {
                var s = from f in db.Sessions where f.Film == film.Film_id select f;
                foreach (var ses in s)
                {
                    ListSessions.Add(ses);
                }                
            }
            SessionsDataGrid.ItemsSource = ListSessions;
        }

        /// <summary>
        /// выйти с настроек сеанса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoBackFromSessionSetup(object sender, RoutedEventArgs e)
        {
            FilmInformationGrid.Visibility = Visibility.Visible;
            SessionSetupGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Перейти к экрану 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToAddSessionGrid(object sender, RoutedEventArgs e)
        {
            AddSessionGrid.Visibility = Visibility.Visible;
            SessionSetupGrid.Visibility = Visibility.Hidden;
            LoadSessions();
        }

        /// <summary>
        /// Покинуть окно добавления сеанса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoOutFromAddSessionGrid(object sender, RoutedEventArgs e)
        {
            AddSessionGrid.Visibility = Visibility.Hidden;
            SessionSetupGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Добавить сессию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSessionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal r = decimal.Parse(AddSessionPriceTextBox.Text);
                int hall = int.Parse(AddSessionHallComboBox.Text);
                foreach (var h in ListHalls) { if (h.Hall_number == hall) hall = h.Hall_id; break; }
                Sessions sessions = new Sessions()
                {
                    Film = film.Film_id,
                    Hall = hall,
                    Price = r,
                    Session_time = dateTime
                };

                ListSessions.Add(sessions);
                SessionsDataGrid.ItemsSource = null;
                SessionsDataGrid.ItemsSource = ListSessions;
                using (var db = new CinemaEntities())
                {
                    db.Sessions.Add(sessions);
                    db.SaveChanges();
                    var places = from f in db.Places where f.Hall == hall select f;
                    foreach(var place in places)
                    {
                        Tickets tickets = new Tickets()
                        {
                            Sessions = sessions,
                            Places = place
                        };
                        db.Tickets.Add(tickets);
                    }
                    db.SaveChanges();
                }
                MessageBox.Show("Сеанс добавлен");
                AddSessionGrid.Visibility = Visibility.Hidden;
                SessionSetupGrid.Visibility = Visibility.Visible;

            }
            catch (FormatException)
            {
                MessageBox.Show("Неверная цена", Error);
            }

        }

        private void AddSessionDataPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddSessionDataPicker.SelectedDate != null)
            {
                AddSessionHourTextBox.IsEnabled = true;
                AddSessionMinuteTextBox.IsEnabled = true;
                AddSessionSeeFreeHallsButton.IsEnabled = true;
            }
            else
            {
                AddSessionHourTextBox.IsEnabled = false;
                AddSessionMinuteTextBox.IsEnabled = false;
                AddSessionSeeFreeHallsButton.IsEnabled = false;
                AddSessionHallComboBox.IsEnabled = false;
                AddSessionButton.IsEnabled = false;
            }
        }

        private void AddSessionSeeFreeHallsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Regex.IsMatch(AddSessionHourTextBox.Text, reg2) || Regex.IsMatch(AddSessionHourTextBox.Text, reg1)) ||
                     !(Regex.IsMatch(AddSessionMinuteTextBox.Text, reg2) || Regex.IsMatch(AddSessionMinuteTextBox.Text, reg1)) ||
                     int.Parse(AddSessionMinuteTextBox.Text) > 59 ||
                     int.Parse(AddSessionHourTextBox.Text) >= 24) MessageBox.Show("Неверное время", Error);
            else
            {
                AddSessionHallComboBox.IsEnabled = true;
                dateTime = (DateTime)AddSessionDataPicker.SelectedDate;
                dateTime = dateTime.AddHours(double.Parse(AddSessionHourTextBox.Text));
                dateTime = dateTime.AddMinutes(double.Parse(AddSessionMinuteTextBox.Text));
                List<int> halls = new List<int>();
                using (var db = new CinemaEntities())
                {
                    ListHalls = db.GetFreeHalls(Cinema.Cinema_id, film.Film_id, dateTime).ToList();
                    foreach (var h in ListHalls) halls.Add(h.Hall_number);
                    AddSessionHallComboBox.ItemsSource = halls;
                }
            }
        }

        private void AddSessionHallComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddSessionButton.IsEnabled = true;
        }

        private void AddSessionHourTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddSessionButton.IsEnabled = false;
            AddSessionHallComboBox.IsEnabled = false;
            AddSessionHallComboBox.ItemsSource = null;
        }

        private void AddSessionGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AddSessionGrid.Visibility == Visibility.Visible)
            {
                AddSessionFilmNameLabel.Content = film.Film_name;
                AddSessionDataPicker.SelectedDate = null;
                AddSessionHourTextBox.Text = "";
                AddSessionMinuteTextBox.Text = "";
                AddSessionPriceTextBox.Text = "";
                AddSessionHallComboBox.ItemsSource = null;
                AddSessionDataPicker_SelectedDateChanged(null, null);
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? date = ((DatePicker)sender).SelectedDate;
            if (date == null) FilmInformationSessionDataGrid.ItemsSource = null;
            else
            {
                using (var db = new CinemaEntities())
                {
                    var s = from f in db.Sessions
                            where f.Film == film.Film_id && f.Session_time.Year == ((DateTime)date).Year && f.Session_time.Month == ((DateTime)date).Month && f.Session_time.Day == ((DateTime)date).Day
                            select new Session
                            {
                                Time = f.Session_time.Hour.ToString() + ":" + f.Session_time.Minute.ToString(),
                                Price = f.Price,
                                Hall = f.Halls.Hall_number,
                                sessionId = f.Session_id
                            };
                    FilmInformationSessionDataGrid.ItemsSource = s.ToList();
                }
            }
        }

        private void SessionSetupGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (SessionsDataGrid.Visibility == Visibility.Visible) LoadSessions();
        }

        private void FilmInformationGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            FilmInformationPosterImage.Source = ImageWork.ByteArrayToImage(film.Poster);
            FilmInformationNameTextBlock.Text = film.Film_name;
            FilmInformationDurationLabel.Content = film.Duration;
            FilmInformationDescriptionTextBlock.Text = film.Description;
        }

        /// <summary>
        /// Удалить сеанс
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSessionButton_Click(object sender, RoutedEventArgs e)
        {
            Sessions ses = (Sessions)SessionsDataGrid.SelectedValue;
            
            using(var db = new CinemaEntities())
            {
                if ((db.IsTicketsOnSessionBought(ses.Session_id)).Count() != 0) MessageBox.Show("Нельзя удалить сеанс, когда на него уже куплены билеты");
                else
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Удалить выбранные сеанс?", "Внимание!", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        var t = from f in db.Tickets where f.Session == ses.Session_id select f;
                        foreach(var ticket in t)
                        {
                            db.Tickets.Remove(ticket);
                        }
                        var s = from f in db.Sessions where f.Session_id == ses.Session_id select f;
                        Sessions sessions = new Sessions();
                        foreach(var s1 in s)
                        {
                            sessions = s1;
                        }
                        db.Sessions.Remove(sessions);
                        db.SaveChanges();
                        MessageBox.Show("Сеанс удален");
                        LoadSessions();
                    }
                }
            }
        }
    }

}

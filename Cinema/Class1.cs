using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cinema
{
    public class PosterClass
    {
        public Films film;
        public Grid Poster;
    }
    public class RoleName//Соответствует данным таблицы Roles
    {
        public string Admin = "Админ";
        public string Visiter = "Посетитель";
    }
    public class Session
    {
        public string Time { get; set; }
        public decimal? Price { get; set; }
        public int Hall { get; set; }
        public int sessionId;
    }
    public class Place
    {
        public GetPlaces_Result placeInfo;
        public Label label;
    }

    public class UserRole
    {
        public string Login { get; set; }
        public string Role { get; set; }
    }
}

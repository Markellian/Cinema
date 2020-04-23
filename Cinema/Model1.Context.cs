﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cinema
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class CinemaEntities : DbContext
    {
        public CinemaEntities()
            : base("name=CinemaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Cinemas> Cinemas { get; set; }
        public virtual DbSet<Films> Films { get; set; }
        public virtual DbSet<Halls> Halls { get; set; }
        public virtual DbSet<Places> Places { get; set; }
        public virtual DbSet<Purchases> Purchases { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Sessions> Sessions { get; set; }
        public virtual DbSet<Tickets> Tickets { get; set; }
        public virtual DbSet<Type_hall> Type_hall { get; set; }
        public virtual DbSet<Type_place> Type_place { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    
        public virtual ObjectResult<GetFreeHalls_Result> GetFreeHalls(Nullable<int> cinema, Nullable<int> film, Nullable<System.DateTime> datetime)
        {
            var cinemaParameter = cinema.HasValue ?
                new ObjectParameter("cinema", cinema) :
                new ObjectParameter("cinema", typeof(int));
    
            var filmParameter = film.HasValue ?
                new ObjectParameter("film", film) :
                new ObjectParameter("film", typeof(int));
    
            var datetimeParameter = datetime.HasValue ?
                new ObjectParameter("datetime", datetime) :
                new ObjectParameter("datetime", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetFreeHalls_Result>("GetFreeHalls", cinemaParameter, filmParameter, datetimeParameter);
        }
    
        public virtual ObjectResult<GetPlaces_Result> GetPlaces(Nullable<int> s)
        {
            var sParameter = s.HasValue ?
                new ObjectParameter("s", s) :
                new ObjectParameter("s", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPlaces_Result>("GetPlaces", sParameter);
        }
    
        public virtual ObjectResult<GetPursheDetails_Result> GetPursheDetails(string user)
        {
            var userParameter = user != null ?
                new ObjectParameter("user", user) :
                new ObjectParameter("user", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPursheDetails_Result>("GetPursheDetails", userParameter);
        }
    }
}

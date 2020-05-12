use master
If Exists(Select * From sysdatabases Where name = 'Cinema')
	Drop DataBase Cinema

GO
Create DataBase Cinema
	On Primary (NAME= Cinemadata, FILENAME= 'D:\SQL\MSSQL14.SQLEXPRESS\MSSQL\DATA\Cinemadata.mdf', SIZE=3 MB, MAXSIZE=100, FILEGROWTH=10)
	LOG ON (NAME= Cinemalog, FILENAME='D:\SQL\MSSQL14.SQLEXPRESS\MSSQL\DATA\Cinemalog.ldf', SIZE=3MB, MAXSIZE=100, FILEGROWTH=10)

GO
Use Cinema
If Exists(Select * From sysobjects Where Name ='Purchase_details') Drop Table Purchase_details
If Exists(Select * From sysobjects Where Name ='Purchase') Drop Table Purchases
If Exists(Select * From sysobjects Where Name ='Users') Drop Table Users
If Exists(Select * From sysobjects Where Name ='Tickets') Drop Table Tickets
If Exists(Select * From sysobjects Where Name ='Sessions') Drop Table [Sessions]
If Exists(Select * From sysobjects Where Name ='Places') Drop Table Places
If Exists(Select * From sysobjects Where Name ='Halls') Drop Table Halls
If Exists(Select * From sysobjects Where Name ='Type_place') Drop Table Type_place
If Exists(Select * From sysobjects Where Name ='Roles') Drop Table Roles
If Exists(Select * From sysobjects Where Name ='Cinemas') Drop Table Cinemas
If Exists(Select * From sysobjects Where Name ='Type_hall') Drop Table Type_hall
If Exists(Select * From sysobjects Where Name ='Films') Drop Table Films



Create Table Roles
	(Role_name NVARCHAR(10) Primary Key)
		
Create Table Cinemas
	(Cinema_id INT Primary Key Identity,
	 [Address] NVARCHAR(125) NOT NULL)

Create Table Type_hall
	(Type_hall_id INT Primary Key Identity,
	 Type_hall_name nvarchar(50) NOT NULL,
	 Number_seats int NOT NULL)

Create Table Films
	(Film_id INT Primary Key Identity,
	 Film_name NVARCHAR(50) NOT NULL,	 
	 Date_release DATE NOT NULL,
	 Date_end Date NOT NULL,
	 Duration time not null,
	 [Description] NVARCHAR(255),
	 Poster image not Null
	 )

Create Table Type_place
	(Type_place_id INT Primary Key Identity,
	 Type_place_name nvarchar(50) NOT NULL)

Create Table Halls
	(Hall_id INT Primary Key Identity,
	 Hall_number int NOT NULL,
	 Cinema INT References Cinemas (Cinema_id) NOT NULL,
	 Type_hall Int References Type_hall (Type_hall_id) NOT NULL)

Create Table Places
	(Place_id INT Primary Key Identity,
	 Place_number int NOT NULL,
	 Hall INT References Halls (Hall_id) NOT NULL,
	 Type_place INT References Type_place (Type_place_id) NOT NULL,
	 [Row_number] int NOT NULL)

Create Table [Sessions]
	(Session_id INT Primary Key Identity,
	 Session_time datetime NOT NULL,
	 Price money NOT NULL,
	 Film INT References Films (Film_id) NOT NULL,
	 Hall INT References Halls (Hall_id) NOT NULL)

Create Table Tickets
	(Ticket_id INT Primary Key Identity,
	 [Session] Int References [Sessions] (Session_id) NOT NULL,
	 Place Int References Places (Place_id) NOT NULL)

Create Table Users
	([Login] NVARCHAR(30) Primary Key,
	 [Password] NVARCHAR(20) NOT NULL,
	 [Role] NVARCHAR(10) References Roles (Role_name) NOT NULL,
	 Last_name NVARCHAR(25),
	 First_name NVARCHAR(25))

Create Table Purchases
	(Purchase_id INT Primary Key Identity,
	 Buyer NVARCHAR(30) References Users ([Login]) NOT NULL,
	 Purchase_date datetime NOT NULL)

Create Table Purchase_details
	(Purchase INT References Purchases (Purchase_id),
	 Ticket INT references Tickets(Ticket_id),
	 Constraint PK_Purchase_details Primary Key (Purchase, Ticket))
	 
GO
INSERT INTO Roles
VALUES('Посетитель'),
	  ('Админ')

INSERT INTO Users
Values ('Master', '111111', 'Админ', 'Марк', 'Лиан'),
		('login20', '222222', 'Посетитель', null, null)

INSERT INTO Type_hall
VALUES  ('Обычный зал', 64),
		('VIP зал', 20)

INSERT INTO Type_place
VALUES  ('Обычное место'),
		('VIP место'),
		('Диван')

INSERT INTO Cinemas
VALUES  ('г.Москва, ул. Верхние поля, д.46')

INSERT INTO Halls
VALUES  (1, 1, 1),
		(2, 1, 1)

Insert INTO Places
Values  (1,1,1,1),(2,1,1,1),(3,1,1,1),(4,1,1,1),(5,1,1,1),(6,1,1,1),(7,1,1,1),(8,1,1,1),
		(1,1,1,2),(2,1,1,2),(3,1,1,2),(4,1,1,2),(5,1,1,2),(6,1,1,2),(7,1,1,2),(8,1,1,2),
		(1,1,1,3),(2,1,1,3),(3,1,1,3),(4,1,1,3),(5,1,1,3),(6,1,1,3),(7,1,1,3),(8,1,1,3),
		(1,1,1,4),(2,1,1,4),(3,1,1,4),(4,1,1,4),(5,1,1,4),(6,1,1,4),(7,1,1,4),(8,1,1,4),
		(1,1,1,5),(2,1,1,5),(3,1,1,5),(4,1,1,5),(5,1,1,5),(6,1,1,5),(7,1,1,5),(8,1,1,5),
		(1,1,1,6),(2,1,1,6),(3,1,1,6),(4,1,1,6),(5,1,1,6),(6,1,1,6),(7,1,1,6),(8,1,1,6),
		(1,1,1,7),(2,1,1,7),(3,1,1,7),(4,1,1,7),(5,1,1,7),(6,1,1,7),(7,1,1,7),(8,1,1,7),
		(1,1,1,8),(2,1,1,8),(3,1,1,8),(4,1,1,8),(5,1,1,8),(6,1,1,8),(7,1,1,8),(8,1,1,8),

		(1,2,1,1),(2,2,1,1),(3,2,1,1),(4,2,1,1),(5,2,1,1),(6,2,1,1),(7,2,1,1),(8,2,1,1),
		(1,2,1,2),(2,2,1,2),(3,2,1,2),(4,2,1,2),(5,2,1,2),(6,2,1,2),(7,2,1,2),(8,2,1,2),
		(1,2,1,3),(2,2,1,3),(3,2,1,3),(4,2,1,3),(5,2,1,3),(6,2,1,3),(7,2,1,3),(8,2,1,3),
		(1,2,1,4),(2,2,1,4),(3,2,1,4),(4,2,1,4),(5,2,1,4),(6,2,1,4),(7,2,1,4),(8,2,1,4),
		(1,2,1,5),(2,2,1,5),(3,2,1,5),(4,2,1,5),(5,2,1,5),(6,2,1,5),(7,2,1,5),(8,2,1,5),
		(1,2,1,6),(2,2,1,6),(3,2,1,6),(4,2,1,6),(5,2,1,6),(6,2,1,6),(7,2,1,6),(8,2,1,6),
		(1,2,1,7),(2,2,1,7),(3,2,1,7),(4,2,1,7),(5,2,1,7),(6,2,1,7),(7,2,1,7),(8,2,1,7),
		(1,2,1,8),(2,2,1,8),(3,2,1,8),(4,2,1,8),(5,2,1,8),(6,2,1,8),(7,2,1,8),(8,2,1,8)

Insert into Films
values ('Похождения ламы', '01-07-2020 00:00', '01-08-2020 00:00', '01:00:00', 'Лама мирно пасется в течении часа', '\Постеры\lama.jpg')
UPDATE Films SET poster=(SELECT * FROM OPENROWSET (BULK 'C:\Users\Nikke.tv\Desktop\Учеба\3.2\Курсовая\Постеры\lama.jpg',SINGLE_BLOB) AS X where Film_id=1)

Insert Into [Sessions]
values ('20-05-2020 13:30', 350, 1, 1)

Insert into Tickets
values (1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7),(1,8),(1,9),
	   (1,10),(1,11),(1,12),(1,13),(1,14),(1,15),(1,16),(1,17),(1,18),(1,19),
	   (1,20),(1,21),(1,22),(1,23),(1,24),(1,25),(1,26),(1,27),(1,28),(1,29),
	   (1,30),(1,31),(1,32),(1,33),(1,34),(1,35),(1,36),(1,37),(1,38),(1,39),
	   (1,40),(1,41),(1,42),(1,43),(1,44),(1,45),(1,46),(1,47),(1,48),(1,49),
	   (1,50),(1,51),(1,52),(1,53),(1,54),(1,55),(1,56),(1,57),(1,58),(1,59),
	   (1,60),(1,61),(1,62),(1,63),(1,64)

GO
If Exists(Select * From sysobjects where name = 'GetPlaces' and Type = 'P')
Drop Proc GetPlaces

GO
Create Proc GetPlaces @s int
as begin
	Select Ticket_id, Place_number, Row_number, Ticket From Tickets as T
	Inner Join Places as P on T.Place = P.Place_id
	left join Purchase_details as D on T.Ticket_id = D.Ticket
	Where Session = @s
end

GO
If exists(select * from sysobjects where name = 'GetPursheDetails' and type = 'P')
Drop Proc GetPursheDetails

GO
Create Proc GetPursheDetails @user nvarchar(30)
as begin
	Select Purchase_id, Purchase_date, Session_time, Price, S.Hall, Place_number, Row_number, F.Film_name from Purchases as P
	Inner join Purchase_details as D on P.Purchase_id = D.Purchase
	Inner join Tickets as T on D.Ticket = T.Ticket_id
	Inner join Sessions as S on s.Session_id = T.Session
	Inner join Places as Pl on T.Place = Pl.Place_id
	Inner Join Films as F on S.Film = F.Film_id
	where Buyer = @user
end

GO
If exists(Select * from sysobjects where name = 'GetFreeHalls' and Type = 'P')
drop proc GetFreeHalls

GO
Create Proc GetFreeHalls @cinema int, @film int, @datetime datetime
as begin
	Declare @film1 time = (Select Duration From Films Where Film_id = @film),
			@FilmEnd datetime
	Set @FilmEnd = Dateadd(MINUTE,DATEPART(minute, @film1),DATEADD(HH,DATEPART(hh,@film1), @datetime))

	Select Hall_id, Hall_number From Halls
	Where Cinema = @cinema AND 
	Hall_id not in 
	(
		Select Hall From Sessions as S
		Left Join Films as F on S.Film = F.Film_id
		Where (@datetime <= Session_time AND Session_time < @FilmEnd)  OR
			  (@datetime < Dateadd(MINUTE,DATEPART(minute, F.Duration),DATEADD(HH,DATEPART(hh,F.Duration), S.Session_time)) AND Dateadd(MINUTE,DATEPART(minute, F.Duration),DATEADD(HH,DATEPART(hh,F.Duration), S.Session_time)) <= @FilmEnd)
	)
end

go
if exists(select * from sysobjects where name='IsTicketsOnSessionBought' and type = 'P')
Drop Proc IsTicketsOnSessionBought

go
Create Proc IsTicketsOnSessionBought @Session int
as begin
	select * from Tickets as T
	inner join Purchase_details as P on T.Ticket_id = P.Ticket
	Where T.Session = @Session
End

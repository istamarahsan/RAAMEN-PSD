drop database Raamen;

create database Raamen;

use Raamen;

create table `Role`(
    `id` int primary key,
    `name` varchar(50) not null 
);

create table `User`(
    Id int primary key,
    Roleid int not null ,
    Username varchar(50) not null ,
    Email varchar(50) not null ,
    Gender varchar(50) not null ,
    `Password` varchar(50) not null ,
    foreign key (Roleid) references `Role`(id)
);

create table Header(
    id int primary key,
    CustomerId int not null ,
    Staffid int not null ,
    `Date` date,
    foreign key (CustomerId) references `User`(id)
);


create table Meat(
    id int primary key,
    `name` varchar(50)
);

create table Ramen(
    id int primary key,
    Meatid int not null ,
    `Name` varchar(50) not null ,
    Borth varchar(50) not null ,
    Price varchar(50) not null ,
    foreign key (Meatid) references Meat(id)
);

create table Detail(
    Headerid int,
    Ramenid int,
    Quantity int not null ,
    primary key (Headerid, Ramenid),
    foreign key (Headerid) references Header(id),
    foreign key (Ramenid) references Ramen(id)
);

insert into Role values
    (0, 'Customer'),
    (1, 'Staff'),
    (2, 'Admin')
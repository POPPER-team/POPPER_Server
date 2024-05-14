create database if not exists POPPERDB;

use POPPERDB;

create table if not exists Users
(
    id             int auto_increment,
    Guid           nvarchar(25)  Not Null,
    FirstName      nvarchar(255) Not Null,
    LastName       nvarchar(255) Not Null,
    Username       nvarchar(255) Not Null,
    Password       nvarchar(255) Not Null,
    Created        date          Not Null,
    DateOfBirth    date          Null,
    Status         nvarchar(255),
    WebLink        nvarchar(255),
    PreferredUnits nvarchar(255),
    Language       nvarchar(255),
    Primary key (id)
);

create table if not exists Following
(
    id          int auto_increment,
    UserID      int Not Null,
    FollowingID int Not Null,
    Primary key (id),
    foreign key (UserID) references Users (id),
    foreign key (FollowingID) references Users (id)
);

create table if not exists Post
(
    id          int auto_increment,
    Title       nvarchar(255) not null,
    Description nvarchar(255) not null,
    MediaGuid   nvarchar(255) not null,
    Duration    TIME          not null,
    UserId      int           not null,
    foreign key (UserId) references Users (id),
    primary key (id)
);

create table if not exists Ingredients
(
    id     int auto_increment,
    Text   nvarchar(255) not null,
    Unit   nvarchar(20)  not null,
    Amount int           not null,
    PostId int           not null,
    foreign key (PostId) references Post (id),
    primary key (id)
);

create table if not exists Views
(
    id     int auto_increment,
    UserId int not null,
    PostId int not null,
    foreign key (UserId) references Users (id),
    foreign key (PostId) references Post (id),
    primary key (id)
);

create table if not exists Saved
(
    id     int auto_increment,
    UserId int not null,
    PostId int not null,
    foreign key (UserId) references Users (id),
    foreign key (PostId) references Post (id),
    primary key (id)
);

create table if not exists Likes
(
    id     int auto_increment,
    UserId int not null,
    PostId int not null,
    foreign key (UserId) references Users (id),
    foreign key (PostId) references Post (id),
    primary key (id)
);

create table if not exists Steps
(
    id     int auto_increment,
    Step   int           not null,
    Text   nvarchar(255) not null,
    PostId int           not null,
    foreign key (PostId) references Post (id),
    primary key (id)
);

create table if not exists Comments
(
    id     int auto_increment,
    UserId int not null,
    PostId int not null,
    Text   nvarchar(255),
    Rating int,
    foreign key (UserId) references Users (id),
    foreign key (PostId) references Post (id),
    primary key (id)
);
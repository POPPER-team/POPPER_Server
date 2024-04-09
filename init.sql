-- Check if the database exists, if not, create it
CREATE DATABASE IF NOT EXISTS test;

-- Use the database
USE test;

-- Create the table if it doesn't exist
CREATE TABLE IF NOT EXISTS tableData (
    id INT AUTO_INCREMENT,
    name VARCHAR(255),
    PRIMARY KEY(id)
    );

-- Insert data into the table
INSERT INTO tableData (name) VALUES ('Data 1'), ('Data 2'), ('Data 3');

create database if not exists useri;
       
use useri;
    
    
create table if not exists Users(
    id int auto_increment,
    FirstName varchar(255),
    LastName varchar(255),
    DateOfBirth date,
    Username varchar(255),
    Password varchar(255),
    Created date,
    Status varchar(255),
    WebLink varchar(255),
    PreferedUnits varchar(255),
    Language varchar(255),
    
    
    Primary key(id)
    );

create table if not exists Following(
    id int auto_increment,
    UserID int,
    FollowingID int,
    Primary key(id)
    foreign key(UserID) references Users(id),
    foreign key(FollowingID) references Users(id)
    );  
);

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

create database if not exists POPPERDB;
       
use POPPERDB;
    
create table if not exists Users(
    id int auto_increment,
    Guid varchar(255) Not Null,
    FirstName varchar(255) Not Null,
    LastName varchar(255) Not Null,
    DateOfBirth date Not Null,
    Username varchar(255) Not Null,
    Password varchar(255) Not Null,
    Created date Not Null,
    Status varchar(255) Not Null,
    WebLink varchar(255) Not Null,
    PreferredUnits varchar(255) Not Null,
    Language varchar(255) Not Null,
    Primary key(id)
    );

create table if not exists Following(
    id int auto_increment,
    UserID int Not Null,
    FollowingID int Not Null,
    Primary key(id),
    foreign key(UserID) references Users(id),
    foreign key(FollowingID) references Users(id)
    );  


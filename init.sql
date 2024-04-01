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
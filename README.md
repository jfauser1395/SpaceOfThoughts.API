# SpaceOfThoughts.API


## Installation on Linux:

### 1. To make use of the API on Linux make sure the .net sdk 8 is installed properly:
	

	sudo apt install dotnet-sdk-8.0

### 2. Then we need to go ahead and install MySQL server and create a database 

	sudo apt install mysql-server 
	sudo mysql_secure_installation
#### VALIDATE PASSWORD COMPONENT: no (because we are testing the blog in a sandbox environment we don't need a strong password)
#### Remove anonymous users?: yes
#### Remove anonymous users?: yes
#### Remove anonymous users?: yes

	sudo mysql
#### Use your usual admin password to get into MySQL

	
	USE mysql;
#### Now we need to setup a database that is matching our connection string setup 
#### Default credentials can be found in the "appsettings.json" 
#### "SpaceOfThoughtsConnectionString": "server=localhost;database=sot;User=root;Password=44059513;"

#### First we need to make sure that the root user has the same password as we set up in the connection string
	
	ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '44059513';
	FLUSH PRIVILEGES;

#### Create the artblog database
	
	CREATE DATABASE artblog;

#### Grant all privileges to the root user on the artblog database
	
	GRANT ALL PRIVILEGES ON artblog.* TO 'root'@'localhost';
	FLUSH PRIVILEGES;

#### Exit MySQL
	
	EXIT;

### 3. After cloning the repository navigate into the root folder of the project and execute the database migrations

#### First delete all files inside the Migrations folder	
	sudo rm Migrations/ -rf   
	sudo mkdir Migrations
#### Install the EF Core tools globally
	sudo dotnet tool install --global dotnet-ef


	dotnet ef migrations add InitialCreate --context ApplicationDbContext
	dotnet ef migrations add InitialCreateAuth --context AuthDbContext
	dotnet ef database update --context ApplicationDbContext
	dotnet ef database update --context AuthDbContext

### 4. Finally start the API

	sudo dotnet run









# SpaceOfThoughts.API


## Installation on Linux:

### 1. To make use of the API on Linux make sure the .net sdk 8 and the entity framework core is installed properly:
	

	sudo apt install dotnet-sdk-8.0

#### Install the EF Core tools globally

	dotnet tool install --global dotnet-ef

#### To add it for the current terminal session, run:

	echo 'export PATH="$PATH:/home/$(whoami)/.dotnet/tools"' >> ~/.profile
	source ~/.profile

#### Check it the installation was successful

	dotnet ef --version


### 2. Then we need to go ahead and install MySQL server and create a database 

	sudo apt install mysql-server 
	sudo mysql_secure_installation

#### VALIDATE PASSWORD COMPONENT: no (because we are testing the blog in a sandbox environment we don't need a strong password)
#### Remove anonymous users?: yes
#### Disallow root login remotely?: yes
#### Remove test database and access to it?: yes
#### Reload privilege tables now?: yes

	sudo mysql

#### Use your usual admin password to get into MySQL
#### Now we need to setup a database that is matching our connection string setup 
#### Default credentials can be found in the "appsettings.json" 
#### "SpaceOfThoughtsConnectionString": "server=localhost;database=SPOT;User=root;Password=44059513;"

#### First we need to make sure that the root user has the same password as we set up in the connection string
	
	ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '44059513';
	FLUSH PRIVILEGES;

#### Create the artblog database
	
	CREATE DATABASE SPOT;

#### Grant all privileges to the root user on the artblog database
	
	GRANT ALL PRIVILEGES ON SPOT.* TO 'root'@'localhost';
	FLUSH PRIVILEGES;

#### Exit MySQL
	
	EXIT;

### 3. After cloning the repository navigate into the root folder of the project and execute the database migrations

#### First delete all files inside the Migrations folder

	sudo rm -rf Migrations/*

#### Now we need execute the database migrations

	dotnet ef migrations add InitialCreate --context ApplicationDbContext
	dotnet ef migrations add InitialCreateAuth --context AuthDbContext
	dotnet ef database update --context ApplicationDbContext
	dotnet ef database update --context AuthDbContext

### 4. Make sure to create the directory and set the appropriate permissions for the keys:
	sudo mkdir /var/mykeys
	sudo chown $(whoami):$(id -gn) /var/mykeys
	sudo chmod 700 /var/mykeys




### 4. Finally start the API

	sudo dotnet run









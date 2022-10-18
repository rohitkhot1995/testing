# Royal Stag Cricket Predictor
Royal Stag Cricket Predictor.
# Getting Started
1)Basic requirements of the project to run locally.
a)Local Sportz Database, Local Redis setup & AWS S3 assets write access.
b)VPN access & GitHub repo access.
2)Also provide useful library links or setup software(s) if any for execution. (None)
3)These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 
a)All the Point 1 requirements need to be met.
b)Get project copy from GitHub repo & resolve the project Dependencies if any & need to make sure Admin Dashboard is UP & running locally.
c)Populate the Data for required series from scoring calendar & scoring fixtures.
d)Scoring URL will have mostly recent data only to access old archive data can append “&year=2019” to get old series from feed.
e)Once Data is populated then need to populate the data for game & after we can begin simulation with API’s.
4)To get a local copy up and running follow these simple example steps.
a)Get copy of the project from the GitHub repo.
b)Resolve any project dependencies if any & make sure have to Database & AWS to connect.
c)To CONNECT AWS, MAKE PROFILE AND USE PROFILE INSTEAD OF UISNG CREDNTILAS IN CODE to connect AWS. 
d)Test DB & other required connection.
e)Once all the above steps are done, we can begin with Data population as per the requirement from scoring feed & calendar.
f)Test API once the generic data generated (Fixtures, Teams, Player Listing) from admin dashboard. If all works well then begin with gameplay development & testing.
# Prerequisites
1)Tools: 
a)Putty
b)VPN Access to connect Server. 
c)PUTTY GEN to generate public keys.
d)AWS Console access
e)Server private keys
2)Server Configuration.
a)Nginx Configuration
b).NET SDK 3.1 need to be installed.
c)Installation steps 
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg
sudo mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/
wget -q https://packages.microsoft.com/config/ubuntu/18.04/prod.list 
sudo mv prod.list /etc/apt/sources.list.d/microsoft-prod.list
sudo chown root:root /etc/apt/trusted.gpg.d/microsoft.asc.gpg
sudo chown root:root /etc/apt/sources.list.d/microsoft-prod.list
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install aspnetcore-runtime-3.1 -y 
sudo apt-get install dotnet-sdk-3.1

To verify dotnet installation 

Sudo dotnet –info
If you only want to see the SDKs: dotnet --list-sdks
If you only want to see installed runtimes: dotnet --list-runtimes

# Installing application on Server.                                                                                                    
1)Verify all the Prerequisites.
2)Publish & upload code AWS deployment bucket with Scripts and other services file to run application.
3)Download script file on server. Download sample command: sudo aws s3 cp s3://si-d11-code-deploy/prod/admin/bg-services/scripts/royalstag-predictor.sh ./
4)Run the script to deploy application.
Service file would be copied to /etc/system/system/ 
Script would enable & start the service.

sudo systemctl enable royalstag-predictor-web.service;
sudo systemctl start royalstag-predictor-web.service

Verify application status by 
Sudo systemctl status royalstag-predictor-web.service;

If any issue check journal for the service 

Sudo journalctl -u royalstag-predictor-web.service;

Sample service & scripts file attached.

Say what the step will be
# Usage
# Running the tests.
While putting up the API layer generate projectname.xml file for swagger & upload with API code deployment.
To view “Swagger Document” generate swagger json & put in web>wwwroot>swagger>config>filename.json
### Break down into end-to-end tests
# Deployment
While GO LIVE 
Make sure Swagger document access is restricted or the documents must be removed.
Make sure appsettings pointing to PROP Connections.
# Built With: 
1).Net Core - .Net Core 3.1
2)Database: PostgreSQL 
3)Frontend: React Js
4)OS Used: Linux 
# Contributing
**NOTE : When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.
# Versioning
GitHub.
GitHub Repo Link: https://github.com/sportzinteractive/gaming-royalstag-predictor.git
Branch name: Master 

# Authors

1.Asim - .NET - Development
3.Nitin Kadam - Database - Development
4.Ankit Rikame – Frontend – Development 
5.Rahul Patil - Product
# Acknowledgments
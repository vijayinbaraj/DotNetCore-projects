# DotNetCore
The objective of this project is to demostrate the implementation of the dotnet core console application and MSSQL on cross platform (Two tier Architecture) with integration to kafka. Application tier will be implemented to Linux and Database Tier will be implemented to Windows. 
The application will be configured as daemon service to produce the windows service like behaviour in Linux rhel 7.4 for real time application. 
The new Application has been designed as a multithread where the first thread will produce the data and insert the data to SQL database. The second thread will read the SQL data and publish to kafka. 


Run the below commands in the docker compose terminal to create containers, images
-----------------------------------------------------------------------------------
1) docker-compose up -d
2) docker start db_sqlserver ppsrfileupload-fileupload-frontend-1 ppsrfileupload-ppsrfileupload.server-1

The above two commends are enough to host the API, UI, Sqlserver in docker.

Banckend URL : https://localhost:8081/swagger/index.html
Frontend URL  : http://localhost:3000/

I have used docker desktop to check the containers, images and its process




Change the connection string to point to the local host if you are running the application in VS
This may not required, but install the below Nuget if Required to run the app in VS:

For PPSRFileUpload.server:
--------------------------
CsvHelper
Swashbuckle.AspNetCore.SwaggerUi

For PPSRFileUpload.Data:
------------------------
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.SqlServer

Unit Test project:
------------------
Moq

ppsrfileupload.client:
----------------------
npm install
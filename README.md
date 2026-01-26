# Project Summary

This repository is part of the Ambev .NET developer evaluation.
I implemented the Sales feature, covering create, list, update, and cancel operations, along with supporting infrastructure and automated validation.
<br /><br />


## Main Technologies & Integrations
- Seq for structured logging
- Postgres
- Grafana + Prometheus for monitoring and metrics
- Telemetry / Metrics integration
- RabbitMQ for messaging and asynchronous processing
- Redis for caching and idempotency
- GitHub Actions CI pipeline with build, tests, and coverage
- XUnit for automated testing
- Github Kanban Dashboard
<br /><br /><br /><br /><br /><br />






## Tutorial to run the application
1 - Open the application on Visual Studio  
2 - Open the Developer PowerShell  
3 - Use the docker command bellow to run the containers from docker-compose.yml  
```shell
  docker compose up -d
```
<img width="739" height="139" alt="image" src="https://github.com/user-attachments/assets/a83bf47a-b890-42f2-9a30-ae5b3df37628" />  

4 - Open the Docker Desktop e check if all the containers are running  
<img width="500"  alt="image" src="https://github.com/user-attachments/assets/f24ad0df-3a50-48c2-a65d-1068098dfd26" />  

5 - Execute the Migration command bellow to generate the tables on the Postgres Database
```shell
dotnet ef database update `
  --project src/Ambev.DeveloperEvaluation.ORM/Ambev.DeveloperEvaluation.ORM.csproj `
  --startup-project src/Ambev.DeveloperEvaluation.WebApi/Ambev.DeveloperEvaluation.WebApi.csproj
```  
<img width="784" height="123" alt="image" src="https://github.com/user-attachments/assets/b90eb116-76e9-4459-bd93-5b64dee7b4ee" />  

6 - Access the Postgres Database and check if the tables were created  

7 - Open the swagger mapped with the port 9826 http://localhost:9826/swagger/index.html  

8 - Access the api healthcheck to confirm if everything okay http://localhost:9826/health/ready

9 - You may test the application using the Swagger or the Postman Collection folder:  https://github.com/wellingtonfzambelli/ambev.evaluation/tree/main/postman-tests-collection

10 - I created a seed on migration to generates some data in the tables "Prodcuts", "Branches" and "Users"

11 - First you should authenticate. Bellow the user/password to authenticate  

```json
{
  "email": "wellington@test.com",
  "password": "12345678Wm@"
}
```  
12 - Now you are able to continue

<br /><br /><br /><br /><br /><br />





## Sales Feature Overview  

I Used Github Kanban Dashboard to organize my tasks  
https://github.com/users/wellingtonfzambelli/projects/4/views/1  
<img width="1477" height="954" alt="image" src="https://github.com/user-attachments/assets/26a7700f-81c2-4645-9bf8-a62363a3fc33" />
<br /><br /><br />


I setup the docker-compose file   
<img width="200"  alt="image" src="https://github.com/user-attachments/assets/fd1b6b14-f938-486f-89b2-1fa6ddaf3962" />  
<br /><br /><br />


I confirmed if the Docker containers are UP
<img width="1638" height="768" alt="image" src="https://github.com/user-attachments/assets/a47c6dce-32bf-4383-b31f-591106ee7c95" />
<br /><br /><br />


I accessed the swagger of the application to confirm it is loading correctly  
http://localhost:9826/swagger/index.html  
<img width="1575" height="991" alt="image" src="https://github.com/user-attachments/assets/cb3a3057-649a-4b80-bff0-6c075444126a" />
<br /><br /><br />


I confirmed if the HealthChecks API are Healthly on the container endpoint http://localhost:9826/health/ready
<img width="813" height="688" alt="image" src="https://github.com/user-attachments/assets/9e0c007b-894f-4b01-b6b7-bd98198584c3" />
<br /><br /><br />


I implemented the methods bellow
<img width="1453" height="554" alt="image" src="https://github.com/user-attachments/assets/f5d54b54-3012-48ff-b8e0-7b4265d6f139" />
<br /><br /><br />


After modeling the sales domain, I executed the migration on the application to create the database and tables
<img width="828" height="462" alt="image" src="https://github.com/user-attachments/assets/08dd18d7-6d71-4ee4-bf0c-6489675a682b" />
<br /><br /><br />


Checked if the database and table are created
<img width="1873" height="881" alt="image" src="https://github.com/user-attachments/assets/b168b004-0bc4-4adc-81a8-e2400438b164" />
<br /><br /><br />


I implemented the 'CorrelationID' concept, to track/log each request individually
<img width="1469" height="884" alt="image" src="https://github.com/user-attachments/assets/a0b11f7b-a00b-48af-898a-ba5b89fd3270" />
<br /><br /><br />


I used Redis for caching the product/branches lists. I used caching for getting sales content
<img width="1883" height="949" alt="image" src="https://github.com/user-attachments/assets/12bcb4be-0abd-451a-abf1-a2c6b8abfa9a" />
<br /><br /><br />


I also used Redis for Idempotency. If the user tries to create sales with the same correlationId, it's not possible  
<img width="400" alt="image" src="https://github.com/user-attachments/assets/2bee2a78-bd09-4c1c-9c16-9e83e64b20d0" />
<img width="400" alt="image" src="https://github.com/user-attachments/assets/318a6bd4-c658-49f4-876f-ff6d5e0bf725" />  
<img width="1908" height="500" alt="image" src="https://github.com/user-attachments/assets/1cfa732e-cc64-4f89-89e8-8fba1e3333c4" />
<br /><br /><br />


I used Seq to persist the application logging. I this case I'm searching by correlationId of the request to get the logs.
<img width="1893" height="450" alt="image" src="https://github.com/user-attachments/assets/87fc458f-4aba-4169-ad90-4732e3c4fde9" />
<br /><br /><br />


After the dababase persistence, I'm producing the message on the RabbitMQ Queues
<img width="1864" height="510" alt="image" src="https://github.com/user-attachments/assets/1e682dc8-6f9d-430d-a994-f13e45266bbc" />
<br /><br /><br />


I also implemented the RateLimit Pattern, to avoid a sequence of unecessary request at the application.   
This cenario I set at maximum 2 requests per 10 seconds  
<img width="400" alt="image" src="https://github.com/user-attachments/assets/cb46cb1b-dbdc-4dc3-a7ba-ae1acc1faaf6" />
<img width="400" alt="image" src="https://github.com/user-attachments/assets/e6e1e440-8c03-4cd7-9cb8-3e7810b78aa0" />
<br /><br /><br />



I'm collecting the metrics of the application to monitoring  
<img width="300" alt="image" src="https://github.com/user-attachments/assets/72febb7a-57ab-4068-98b7-dc035b5bc6a9" />
<br /><br /><br />



I created a Grafana Dashboard to monitoring: RabbitMQ Queues, Database Tables, HealtchChecks integrations, Api Resources _(CPU & Memory)_  
Container link: http://localhost:3000/d/ambev-dashboard/ambev-dashboard?orgId=1&from=now-5m&to=now&timezone=browser&refresh=5s  
<img width="1859" height="998" alt="image" src="https://github.com/user-attachments/assets/9187bd17-046f-48be-962f-0785b5fca8af" />
<br /><br /><br />


I implemented some tests focused on the 'Sales Feature' using XUnit  
<img width="561" height="372" alt="image" src="https://github.com/user-attachments/assets/68f18cb9-c1e2-4cb1-8c5c-85ff1522ca54" />
<br /><br /><br />


I created a Continous Integration Pipeline on the Github Actions to restore, build and test the application automatically every merge
<img width="500" alt="image" src="https://github.com/user-attachments/assets/e5075d57-cab4-45de-9890-559e8906e4d8" />
<img width="500" alt="image" src="https://github.com/user-attachments/assets/65b0cc10-92f7-435f-a897-136f9d25e5f1" />
<br /><br /><br />


The pipeline generates a global test report for downloading.  
Here is a pipeline link example: https://github.com/wellingtonfzambelli/ambev.evaluation/actions/runs/21365841531/job/61497242395  
<img width="1244" height="930" alt="image" src="https://github.com/user-attachments/assets/8c865fd3-53d0-48a0-aff6-44f8ce8163f3" />
<img width="1648" height="929" alt="image" src="https://github.com/user-attachments/assets/1894bc9b-eca9-4c39-a1fc-302324add75a" />

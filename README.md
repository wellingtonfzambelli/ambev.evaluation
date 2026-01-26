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
<br /><br />


## Tutorial to run the application


## Sales Feature Overview  

I Used Github Kanban Dashboard to organize my tasks  
https://github.com/users/wellingtonfzambelli/projects/4/views/1  
<img width="1477" height="954" alt="image" src="https://github.com/user-attachments/assets/26a7700f-81c2-4645-9bf8-a62363a3fc33" />
<br /><br />


I setup the docker-compose file   
<img width="200"  alt="image" src="https://github.com/user-attachments/assets/fd1b6b14-f938-486f-89b2-1fa6ddaf3962" />  
<br /><br />


I confirmed if the docke Containers are UP
<img width="1699" height="967" alt="image" src="https://github.com/user-attachments/assets/b46a48b9-3720-4db8-9fff-92fa89405172" />
<br /><br />


I accessed the swagger of the application to confirm it is loading correctly  
http://localhost:9826/swagger/index.html  
<img width="1575" height="991" alt="image" src="https://github.com/user-attachments/assets/cb3a3057-649a-4b80-bff0-6c075444126a" />
<br /><br />


I confirmed if the HealthChecks API are Healthly on the container endpoint bellow
http://localhost:9826/health/ready
<img width="813" height="688" alt="image" src="https://github.com/user-attachments/assets/9e0c007b-894f-4b01-b6b7-bd98198584c3" />
<br /><br />


I implemented the methods bellow
<img width="1453" height="554" alt="image" src="https://github.com/user-attachments/assets/f5d54b54-3012-48ff-b8e0-7b4265d6f139" />
<br /><br />


After modeling the sales domain, I executed the migration on the application to create the database and tables
<img width="828" height="462" alt="image" src="https://github.com/user-attachments/assets/08dd18d7-6d71-4ee4-bf0c-6489675a682b" />
<br /><br />


Checked if the database and table are created
<img width="1873" height="881" alt="image" src="https://github.com/user-attachments/assets/b168b004-0bc4-4adc-81a8-e2400438b164" />
<br /><br />


I implemented the 'CorrelationID' concept, to track/log each request individually
<img width="1469" height="884" alt="image" src="https://github.com/user-attachments/assets/a0b11f7b-a00b-48af-898a-ba5b89fd3270" />
<br /><br />


I used Redis for caching the product/branches lists. I used caching for getting sales content
<img width="1883" height="949" alt="image" src="https://github.com/user-attachments/assets/12bcb4be-0abd-451a-abf1-a2c6b8abfa9a" />


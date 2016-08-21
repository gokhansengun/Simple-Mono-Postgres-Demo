## A Sample Project Examplifying Usage of the Following Together

- ASP.NET OWIN on Mono
  - Backed by Dapper
  - Backed by Postgres
  - Powered by Npgsql
- ASP.NET Identity Framework on Mono
  - Backed by Dapper
  - Backed by Postgres
  - Powered by Npgsql
  - using bearer Token Authentication
- ASP.NET project built with Mono inside a Docker Container
- ASP.NET project run over Mono inside a Docker Container
- Docker containers orchestrated with Docker Compose
- Docker Compose powered by a Makefile
- Flyway to handle DB migrations
- Flyway also run over a Docker Container
- JMeter scripts to test scenarios

## Getting Started

Easy with `Docker`, possible without `Docker`, so just use `Docker` :-) Easy to run on Linux and Mac. Will need `make` installed on Windows (you can use Bash on Windows)

You need to have Docker version 1.12 and above installed on the computer.

### Steps

1. Clone the repository
2. Navigate to the `Docker` folder under the root directory
3. `make app` to build docker images and run containers in correct order
4. The app will listen on port 8090 on localhost
5. Run JMeter test scripts (*.jmx) located under AcceptanceTests folder. You should be able to run them with JMeter 2.13 and above.

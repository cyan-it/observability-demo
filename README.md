
# Observability Demo

This demonstrates how logs, traces and metrics work in a tech-stack with an Angular 17 Frontend and an Asp.Net Core Minimal Api Backend.

To run this, go to the root folder (where the docker-compose.yaml is) and run:

    1. docker-compose build
    2. docker-compose up -d

The credentials for grafana are:

    - Username: "username"
    - Password: "password"

The Web-UI gives you a couple buttons that will hit an endpoint in the Api which will return a StatusCode 200 in 50% of the cases, in 40% it will return the StatusCode of the button you clicked and in 10% it will return StatusCode 500, since an Error will be thrown inside the Application.

You can access various parts of the application like this:

-   Web-UI: http://localhost:4200
-   Api: http://localhost:5028/swagger
-   Grafana: http://localhost:3000
-   Prometheus: http://localhost:9090


Grafana will have two dashboards enabled by default:

-   Asp.Net Core Metrics
-   Loki Logs

To view Logs and Traces in Action, go to Loki, click on a Log and then click on View Trace. You will be taken to the Tempo Datasource and can view the Traces associated with the log.
Problem Statement

It’s 1999. You, a software engineer working at a rapidly growing scale-up. The company has outgrown its start-up era, single server setup. Things are starting to fail rapidly. You are tasked with designing and building a software-based load balancer to allow multiple machines to handle the load.

Your task is to implement a basic, software-based load-balancer, operating at layer 4. It must have the following capabilities:

• It can accept traffic from many clients • It can balance traffic across multiple backend services • It can remove a service from operation if it goes offline You may add other requirements as necessary / you feel appropriate. You may choose and language you are comfortable with. You should not use any cloud-services in the completion of this exercise.

Acceptance Criteria:

Given the Load balancer is running 
When requests are made from multiple different clients 
Then the Load Balancer still functions as it should without error

Given the Load Balancer is running 
When we receive multiple requests for our servers 
Then the Load balancer distributes requests across available servers

Given the Load Balancer is running 
When one of the backend servers is offline 
Then the Load balancer detects that and stops sending requests to that server


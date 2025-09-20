# Problem Statement

It's 1999. You, a software engineer working at a rapidly growing scale-up. The company has outgrown its start-up era, single server setup. Things are starting to fail rapidly. You are tasked with designing and building a software-based load balancer to allow multiple machines to handle the load.

Your task is to implement a basic, software-based load-balancer, operating at layer 4. It must have the following capabilities:

- It can accept traffic from many clients
- It can balance traffic across multiple backend services
- It can remove a service from operation if it goes offline You may add other requirements as necessary / you feel appropriate. 
- You may choose and language you are comfortable with. You should not use any cloud-services in the completion of this exercise.

## Acceptance Criteria:

Given the Load balancer is running 
When requests are made from multiple different clients 
Then the Load Balancer still functions as it should without error

Given the Load Balancer is running 
When we receive multiple requests for our servers 
Then the Load balancer distributes requests across available servers

Given the Load Balancer is running 
When one of the backend servers is offline 
Then the Load balancer detects that and stops sending requests to that server


## Repo notes
- This has been built using .Net8 sdk
- All Powershell commands were used through Powershell 5.1 
- Implicit usings are enabled in this repo


# Running the demo
## 1) Begin by creating a number of backend APIs (please sure these are outside this repo directory)


```
dotnet new web -n Api1
dotnet new web -n Api2
dotnet new web -n Api3
```

## 2) Assign API content

#### Api1/Program.cs
```
Set-Content Api1/Program.cs @'
var b = WebApplication.CreateBuilder(args);
var app = b.Build();
app.MapGet("/", () => "Backend 9001");
app.Run();
'@
```

#### Api2/Program.cs
```
Set-Content Api2/Program.cs @'
var b = WebApplication.CreateBuilder(args);
var app = b.Build();
app.MapGet("/", () => "Backend 9002");
app.Run();
'@
```

#### Api3/Program.cs
```
Set-Content Api3/Program.cs @'
var b = WebApplication.CreateBuilder(args);
var app = b.Build();
app.MapGet("/", () => "Backend 9003");
app.Run();
'@
```

## 3) Start each API on a fixed port in separate terminals
```
dotnet run --project Api1 --urls http://127.0.0.1:9001
dotnet run --project Api2 --urls http://127.0.0.1:9002
dotnet run --project Api3 --urls http://127.0.0.1:9003
```
## 4) Run the load balancer
Navigate to the diectory containing the csproj file.
From this directory run:
```
dotnet run
```

You should see:
```
Load Balancer listening on :9000 -> 127.0.0.1:9001, 127.0.0.1:9002, 127.0.0.1:9003
```

## Testing
You should now be able to directly connect to the APIs individually:
```
curl.exe -s http://127.0.0.1:9001/
curl.exe -s http://127.0.0.1:9002/
curl.exe -s http://127.0.0.1:9003/
```
which should show:
```
Backend 9001Backend 9002Backend 9003
```
 
 Or through the load balancer:
 ```
 curl.exe -s -H "Connection: close" http://127.0.0.1:9000/
 ```
 Which will show a variation of
 ```
 Backend 9002
 ```
 (depending on which endpoint is selected)

You can send multiple requests through and view the distribution of requests across the APIs:
```
1..30 | % { curl.exe -s -H "Connection: close" http://127.0.0.1:9000/ } |
  Group-Object | Sort-Object Count -Desc |
  Format-Table Name,Count -AutoSize
```

Which should show a distribution table similar to:
```Name         Count
----         -----
Backend 9001    11
Backend 9003    10
Backend 9002     9
```


You can also stop one API in one of the Powershell instances (Ctrl + c), run the requests and will only see the up servers (both through Load Balncer as single requests or through distribution)
There will also be a down server notification

You can then restart the stopped api; e.g. if you stopped API1, run:
```
dotnet run --project Api1 --urls http://127.0.0.1:9001
```
 and you will get a message saying re-added, and will see re-added server both through Load Balncer as single requests or through distribution



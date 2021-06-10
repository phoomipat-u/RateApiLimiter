# RateApiLimiter

<h3>API Instructions - I’m going to be using Docker so I don’t have to write 2 versions for Windows/Mac </h3>
1.	Navigate to the root of the project
2.	Build project image `docker --build-arg ASPNETCORE_ENVIRONMENT=Development build -t rate-api-limiter -f Dockerfile .`
3.  Run container `docker run --name rate-api-limiter -p 5001:80 rate-api-limiter`
4.  Navigate to `localhost:5001/swagger` to view the API swagger and the API usage instructions - you can try testing to API here
5.  (Optional) You can change the limit config in RateApiLLimiter/appsettings.json

<h3>Unit Test Instruction</h3>
1.  Navigate to the UnitTest directory
2.  Run `dotnet test --logger trx` to execute the unit tests and generate a test result file

<h3>Rate Limiting Demonstration - Windows</h3>
1. Run project using dotnet cli or Docker
2. Start PowerShell
3. Run `1..25 | ForEach-Object -Parallel { "Start of call $_"; $response = try {Invoke-WebRequest -Uri http://localhost:5001/hotel/city?city=Bangkok -Method Get | Select-Object -Expand StatusCode} catch { $_.Exception.Response.StatusCode } ; "Response Code of call $_ is $response"  ; "End of call $_"  } -ThrottleLimit 25`
4. You should get 10 successful calls and 15 unsuccessful calls (I couldn't make it return 429 instead of "TooManyRequests")

<h3>Rate Limiting Demonstration - Linux</h3>
1. Run project using dotnet cli or Docker
2. Start Terminal
3.
4.

<h3>Design Choices</h3>
-	The reason why I used an algorithm similar to Sliding Log is because we can have different periods for each endpoint. If we used the Fixed Window algorithm, we will require an expiration mechanism to manage the state. Leaky Bucket is not suitable because we’re looking to limit the API call over a period of time whereas the Leaky Bucket limits the API call to a specific amount at any given moment. Sliding Window is not used here because the requirement does not tell us to factor in the previous window when calculating the current window’s limit.
-   Ideally I wouldn't want to use query string which has the name as the API path i.e. `/hotel/city?city=Bangkok` since it doesn't really treat content as a resource but that can not be avoided as we have to have two endpoints in order to test out the different configurations. I've created a third endpoint `/hotel` which I think conforms more to the REST architecture - view this in Swagger

<h3>Assumptions & Limitations</h3>
-	The solution is limited to a single instance since it does not share the state with each other (if any). If we need this feature to be implemented across a cluster of servers, then a service that acts as a Gateway to monitor the API call limit or a centralized data store to help manage the state might be more preferable.
-	The limit set for each configuration is NOT specific to any API token or IP. This means that if one caller spams the API, the API could be unusable for other users.
-   The dotnet unit tests can't be run case-by-case so the integration tests will have to be run manually one at a time otherwise they will interfere with each other.



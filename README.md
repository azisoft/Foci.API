# Foci.API

- clone github repo
- open Foci.API.sln in VS 2022 (I used the community edition)
- make sure you have installed the following workloads in VS:
  - .NET desktop development
  - ASP.NET and web development
  - .NET Core cross-platform development
- the API can use either a SQL Server database, or an in-memory database - this is controlled in appSettings.json -> "useInMemoryDatabase" flag
	- for in-memory, there is no need to configure DB access, but the initial data is blank, you need to use the API (either front-end or Swagger) to supply some data
	- for SQL Server, run the db.sql file on a SQL Server instance and then configure your connection string in appSettings.json -> "DefaultConnection" (I used SQL Server Express edition)
- hit F5 - you will be taken to the swagger page of this API project
- to run tests, right click on the test project and select "Run Tests"

# Demo

You can find a demo instance (using SQL Server) of this API at http://foci-demo.azisoft.ca/swagger
You can also find a demo instance of the front-end application consuming this API at http://foci-demo.azisoft.ca/app

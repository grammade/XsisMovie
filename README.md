
# XsisMovieService

Xsis prerequisite project with basic .NET 7 WebApi Service


## Features

- (requirement) Movie CRUD
- (requirement) Request Validation with FluentValidation
- (requirement) Unit Test
- (requirement) Global Error Handling
- EF code first migration
- JWT authorization
- Paging with meta-data


## Environment Variables

To run this project, you will need to adjust the following environment variables (appsetting.json) according to your local

`ConnectionStrings:DefaultConnection`


## Run Locally

Clone the project

```bash
  git clone https://github.com/grammade/Agit-TasksService.git
```


Required Depedencies

- .NET 7 Runtime
- MS SQL

Run migration
```bash
  //navigate to project dir
  dotnet ef database update
```

Start the server
```bash
  dotnet build
  dotnet run
```


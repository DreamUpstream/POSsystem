# What is this project about?

POS system is a proof of work point of sale system to demonstrate building a multi-container Full Stack application with ASP.NET Core (.NET 6) Web API following Clean Architecture. The solution uses Docker Compose to orchestrate deployment of this entire stack to Docker.

# What does the Solution offer?

The Solution is built keeping in mind the most fundamental blocks an API must have in order to build a scalable and near-perfect API component. The solution offers a complete implementation of the following:

- Clean Architecture with separated layers for API, Core, Contracts, Infra and Migrations
- UnitOfWork with Generic Repository
- Entity Framework Core migrations with SQLite
- Complete CRUD for an Entity following CQRS, with segregated Commands and Queries
- Fluent Validation of input inside the Command classes
- Preconfigured Swagger UI
- ETag generation and validation on the API side for Response Caching (GET) and Collision detection (PUT)
- Ready to use Docker configuration with Dockerfiles
- In-Memory Caching for single Entity via IMemoryCache
- Distributed Caching implementation via IDistributedCache, with NCache 
- JWT Token API for Generation and Configured JWT Validation
- Roles based Authorization with predefined Roles
- Auditable Entities with User Tracking
- API Versioning with separated Swagger Documentation
- AutoMapper implementation for Entity-to-DTO conversion
- One Command deployment in Docker with Docker Compose
- ILogger logging implementation
- Database Seeding with a Single User and Roles added as the application starts

# Technologies Used

* ASP.NET Core (.NET 6) Web API
* Entity Framework Core (EFCore 6)
* MediatR for .NET 6
* Fluent Validation for .NET 6
* SQLite
* SwaggerUI
* AutoMapper
* Nginx (Proxy)
* Docker Compose

# How do I get started with Docker Compose?

To get started, follow the below steps:

1. Install .NET 6 SDK (optional, for developer only / for reviewers there is no need.)
2. Install Docker Desktop (for Windows) / Docker (for Linux/Mac)
3. Clone the Solution into your Local Directory
4. On the Repository root you can find the docker-compose.yml file
5. Run the command in the directory to build and run the solution in Docker (requires a working Docker installation)

```
docker-compose build --force-rm --no-cache
```
### After it's done installing
```
docker-compose up -d
```

# API REQUEST EXAMPLES:
### (Make sure to add Content-Type application/json to every request)
POST -> http://localhost:8084/api/v1/Auth/token | Payload: {"emailAddress": "admin@admin.com", "password": "admin"}
Response -> {
    "accessToken": "[YOUR TOKEN]",
    "expiresIn": 10
}

GET -> http://localhost:8084/api/v1/Items | Authorization: Bearer [TOKEN RECEIVED BEFORE] 
Response -> 200 {[Items, if any]}

POST -> http://localhost:8084/api/v1/Items | Authorization: Bearer [TOKEN RECEIVED BEFORE] | Payload: {"name": "Burger", "description": "A delicious american burger", "categories": "Food", "colorCode": "#fd6d6d"}
Response -> 201 {
    "name": "Burger",
    "description": "A delicious american burger",
    "categories": "Food",
    "colorCode": "#fd6d6d",
    "createdBy": "1",
    "modifiedBy": null,
    "lastModified": null,
    "id": 1,
    "created": "2022-12-11T21:21:46.570839Z"
}


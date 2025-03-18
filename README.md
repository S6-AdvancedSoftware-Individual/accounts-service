# Accounts-service

The service responsible for handling CRUD operations of accounts and detailed account data retrievals.

## Build images

``docker-compose build``

## Create & run containers

``docker-compose up``

## To stop the service

``docker-compose down``

## EF hell

``dotnet ef migrations add InitialCreate --project .\Infrastructure\Infrastructure.csproj --startup-project .\AccountService\Api.csproj``

from 'PS C:\Git\s6-individual\accounts-service\api\AccountService>' run
``dotnet ef database update --project .\Infrastructure\Infrastructure.csproj --startup-project .\AccountService\Api.csproj``

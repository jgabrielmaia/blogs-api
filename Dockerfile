# Set the base image to the official .NET 6 SDK image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the solution file and restore dependencies
COPY BestBlogs.sln .
COPY src/Api/Api.csproj ./src/Api/
COPY src/Model/Model.csproj ./src/Model/
COPY src/Repository/Repository.csproj ./src/Repository/
COPY test/Api.Tests/Api.Tests.csproj ./test/Api.Tests/
COPY test/Repository.Tests/Repository.Tests.csproj ./test/Repository.Tests/
COPY test/Model.Tests/Model.Tests.csproj ./test/Model.Tests/

RUN dotnet restore BestBlogs.sln
RUN dotnet test --no-restore --verbosity normal
RUN dotnet build -c Release --no-restore

# Publish the API project
WORKDIR /app/src/Api
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/src/Api/out .

# Expose the port
EXPOSE 5000

# Set the entry point
ENTRYPOINT ["dotnet", "Api.dll"]

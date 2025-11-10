# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj first for layer caching
COPY ./Api/Api.csproj Api/
COPY ./Application/Application.csproj Application/
COPY ./Domain/Domain.csproj Domain/
COPY ./Infrastructure/Infrastructure.csproj Infrastructure/

RUN dotnet restore Api/Api.csproj

# copy the rest and publish
COPY . .
RUN dotnet publish Api/Api.csproj -c Release -o /app/publish

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
CMD ["dotnet", "Api.dll"]

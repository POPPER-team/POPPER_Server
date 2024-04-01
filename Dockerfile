FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app

COPY ["POPPER_Server.csproj", "./"]
RUN dotnet restore "POPPER_Server.csproj"

COPY . .

# Expose ports for the application
EXPOSE 8080
EXPOSE 8443
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "watch", "run", "--no-launch-profile"]

#releasse config
#FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8443
#
#FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["POPPER_Server.csproj", "./"]
#RUN dotnet restore "POPPER_Server.csproj"
#COPY . .
#WORKDIR "/src/"
#RUN dotnet build "POPPER_Server.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "POPPER_Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "POPPER_Server.dll"]

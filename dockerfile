# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application files
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image as the base image for the runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install FFmpeg
RUN apt-get update && \
    apt-get install -y ffmpeg

# Set the working directory
WORKDIR /app

# Copy the build output from the build stage
COPY --from=build /app/out ./

# Set the entry point for the application
ENTRYPOINT ["dotnet", "FrameExtraction.dll"]

# Default command-line arguments (can be overridden)
CMD ["/input", "/output", "1"]
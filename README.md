# Video Frame Extractor

This tool is an incredibly basic tool designed to extract frames from video files at a specified frame rate, resolution, and aspect ration and save them into a structured directory format. This is a basic implementation of FFMPEG inside a docker container. Follow the steps below to get started.

## Prerequisites

- Docker installed on your machine
- Git (optional, only if you prefer to clone the repository instead of downloading the source code directly)

## Getting Started

### Step 1: Clone the Repository

First, clone the repository to your local machine using Git. Or download it.

```bash
git clone https://github.com/dso4/seeSharpFrameExtraction.git
```

### Step 2: Build the Docker Image
Navigate to the root directory of the project (where the Dockerfile is located) and build the Docker image. Replace your_image_name with the name you want to give to your Docker image.

```bash
cd path/to/project
docker build -t your_image_name .
```
### Step 3: Run the Docker Image
After building the image, you can run it using Docker. You need to specify the input directory, output directory, and frame rate as arguments. Replace your_image_name with the name of your Docker image, and adjust the <inputDirectory>, <outputDirectory>, and <frameRate> placeholders with your desired values.
### NOTE: 
only works with .mp4 videos, will probably update that at some point,
framerate is fps, meaning a value of 1 will take a png snapshot every second
The output directory will be created if it does not exist
if the source video is lower resolution than 1024 on one side, a further subdirectory will be created (low_resolution) for post-processing. I think. We'll see


```bash
#windows style:
docker run -v F:\king:/input -v F:\quing\:/output <your image name> /input /output 1
#bash style:
docker run -v /path/to/local/inputDirectory:/app/inputDirectory -v /path/to/local/outputDirectory:/app/outputDirectory your_image_name /input /output /1
```

### Warranty Information
none. No warranty is either implied or really even considered. Always check source code, and be careful when running code from the internet. 

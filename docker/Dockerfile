FROM tafilz/xamarin-android:31-latest AS build
WORKDIR /netcore3.1
RUN curl -o dotnet.tar.gz https://download.visualstudio.microsoft.com/download/pr/e89c4f00-5cbb-4810-897d-f5300165ee60/027ace0fdcfb834ae0a13469f0b1a4c8/dotnet-sdk-3.1.426-linux-x64.tar.gz
RUN tar zxf dotnet.tar.gz
WORKDIR /src
COPY TimeTrackerXamarin/TimeTrackerXamarin/ TimeTrackerXamarin/TimeTrackerXamarin/
COPY TimeTrackerXamarin/TimeTrackerXamarin.Android/ TimeTrackerXamarin/TimeTrackerXamarin.Android/
COPY TimeTrackerXamarin/TimeTrackerXamarin.Test/ TimeTrackerXamarin/TimeTrackerXamarin.Test/
COPY docker/start.sh start.sh
ENTRYPOINT ["./start.sh"]

#!/bin/bash

function build {
  if [ ! -d "/output/" ]; then
    echo "Volume for output was not connected, please start container with /output/ volume connected."
    exit 1
  fi

  msbuild TimeTrackerXamarin/TimeTrackerXamarin.Android/TimeTrackerXamarin.Android.csproj /p:AndroidSdkDirectory=/usr/lib/android-sdk /p:Configuration="Debug" /p:Platform="AnyCPU" /restore
  msbuild TimeTrackerXamarin/TimeTrackerXamarin.Android/TimeTrackerXamarin.Android.csproj /p:AndroidSdkDirectory=/usr/lib/android-sdk /p:Configuration="Debug" /p:Platform="AnyCPU" /t:SignAndroidPackage /p:OutputPath="/build"

  cp /build/com.devpark.time_tracker-Signed.apk /output/com.devpark.time_tracker-Signed.apk
}

function test {
  dotnet restore TimeTrackerXamarin/TimeTrackerXamarin.Test/TimeTrackerXamarin.Test.csproj
  dotnet build TimeTrackerXamarin/TimeTrackerXamarin.Test/TimeTrackerXamarin.Test.csproj -o /tests/
  cd /tests || exit
  /netcore3.1/dotnet test TimeTrackerXamarin.Test.dll
}

case "$1" in
  "build-android") build ;;
  "test") test ;;
  *) echo "Invalid or missing parameter. Valid parameters: build-android, test" ;;
esac

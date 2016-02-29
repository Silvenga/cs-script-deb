# CS-Script

[![Build Status](https://img.shields.io/teamcity/https/teamcity.silvenga.com/e/CsScript_Build.svg?label=TeamCity&style=flat-square)](https://teamcity.silvenga.com/viewType.html?buildTypeId=CsScript_Build&guest=1)

Debian/Ubuntu package builder for cs-script (http://www.csscript.net/).

The final package can be found at http://deb.silvenga.com.

# Build Your Own

### Build Dependencies

```
# Build dependencies
apt-get install p7zip-full mono-complete devscripts debhelper
# Get certs for mono
mozroots --import --ask-remove
```

### Building

```
./build --target Build
```

### Cleaning

```
./build --target Clean
```

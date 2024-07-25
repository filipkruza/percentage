# percentage

![image](./res/screenshot1.jpg)

See your battery percentage in the Windows 10/11 system tray/notification area

## Fork

This fork has additional features
- meant to be used alongside normal icon, so disabled charging icon ( `96*` -> `96` )
- Light theme support (text can be shown in black color)

   ![image](./res/screenshot2.jpg)
- Updated .NET to v4.7.2
- starting up a lot faster
- Update timer reduced from 1 to 5s

## Installing

1. [Download the latest release](https://github.com/filipkruza/percentage/releases)
1. Put percentage.exe in your startup folder
   1. To get to your startup folder, press `Windows`+`R`, type `shell:startup`

## Compiling

This project was compiled with Visual Studio 2022.

Select ".NET desktop development" when setting up Visual Studio.

To build the project
1. Open the percentage/percentage.sln file with Visual Studio
1. Switch Debug to Release

   ![image](./res/screenshot3.jpg)

1. Click "Build > Build Solution"
1. percentage.exe can be found at percentage\percentage\percentage\bin\Release\percentage.exe

## Contributions

> My goal for this project is to keep it as simple as possible. I welcome suggestions, but for complicated features I'd recommend forking the project.

Original goal remains the same

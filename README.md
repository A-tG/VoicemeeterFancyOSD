# Voicemeeter Fancy OSD
![](https://i.imgur.com/YpWj6YI.gif)

On-Screen display for Voicemeeter that works with fullscreen apps by using private WinAPI without hooking into Graphics API.

Windows 10 or newer is required. 
May work with older Windows versions (not older than Windows 7) but OSD will work as normal window and will not be displayed on top of fullscreen applications.

Install .NET 5.0 Desktop runtime so I don't have to upload a "portable" version of the program that's about 150 MB in size.

You can change settings in config\config.ini (created on program launch if not exist). And settings are automatically validated and updated on file change, no need to restart.

# Build instructions
[.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) WPF project

Start/build in debug mode to get acces to Debug Window in tray context menu.

* Select Target Platform (x64 or x86, dont use AnyCpu or \*Host.exe will not be copied).
* Press "Rebuild Solution" to guarantee that up-to-date Host.exe and dependencies are copied to result folder.
* The program will be compiled in the **Solution's** folder.
* Launch \*Host.exe or ontop fullscreen functionality will not work.

# Uses
* [Ini File Parser](https://github.com/rickyah/ini-parser)
* [Extended Voicemeeter Remote API wrapper](https://github.com/A-tG/voicemeeter-remote-api-extended)
* Partially code from [ModernFlyouts](https://github.com/ModernFlyouts-Community/ModernFlyouts)

# Explanation
* Don't forget app.manifest if you want to modify/make your own program based on this or BandWindow might throw Exception.
* Make whatever WPF UserControl you want and assign it to BandWindow's Content
* All magic stuff happens in Host and Bridge projects (copied from [ModernFlyouts](https://github.com/ModernFlyouts-Community/ModernFlyouts)). Without it, it's not that easy to create "true" topmost window. The alternative is to sign your exe file with Microsoft Windows certificate.
* Main code for topmost window is located in [Interop](VoicemeeterOsdProgram/Interop) based on a modified code from [ModernFlyouts](https://github.com/ModernFlyouts-Community/ModernFlyouts) (Thanks for advice and directions from their Discord server!)
* [Program.cs](VoicemeeterOsdProgram/Program.cs) and [App.xaml.cs](VoicemeeterOsdProgram/App.xaml.cs) are entry points. The program dll's name should be defined [here](Bridge/dllmain.cpp#L42)
* As far as I know, host is actually renamed ApplicationFrameHost.exe from System32
* [What "private" API is used](https://blog.adeltax.com/window-z-order-in-windows-10/)

## Donate to support the project
[![Paypal Logo](https://www.paypalobjects.com/webstatic/paypalme/images/pp_logo_small.png)](https://www.paypal.me/atgDeveloperMusician/5)

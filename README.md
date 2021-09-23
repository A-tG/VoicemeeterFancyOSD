# Voicemeeter Fancy OSD
[.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) WPF project

**WIP** Prototype. Voicemeeter API is not fully integrated yet!
Start/build in debug mode to get acces to Debug Window in tray context menu.

On-Screen display for Voicemeeter that works with fullscreen apps by using private WinAPI without hooking into Graphics API.

Windows 10 or newer is required. 
Support for older Windows versions can be added but OSD will work as normal window and will not be displayed on top of fullscreen applications.

Based on a modified code from [ModernFlyouts](https://github.com/ModernFlyouts-Community/ModernFlyouts) (Thanks for advice and directions from their Discord server!)

# Build instructions
* Select Target Platform (e.g. x64, x86)
* Launch \*Host.exe or fullscreen ontop functionality will not work

# Explanation
* Don't forget app.manifest if you want to modify/make your own program based on this or BandWindow might throw Exception.
* All magic stuff happens in Host and Bridge projects. Without it, it's not that easy to create "true" topmost window. Only alternative is to sign your exe file with Microsoft Windows certificate.
* Main code for topmost window is located in [Interop](VoicemeeterOsdProgram/Interop)
* [Program.cs](VoicemeeterOsdProgram/Program.cs) is a entry point for the program and the "host"
* As far as I know, host is actually renamed ApplicationFrameHost.exe from System32
* [What "private" API is used](https://blog.adeltax.com/window-z-order-in-windows-10/)

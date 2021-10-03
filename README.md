# Voicemeeter Fancy OSD
![](https://i.imgur.com/YpWj6YI.gif)

On-Screen display for Voicemeeter that works with fullscreen apps by using private WinAPI without hooking into Graphics API.

Windows 10 or newer is required. 
Should work with older Windows versions but OSD will work as normal window and will not be displayed on top of fullscreen applications.

[.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) WPF project

Start/build in debug mode to get acces to Debug Window in tray context menu.

# Build instructions
* Select Target Platform (x64 or x86, dont use AnyCpu or \*Host.exe will not be copied).
* Press "Rebuild Solution" to guarantee that up-to-date Host.exe and dependencies are copied to result folder.
* The program will be compiled in the **Solution's** folder.
* Launch \*Host.exe or ontop fullscreen functionality will not work.

# Explanation
* Don't forget app.manifest if you want to modify/make your own program based on this or BandWindow might throw Exception.
* All magic stuff happens in Host and Bridge projects. Without it, it's not that easy to create "true" topmost window. The only alternative is to sign your exe file with Microsoft Windows certificate.
* Main code for topmost window is located in [Interop](VoicemeeterOsdProgram/Interop) based on a modified code from [ModernFlyouts](https://github.com/ModernFlyouts-Community/ModernFlyouts) (Thanks for advice and directions from their Discord server!)
* [Program.cs](VoicemeeterOsdProgram/Program.cs) and [App.xaml.cs](VoicemeeterOsdProgram/App.xaml.cs) are entry points. The program dll's name should be defined [here](Bridge/dllmain.cpp#L42)
* As far as I know, host is actually renamed ApplicationFrameHost.exe from System32
* [What "private" API is used](https://blog.adeltax.com/window-z-order-in-windows-10/)

## Do you like my projects? Donate
[![Paypal Logo](https://www.paypalobjects.com/webstatic/paypalme/images/pp_logo_small.png)](https://www.paypal.me/atgDeveloperMusician/5)

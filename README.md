# Voicemeeter Fancy OSD
[.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) WPF project

**WIP** Prototype. Voicemeeter API is not integrated yet!
Start/build in debug mode to get acces to Debug Window in tray menu.

On-Screen display for Voicemeeter that work with fullscreen apps by using private Windows' API without hooking into Graphics API.

In order to show OSD on top of fullscreen apps Windows 10 is required. 
Support for other Windows versions can be added but without fullscreen apps support.

Based on modified code from [ModernFlyouts](https://github.com/ModernFlyouts-Community/ModernFlyouts) (Thanks for advices and directions from it's Discord server!)

# Build instructions
* Select Target Platform (e.g. x64, x86)
* hostfxr.dll is required. You can publish the program as Self-contained and copy hostfxr.dll to output folder
* Launch \*Host.exe or fullscreen ontop functionality will not work

# Explanation
* app.manifest is crucial if you want to modify/make your own program based on this
* Minimal
* All magic stuff happens in Host and Bridge projects. Without it, it's not possible to create "true" topmost window. Only alternative is to sing your exe file with Microsoft Windows certificate.
* Main code for topmost window is located in [Interop](VoicemeeterOsdProgram/Interop) and [OsdWindow](VoicemeeterOsdProgram/UiControls/OSD/OsdWindow.cs) derived from it
* [Program.cs](VoicemeeterOsdProgram/Program.cs) is entry point
* [What "private" API is used](https://blog.adeltax.com/window-z-order-in-windows-10/)

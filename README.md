# Voicemeeter Fancy OSD
![](https://i.imgur.com/hleMUFv.gif)

On-Screen display for [Voicemeeter](https://voicemeeter.com/) that works with fullscreen apps by using private WinAPI without hooking into Graphics API.

Windows 10 or newer is required. May work with older Windows versions (no older than Windows 7) but the actual OSD will work as a normal window and will not be displayed on top of fullscreen applications.

It might also not work with some OpenGL games, even if in borderless mode. Basically if "Xbox Game Bar" or standard Volume/Media pop-up is not showing on top of the game, neither will this OSD.

# Installation and Usage
The program is portable, no installation is required, but you need [.NET Desktop runtime](https://dotnet.microsoft.com/download/dotnet/8.0/runtime) (click '*Download x64*' under '*Run desktop apps*') **or alternatively** you may install the runtime via Command Prompt with the following command: 

`winget install Microsoft.DotNet.DesktopRuntime.9`

(winget is bundled in Windows 11 and some modern versions of Windows 10 by default as the "App Installer", if your Windows version does not include it, you may read [here](https://docs.microsoft.com/en-us/windows/package-manager/winget/) for further instructions on how to get it). 

Download the latest version of VoicemeeterFancyOSD from the [Releases](https://github.com/A-tG/VoicemeeterFancyOSD/releases) page, unpack and launch VoicemeeterFancyOsdHost.exe (if you're on Windows 8.1 or newer). For any other scenario such as running an older Windows version, or if VoicemeeterFancyOsdHost.exe simply doesn't work for some reason, you may instead try and run VoicemeeterFancyOsd.exe. Do not use the program from Windows' 'Program Files' since that might cause problems creating the config file or installing future updates because of Windows' permissions and policies.

# Upcoming features
Check the [Milestones](https://github.com/A-tG/VoicemeeterFancyOSD/milestones)

# What's the purpose?
When you change gain/routing/mute etc, by using Voicemeeter Macro Buttons (or any other way), for example if some of your buttons configuration looks like this:

![](https://i.imgur.com/M3mwHnY.png)

For the above example it's very convenient to see changes on a little On-screen display instead of opening the actual Voicemeeter window.
In other words it works similar to Windows' volume pop-up but for Voicemeeter, so you can enjoy all good things like volume/gain in decibels instead of the unnatural 0-100 range that Windows uses.

For example, in Voicemeeter when you change the gain from 0 to -60, you will experience a smooth volume transition from loud to quiet, but in Windows' volume pop-up 100% is 0 in Voicemeeter, 50% is actually equivalent to Voicemeeter's -10.0, 25% is around -20.0, 12.5% is -30.0 and so on - i.e not a very smooth transition and enjoyable experience.

So this program should be a good "replacement" of the default volume pop-up in Windows, considering that you're using Voicemeeter of course.

# Advanced troubleshooting
VoicemeeterFancyOsdHost.exe, hostfxr.dll and DXGI.dll are necessary to display OSD on top of fulscreen games, but if you have weird problems launching VoicemeeterFancyOsdHost.exe you can try to replace hostfxr.dll with different version bundled with Dotnet from `C:\Program Files\dotnet\host\fxr\N.N.N\hostfxr.dll`. Also VoicemeeterFancyOsdHost.exe is a renamed ApplicationFrameHost.exe from `C:\Windows\System32` and can be replaced too if there are some problems with it. DXGI.dll is compiled and helps to launch program itself by Host.exe. If even VoicemeeterFancyOsd.exe is not working deletion of these is also an option but you lose that functionaluty with fullscreen games of course.

# Build instructions
[.NET 9.0](https://dotnet.microsoft.com/download) WPF project

Start/build in debug mode to get acces to Debug Window in tray context menu.

* Select Target Platform (x64 or x86, dont use AnyCpu or \*Host.exe will not be copied).
* Press "Rebuild Solution" to guarantee that up-to-date Host.exe and dependencies are copied to the results folder.
* The program will be compiled in the **Solution** folder.
* Launch \*Host.exe or on-top fullscreen functionality will not work.

# Uses
* [INI File Parser](https://github.com/rickyah/ini-parser)
* [Extended Voicemeeter Remote API wrapper](https://github.com/A-tG/voicemeeter-remote-api-extended)
* [WpfScreenHelper](https://github.com/micdenny/WpfScreenHelper)
* [Hardcodet NotifyIcon for WPF](https://github.com/hardcodet/wpf-notifyicon)
* [Octokit.net](https://github.com/octokit/octokit.net)
* Partially code from [ModernFlyouts](https://github.com/ModernFlyouts-Community/ModernFlyouts)

# Explanation for developers
* Don't forget app.manifest if you want to modify/make your own program based on this or BandWindow might throw Exception.
* Make whatever WPF UserControl you want and assign it to BandWindow's Content
* All magic stuff happens in the Host and Bridge projects (copied from [ModernFlyouts](https://github.com/ModernFlyouts-Community/ModernFlyouts)). Without it, it's not that easy to create "true" topmost window. The alternative is to sign your .exe file with Microsoft Windows certificate.
* Main code for topmost window is located in [Interop](VoicemeeterOsdProgram/Interop) based on a modified code from [ModernFlyouts](https://github.com/ModernFlyouts-Community/ModernFlyouts) (Thanks to their Discord server for advice and directions!)
* [Program.cs](VoicemeeterOsdProgram/Program.cs) and [App.xaml.cs](VoicemeeterOsdProgram/App.xaml.cs) are entry points. The program dll's name should be defined [here](Bridge/dllmain.cpp#L42)
* As far as I know, Host.exe is actually renamed ApplicationFrameHost.exe from System32
* [What "private" API is used](https://blog.adeltax.com/window-z-order-in-windows-10/)

## Donate to support the project
[Available methods](https://taplink.cc/atgdev)

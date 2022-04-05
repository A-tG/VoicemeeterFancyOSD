# Voicemeeter Fancy OSD
![](https://i.imgur.com/qdyqeYF.gif

On-Screen display for [Voicemeeter](https://voicemeeter.com/) that works with fullscreen apps by using private WinAPI without hooking into Graphics API.

Windows 10 or newer is required. 
May work with older Windows versions (not older than Windows 7) but OSD will work as normal window and will not be displayed on top of fullscreen applications.

It might not work with some OpenGL games, even in borderless mode. Basically if "Xbox Game Bar" or standard Volume/Media pop-up is not showing on top of the game, neither will this OSD.

# Installation and Usage
The program is portable, no installation required, but you need [.NET Desktop runtime](https://dotnet.microsoft.com/download/dotnet/6.0/runtime). Download the latest version from the [Releases](https://github.com/A-tG/VoicemeeterFancyOSD/releases) page, unpack and launch VoicemeeterFancyOsdHost.exe (on Windows 8.1 or newer), or VoicemeeterFancyOsd.exe if VoicemeeterFancyOsdHost.exe doesn't work for some reason or you are using older version of Windows.

Settings are stored in *PROGRAM_LOCATION\config\config.ini* (created on program launch if not exist). And are automatically validated and updated on file change, no need to restart.

# Upcoming features
Check the [Milestones](https://github.com/A-tG/VoicemeeterFancyOSD/milestones)

# What's the point?
When you change gain/routing by using Voicemeeter Macro Buttons (or maybe some other way), for example if some of your Button's configuration looks like this

![](https://i.imgur.com/M3mwHnY.png)

it's very convenient to see changes on little On-screen display instead of opening the Voicemeeter window.
In other words it works similar to Windows volume pop-up but for Voicemeeter so you can enjoy all good thing like volume/gain in decibels instead of unnatural 0-100 range.
For example in Voicemeeter when you change gain from 0 to -60 you will experience smooth volume transition from loud to quiet, but in Windows volume pop-up
100% is 0 in Voicemeeter, 50% is actually a equivalent to Voicemeeter's -10.0, 25% is around -20.0, 12.5% is -30.0 and so on - not very smooth transition.

So this program should be a good "replacement" of volume pop-up built in Windows, if you use Voicemeeter of course.


# Build instructions
[.NET 6.0](https://dotnet.microsoft.com/download) WPF project

Start/build in debug mode to get acces to Debug Window in tray context menu.

* Select Target Platform (x64 or x86, dont use AnyCpu or \*Host.exe will not be copied).
* Press "Rebuild Solution" to guarantee that up-to-date Host.exe and dependencies are copied to result folder.
* The program will be compiled in the **Solution's** folder.
* Launch \*Host.exe or ontop fullscreen functionality will not work.

# Uses
* [INI File Parser](https://github.com/rickyah/ini-parser)
* [Extended Voicemeeter Remote API wrapper](https://github.com/A-tG/voicemeeter-remote-api-extended)
* [WpfScreenHelper](https://github.com/micdenny/WpfScreenHelper)
* [Hardcodet NotifyIcon for WPF](https://github.com/hardcodet/wpf-notifyicon)
* [Octokit.net](https://github.com/octokit/octokit.net)
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

# Voicemeeter Fancy OSD
**WIP** Prototype. Voicemeeter API is not integrated yet! 

On-Screen display for Voicemeeter that work with fullscreen apps by using private Windows' API without hooking into Graphics API.

In order to show OSD on top of fullscreen apps Windows 10 is required. 
Support for other Windows versions can be added but without fullscreen apps support.

Based on modified code from [ModernFlyouts](https://github.com/ModernFlyouts-Community/ModernFlyouts) (Thanks for advices and directions from it's Discord server!)

# Build instructions
 * hostfxr.dll is required. You can publish the program as Self-contained and copy hostfxr.dll to output folder
 * Launch \*Host.exe or fullscreen ontop functionality will not work
 * app.manifest is crucial if you want to modify/make your own program based on this

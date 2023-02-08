Stuff you need to reference in your IDE : (Probably won't need all of them for simple modding, but it doesn't hurt to have them all ready.)
Rain World/BepInEx/core - Every .dll file
Rain World/BepInEx/utils - PUBLIC-Assembly-CSharp.dll
Rain World/BepInEx/plugins - HOOKS-Assembly-CSharp.dll
Rain World/RainWorld_Data/Managed - Assembly-CSharp-firstpass.dll, com.rlabrecque.steamworks.net.dll, 
UnityEngine.dll, UnityEngine.CoreModule.dll, UnityEngine.InputLegacyModule.dll, UnityEngine.ImageConversionModule.dll,
GoKit.dll, Newtonsoft.Json.dll


Throw your TemplateMod folder into:
Rain World/RainWorld_Data/StreamingAssets/mods


When writing mods, logging is your best friend.
Rain World/BepInEx/LogOutput.txt - for exceptions, loading fails and your own Logger.LogInfo/Logger.LogError messages.
Rain World/consoleLog.txt - for your own Debug.Log messages. You can access console log in-game by enabling Dev Tools and pressing 'K'
Rain World/exceptionLog.txt* - for exceptions; *Doesn't catch everything + obsolete if using a try-catch (look next step)

It's also a good idea to have a try-catch around RainWorld.Update while writing a mod - but instead of doing it for every mod (and later removing it when shipping it), you can use one ready for you :
https://steamcommunity.com/sharedfiles/filedetails/?id=2927326990

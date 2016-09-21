# simpleMemoryManager
Simple Memory Class. Includes a couple of Windows APIs and Handle/Reading/Writing  

You need to use the following namespaces
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

Example Usage
memoryFunctions mem = new memoryFunctions();
mem.setProcessName("csgo");
IntPtr handle = mem.getHandle();
IntPtr clientAddress = mem.getModuleAddress("client.dll");

windowsAPIs winAPI = new windowsAPIs();

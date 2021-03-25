using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NekoClient.Logging;
using UnityEngine;

namespace UnityParrotLite
{
    public class Main
    {
        public static void Initialize()
        {
            Log.Initialize(LogLevel.All);
            Log.AddListener(new ConsoleLogListener());
            Log.AddListener(new TraceLogListener());
            Log.AddListener(new FileLogListener(FileSystem.Configuration.GetFilePath("..\\UnityParrot.log"), true));

            Components.BookkeepPatches.Patch();
            Components.CreditPatches.Patch();
            Components.OperationManagerPatches.Patch();
            Components.TimerPatches.Patch();
            //Components.PacketPatches.Patch();

            new Thread(() =>
            {
                Thread.Sleep(300);

                GameObject mainObject = new GameObject();

                mainObject.AddComponent<Components.FPSPatches>();

                UnityEngine.Object.DontDestroyOnLoad(mainObject);

            }).Start();

            Components.SharedMemory.Initialize();
        }
    }
}

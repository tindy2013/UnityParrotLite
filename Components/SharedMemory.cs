using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Winterdom.IO.FileMap;

namespace UnityParrotLite.Components
{

    /*
     * Memory structure:
     * 0-14 mapped to JvsButtonID
     * 15-18 is the analog input
     * 19 is coin button
     * 20 is card button
     */
    
    class SharedMemory
    {
        public static MemoryMappedFile mappedFile;
        public static MapViewStream mapViewStream;

        public enum MapPosition
        {
            Function_Coin = 19,
            Function_Card
        }

        public static void Initialize()
        {
            try
            {
                mappedFile = MemoryMappedFile.Open(MapAccess.FileMapAllAccess, "Local\\BROKENGEKI_SHARED_BUFFER");
                NekoClient.Logging.Log.Info("Brokengeki shared memory exists, opening...");
            }
            catch (FileMapIOException)
            {
                NekoClient.Logging.Log.Info("Brokengeki shared memory does not exist, creating...");
                mappedFile = MemoryMappedFile.Create(MapProtection.PageReadWrite, 256, "Local\\BROKENGEKI_SHARED_BUFFER");
            }
            mapViewStream = (MapViewStream)mappedFile.MapView(MapAccess.FileMapAllAccess, 0, 256);
        }

        public static byte[] Read(int offset, int count)
        {
            byte[] b = new byte[count];
            if (mapViewStream != null)
            {
                mapViewStream.Read(offset, b, 0, count);
            }
            else
            {
                NekoClient.Logging.Log.Info("error when try to read memory: mapView is null");
            }
            return b;
        }

        public static void Write(byte[] buffer, int offset, int count)
        {
            if (mapViewStream != null)
                mapViewStream.Write(offset, buffer, 0, count);
        }
    }
}

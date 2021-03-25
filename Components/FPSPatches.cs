using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityParrotLite.Components
{
    class FPSPatches : MonoBehaviour
    {
        // Ongeki requires framerate to be capped at 60
        // or beatmap will be out of sync with music
        public int desiredFPS = 60;

        void Awake()
        {
            Screen.SetResolution(1080, 1920, true);
            Application.targetFrameRate = desiredFPS;
            QualitySettings.vSyncCount = 0;
        }

        void Update()
        {
            long lastTicks = DateTime.Now.Ticks;
            long currentTicks;
            float delay = 1f / desiredFPS;
            float elapsedTime;

            if (desiredFPS <= 0)
                return;

            while (true)
            {
                currentTicks = DateTime.Now.Ticks;
                elapsedTime = (float)TimeSpan.FromTicks(currentTicks - lastTicks).TotalSeconds;
                if (elapsedTime >= delay)
                {
                    break;
                }
            }
        }
    }
}

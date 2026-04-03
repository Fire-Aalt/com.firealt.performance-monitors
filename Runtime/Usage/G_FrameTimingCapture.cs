/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            02-Apr-26
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using UnityEngine;

namespace Tayx.Graphy.Usage
{
    internal static class G_FrameTimingCapture
    {
        private static readonly FrameTiming[] FrameTimings = new FrameTiming[1];

        private static int s_lastUpdatedFrame = -1;
        private static bool s_hasTimings;
        private static float s_cpuFrameTimeMs;
        private static float s_cpuRenderThreadFrameTimeMs;
        private static float s_gpuFrameTimeMs;

        public static bool TryGetLatestTimings(
            out float cpuFrameTimeMs,
            out float cpuRenderThreadFrameTimeMs,
            out float gpuFrameTimeMs)
        {
            UpdateCache();

            cpuFrameTimeMs = s_cpuFrameTimeMs;
            cpuRenderThreadFrameTimeMs = s_cpuRenderThreadFrameTimeMs;
            gpuFrameTimeMs = s_gpuFrameTimeMs;

            return s_hasTimings;
        }

        private static void UpdateCache()
        {
            var currentFrame = Time.frameCount;
            if (s_lastUpdatedFrame == currentFrame)
            {
                return;
            }

            s_lastUpdatedFrame = currentFrame;

            if (!FrameTimingManager.IsFeatureEnabled())
            {
                s_hasTimings = false;
                return;
            }

            FrameTimingManager.CaptureFrameTimings();
            s_hasTimings = FrameTimingManager.GetLatestTimings(1u, FrameTimings) > 0;

            if (!s_hasTimings)
            {
                return;
            }

            s_cpuFrameTimeMs = (float)FrameTimings[0].cpuFrameTime;
            s_cpuRenderThreadFrameTimeMs = (float)FrameTimings[0].cpuRenderThreadFrameTime;
            s_gpuFrameTimeMs = (float)FrameTimings[0].gpuFrameTime;
        }
    }
}

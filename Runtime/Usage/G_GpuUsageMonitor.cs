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
    public class G_GpuUsageMonitor : G_StatUsageMonitor
    {
        protected override bool TryGetUsage(out float usageMs)
        {
            if (G_FrameTimingCapture.TryGetLatestTimings(out _, out var cpuRenderThreadFrameTimeMs, out var gpuFrameTimeMs))
            {
                usageMs = Application.isMobilePlatform
                    ? cpuRenderThreadFrameTimeMs
                    : gpuFrameTimeMs;

                if (usageMs < AverageValue / 2f)
                {
                    return false;
                }
                
                return true;
            }

            usageMs = 0f;
            return false;
        }
    }
}

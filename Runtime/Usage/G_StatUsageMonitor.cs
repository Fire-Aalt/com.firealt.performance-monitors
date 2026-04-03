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

using Tayx.Graphy.Graph;

namespace Tayx.Graphy.Usage
{
    public abstract class G_StatUsageMonitor : G_StatMonitor
    {
        private const int UsageSamplesCapacity = 512;

        public float CurrentUsage => CurrentValue;

        public float AverageUsage { get; private set; }

        public float OnePercentHighestUsage { get; private set; }

        public float Zero1PercentHighestUsage { get; private set; }

        protected virtual void Awake()
        {
            InitializeMonitor(UsageSamplesCapacity);
            UpdateParameters();
        }

        public void UpdateMonitor(bool updateHistoricalStats)
        {
            if (!TryGetUsage(out var currentUsage))
            {
                currentUsage = SamplesCount > 0
                    ? CurrentUsage
                    : 0f;
            }

            AddSample(currentUsage, out _);

            AverageUsage = SamplesCount > 0
                ? AverageValue
                : currentUsage;

            if (!updateHistoricalStats)
            {
                OnePercentHighestUsage = currentUsage;
                Zero1PercentHighestUsage = currentUsage;
                return;
            }

            OnePercentHighestUsage = GetAverageOfHighestSamples(OnePercentSampleCount);
            Zero1PercentHighestUsage = GetAverageOfHighestSamples(Zero1PercentSampleCount);
        }

        public void UpdateParameters()
        {
            UpdatePercentileSampleWindows();
        }

        protected abstract bool TryGetUsage(out float usageMs);
    }
}

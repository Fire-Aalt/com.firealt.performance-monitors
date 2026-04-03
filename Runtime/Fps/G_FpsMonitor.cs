/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            15-Dec-17
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using UnityEngine;
using Tayx.Graphy.Graph;

namespace Tayx.Graphy.Fps
{
    public class G_FpsMonitor : G_StatMonitor
    {
        private const int FPSSamplesCapacity = 512;
        private const int MaxTrackedFps = short.MaxValue;

        private int[] _fpsSampleFrequencies;
        private int _lowestTrackedFps = MaxTrackedFps;
        private int _highestTrackedFps;

        public short CurrentFPS { get; private set; }
        public short AverageFPS { get; private set; }
        public short OnePercentFPS { get; private set; }
        public short Zero1PercentFps { get; private set; }

        private void Awake()
        {
            InitializeMonitor(FPSSamplesCapacity);

            _fpsSampleFrequencies = new int[MaxTrackedFps + 1];

            UpdateParameters();
        }

        public void UpdateMonitor(float unscaledDeltaTime, bool updateHistoricalStats)
        {
            if (unscaledDeltaTime <= 0f)
            {
                CurrentFPS = short.MaxValue;
            }
            else
            {
                CurrentFPS = (short)Mathf.Min(Mathf.RoundToInt(1f / unscaledDeltaTime), MaxTrackedFps);
            }

            if (!updateHistoricalStats)
            {
                AverageFPS = CurrentFPS;
                OnePercentFPS = CurrentFPS;
                Zero1PercentFps = CurrentFPS;
                return;
            }

            var replacedExistingSample = AddSample(CurrentFPS, out var removedSample);

            if (replacedExistingSample)
            {
                RemoveFrequency((int)removedSample);
            }

            AddFrequency(CurrentFPS);

            AverageFPS = SamplesCount > 0
                ? (short)AverageValue
                : (short)0;

            OnePercentFPS = GetAverageOfLowestSamples(OnePercentSampleCount, OnePercentSampleWindow);
            Zero1PercentFps = GetAverageOfLowestSamples(Zero1PercentSampleCount, Zero1PercentSampleWindow);
        }

        public void UpdateParameters()
        {
            UpdatePercentileSampleWindows();
        }

        private void AddFrequency(int fpsSample)
        {
            _fpsSampleFrequencies[fpsSample]++;

            if (fpsSample < _lowestTrackedFps)
            {
                _lowestTrackedFps = fpsSample;
            }

            if (fpsSample > _highestTrackedFps)
            {
                _highestTrackedFps = fpsSample;
            }
        }

        private void RemoveFrequency(int fpsSample)
        {
            _fpsSampleFrequencies[fpsSample]--;

            if (fpsSample == _lowestTrackedFps)
            {
                while (_lowestTrackedFps <= _highestTrackedFps && _fpsSampleFrequencies[_lowestTrackedFps] == 0)
                {
                    _lowestTrackedFps++;
                }
            }

            if (fpsSample == _highestTrackedFps)
            {
                while (_highestTrackedFps >= _lowestTrackedFps && _fpsSampleFrequencies[_highestTrackedFps] == 0)
                {
                    _highestTrackedFps--;
                }
            }
        }

        private short GetAverageOfLowestSamples(int sampleCount, int denominator)
        {
            if (sampleCount <= 0)
            {
                return 0;
            }

            var totalFps = 0L;
            var remainingSamples = sampleCount;

            for (var fps = _lowestTrackedFps; fps <= _highestTrackedFps && remainingSamples > 0; fps++)
            {
                var sampleFrequency = _fpsSampleFrequencies[fps];

                if (sampleFrequency == 0)
                {
                    continue;
                }

                var samplesToTake = sampleFrequency < remainingSamples
                    ? sampleFrequency
                    : remainingSamples;

                totalFps += (long)fps * samplesToTake;
                remainingSamples -= samplesToTake;
            }

            return (short)(totalFps / denominator);
        }
    }
}

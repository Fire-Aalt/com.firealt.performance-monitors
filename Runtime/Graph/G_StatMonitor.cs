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

using System;
using UnityEngine;

namespace Tayx.Graphy.Graph
{
    public abstract class G_StatMonitor : MonoBehaviour
    {
        private float[] _highestSamplesBuffer;
        private float[] _samples;
        private int _samplesCapacity;
        private int _samplesCount;
        private int _nextSampleIndex;
        private int _onePercentSamples;
        private float _runningSampleSum;
        private int _zero1PercentSamples;

        public float CurrentValue { get; private set; }

        public float AverageValue => _samplesCount > 0
            ? _runningSampleSum / _samplesCount
            : 0f;

        protected int SamplesCount => _samplesCount;

        protected int OnePercentSampleCount => GetTrackedHistoricalSampleCount(_onePercentSamples);

        protected int OnePercentSampleWindow => _onePercentSamples;

        protected int Zero1PercentSampleCount => GetTrackedHistoricalSampleCount(_zero1PercentSamples);

        protected int Zero1PercentSampleWindow => _zero1PercentSamples;

        protected void InitializeMonitor(int samplesCapacity)
        {
            _samplesCapacity = Mathf.Max(1, samplesCapacity);
            _highestSamplesBuffer = new float[_samplesCapacity];
            _samples = new float[_samplesCapacity];

            _samplesCount = 0;
            _nextSampleIndex = 0;
            _onePercentSamples = 1;
            _runningSampleSum = 0f;
            _zero1PercentSamples = 1;

            CurrentValue = 0f;

            UpdatePercentileSampleWindows();
        }

        protected bool AddSample(float sample, out float removedSample)
        {
            CurrentValue = sample;
            removedSample = 0f;

            var replacedExistingSample = _samplesCount == _samplesCapacity;

            if (replacedExistingSample)
            {
                removedSample = _samples[_nextSampleIndex];
                _runningSampleSum -= removedSample;
            }
            else
            {
                _samplesCount++;
            }

            _samples[_nextSampleIndex] = sample;
            _nextSampleIndex++;

            if (_nextSampleIndex >= _samplesCapacity)
            {
                _nextSampleIndex = 0;
            }

            _runningSampleSum += sample;

            return replacedExistingSample;
        }

        public void PopulateGraphValues(float[] destination)
        {
            if (destination == null || destination.Length == 0)
            {
                return;
            }

            Array.Clear(destination, 0, destination.Length);

            if (_samplesCount == 0)
            {
                return;
            }

            var samplesToCopy = Mathf.Min(destination.Length, _samplesCount);
            var sourceIndex = _nextSampleIndex - samplesToCopy;

            if (sourceIndex < 0)
            {
                sourceIndex += _samplesCapacity;
            }

            var destinationIndex = destination.Length - samplesToCopy;

            for (var i = 0; i < samplesToCopy; i++)
            {
                var sampleIndex = sourceIndex + i;

                if (sampleIndex >= _samplesCapacity)
                {
                    sampleIndex -= _samplesCapacity;
                }

                destination[destinationIndex + i] = _samples[sampleIndex];
            }
        }

        protected void UpdatePercentileSampleWindows()
        {
            _onePercentSamples = Mathf.Max(1, (int)(_samplesCapacity * 0.01f));
            _zero1PercentSamples = Mathf.Max(1, (int)(_samplesCapacity * 0.001f));
        }

        protected float GetAverageOfHighestSamples(int sampleCount)
        {
            if (sampleCount <= 0 || _samplesCount == 0 || _highestSamplesBuffer == null)
            {
                return 0f;
            }

            sampleCount = Mathf.Min(sampleCount, _samplesCount);

            var sourceIndex = _samplesCount == _samplesCapacity
                ? _nextSampleIndex
                : 0;

            var selectedCount = 0;

            for (var i = 0; i < _samplesCount; i++)
            {
                var sampleIndex = sourceIndex + i;

                if (sampleIndex >= _samplesCapacity)
                {
                    sampleIndex -= _samplesCapacity;
                }

                var sample = _samples[sampleIndex];

                if (selectedCount < sampleCount)
                {
                    InsertHighestSample(sample, selectedCount);
                    selectedCount++;
                    continue;
                }

                ReplaceSmallestHighestSample(sample, sampleCount);
            }

            var totalValue = 0f;

            for (var i = 0; i < selectedCount; i++)
            {
                totalValue += _highestSamplesBuffer[i];
            }

            return totalValue / selectedCount;
        }

        private int GetTrackedHistoricalSampleCount(int sampleCount)
        {
            return _samplesCount < sampleCount
                ? _samplesCount
                : sampleCount;
        }

        private void InsertHighestSample(float sample, int selectedCount)
        {
            var insertIndex = selectedCount;

            while (insertIndex > 0 && sample < _highestSamplesBuffer[insertIndex - 1])
            {
                _highestSamplesBuffer[insertIndex] = _highestSamplesBuffer[insertIndex - 1];
                insertIndex--;
            }

            _highestSamplesBuffer[insertIndex] = sample;
        }

        private void ReplaceSmallestHighestSample(float sample, int sampleCount)
        {
            if (sample <= _highestSamplesBuffer[0])
            {
                return;
            }

            var insertIndex = 0;

            while (insertIndex < sampleCount - 1 && _highestSamplesBuffer[insertIndex + 1] < sample)
            {
                _highestSamplesBuffer[insertIndex] = _highestSamplesBuffer[insertIndex + 1];
                insertIndex++;
            }

            _highestSamplesBuffer[insertIndex] = sample;
        }
    }
}

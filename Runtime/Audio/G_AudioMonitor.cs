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
using UnityEngine.SceneManagement;

namespace Tayx.Graphy.Audio
{
    public class G_AudioMonitor : MonoBehaviour
    {
        private const float ReferenceValue = 1f;

        private G_AudioModule _mAudioModule;
        private AudioListener m_audioListener;
        private G_AudioModule.LookForAudioListener m_findAudioListenerInCameraIfNull =
            G_AudioModule.LookForAudioListener.OnSceneLoad;
        private FFTWindow m_fftWindow = FFTWindow.Blackman;
        private int m_spectrumSize = 512;

        public float[] Spectrum { get; private set; }

        public float[] SpectrumHighestValues { get; private set; }

        public float MaxDB { get; private set; }

        public bool SpectrumDataAvailable => m_audioListener != null;

        private void Awake()
        {
            _mAudioModule = GetComponent<G_AudioModule>();
            UpdateParameters();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void UpdateMonitor(float deltaTime, bool updateSpectrumData)
        {
            if (m_audioListener != null)
            {
                AudioListener.GetOutputData(Spectrum, 0);

                var sum = 0f;
                for (var i = 0; i < Spectrum.Length; i++)
                {
                    sum += Spectrum[i] * Spectrum[i];
                }

                var rmsValue = Mathf.Sqrt(sum / Spectrum.Length);
                MaxDB = 20f * Mathf.Log10(rmsValue / ReferenceValue);
                if (MaxDB < -80f)
                {
                    MaxDB = -80f;
                }

                if (!updateSpectrumData)
                {
                    return;
                }

                AudioListener.GetSpectrumData(Spectrum, 0, m_fftWindow);

                for (var i = 0; i < Spectrum.Length; i++)
                {
                    if (Spectrum[i] > SpectrumHighestValues[i])
                    {
                        SpectrumHighestValues[i] = Spectrum[i];
                    }
                    else
                    {
                        SpectrumHighestValues[i] = Mathf.Clamp(
                            SpectrumHighestValues[i] - SpectrumHighestValues[i] * deltaTime * 2f,
                            0f,
                            1f);
                    }
                }
            }
            else if (m_findAudioListenerInCameraIfNull == G_AudioModule.LookForAudioListener.Always)
            {
                m_audioListener = FindAudioListener();
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void UpdateParameters()
        {
            m_findAudioListenerInCameraIfNull = _mAudioModule.AudioListenerSearchMode;
            m_audioListener = _mAudioModule.AudioListener;
            m_fftWindow = _mAudioModule.FftWindow;
            m_spectrumSize = _mAudioModule.SpectrumSize;

            if (m_audioListener == null &&
                m_findAudioListenerInCameraIfNull != G_AudioModule.LookForAudioListener.Never)
            {
                m_audioListener = FindAudioListener();
            }

            Spectrum = new float[m_spectrumSize];
            SpectrumHighestValues = new float[m_spectrumSize];
        }

        public float lin2dB(float linear)
        {
            return Mathf.Clamp(Mathf.Log10(linear) * 20f, -160f, 0f);
        }

        public float dBNormalized(float db)
        {
            return (db + 160f) / 160f;
        }

        private AudioListener FindAudioListener()
        {
            var mainCamera = Camera.main;

            if (mainCamera != null && mainCamera.TryGetComponent(out AudioListener audioListener))
            {
                return audioListener;
            }

            return null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (m_findAudioListenerInCameraIfNull == G_AudioModule.LookForAudioListener.OnSceneLoad)
            {
                m_audioListener = FindAudioListener();
            }
        }
    }
}

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

using Tayx.Graphy.Graph;
using UnityEngine;
using UnityEngine.UI;

namespace Tayx.Graphy.Ram
{
    public class G_RamGraph : G_Graph
    {
        [SerializeField] private Image m_imageAllocated;
        [SerializeField] private Image m_imageReserved;
        [SerializeField] private Image m_imageMono;
        [SerializeField] private Shader ShaderFull;
        [SerializeField] private Shader ShaderLight;

        private G_RamModule _mRamModule;
        private G_RamMonitor m_ramMonitor;
        private int m_resolution = 128;
        private G_GraphShader m_shaderGraphAllocated;
        private G_GraphShader m_shaderGraphReserved;
        private G_GraphShader m_shaderGraphMono;
        private float[] m_allocatedArray;
        private float[] m_reservedArray;
        private float[] m_monoArray;
        private float m_highestMemory;

        protected override void UpdateGraph()
        {
            var allocatedMemory = m_ramMonitor.AllocatedRam;
            var reservedMemory = m_ramMonitor.ReservedRam;
            var monoMemory = m_ramMonitor.MonoRam;

            for (var i = 0; i < m_resolution; i++)
            {
                if (i >= m_resolution - 1)
                {
                    m_allocatedArray[i] = allocatedMemory;
                    m_reservedArray[i] = reservedMemory;
                    m_monoArray[i] = monoMemory;
                }
                else
                {
                    m_allocatedArray[i] = m_allocatedArray[i + 1];
                    m_reservedArray[i] = m_reservedArray[i + 1];
                    m_monoArray[i] = m_monoArray[i + 1];
                }
            }

            UpdateShaderPointsFromSamples();
        }

        protected override void UpdateGraphParameters()
        {
            ConfigureShaderForMode(m_shaderGraphAllocated, _mRamModule.CurrentGraphyMode, ShaderFull, ShaderLight);
            ConfigureShaderForMode(m_shaderGraphReserved, _mRamModule.CurrentGraphyMode, ShaderFull, ShaderLight);
            ConfigureShaderForMode(m_shaderGraphMono, _mRamModule.CurrentGraphyMode, ShaderFull, ShaderLight);
            m_resolution = m_shaderGraphAllocated.GetArraySize(_mRamModule.RamGraphResolution);
            CreatePoints();
        }

        protected override void CreatePoints()
        {
            m_allocatedArray = ResizeBufferPreservingLatest(m_allocatedArray);
            m_reservedArray = ResizeBufferPreservingLatest(m_reservedArray);
            m_monoArray = ResizeBufferPreservingLatest(m_monoArray);

            ResetShaderValues(m_shaderGraphAllocated, m_resolution);
            ResetShaderValues(m_shaderGraphReserved, m_resolution);
            ResetShaderValues(m_shaderGraphMono, m_resolution);

            ApplyShaderStyle(
                m_shaderGraphAllocated,
                _mRamModule.AllocatedRamColor,
                _mRamModule.AllocatedRamColor,
                _mRamModule.AllocatedRamColor);

            ApplyShaderStyle(
                m_shaderGraphReserved,
                _mRamModule.ReservedRamColor,
                _mRamModule.ReservedRamColor,
                _mRamModule.ReservedRamColor);

            ApplyShaderStyle(
                m_shaderGraphMono,
                _mRamModule.MonoRamColor,
                _mRamModule.MonoRamColor,
                _mRamModule.MonoRamColor);

            UpdateShaderPointsFromSamples();
        }

        protected override void InitializeGraph()
        {
            _mRamModule = GetComponent<G_RamModule>();
            m_ramMonitor = GetComponent<G_RamMonitor>();

            m_shaderGraphAllocated = new G_GraphShader { Image = m_imageAllocated };
            m_shaderGraphReserved = new G_GraphShader { Image = m_imageReserved };
            m_shaderGraphMono = new G_GraphShader { Image = m_imageMono };
        }

        private float[] ResizeBufferPreservingLatest(float[] source)
        {
            if (source != null && source.Length == m_resolution)
            {
                return source;
            }

            var resized = new float[m_resolution];

            if (source == null || source.Length == 0)
            {
                return resized;
            }

            var samplesToCopy = Mathf.Min(source.Length, m_resolution);
            System.Array.Copy(source, source.Length - samplesToCopy, resized, m_resolution - samplesToCopy, samplesToCopy);
            return resized;
        }

        private void UpdateShaderPointsFromSamples()
        {
            m_highestMemory = 0f;

            for (var i = 0; i < m_resolution; i++)
            {
                if (m_highestMemory < m_reservedArray[i])
                {
                    m_highestMemory = m_reservedArray[i];
                }
            }

            m_highestMemory = Mathf.Max(1f, m_highestMemory);

            for (var i = 0; i < m_resolution; i++)
            {
                m_shaderGraphAllocated.ShaderArrayValues[i] = m_allocatedArray[i] / m_highestMemory;
                m_shaderGraphReserved.ShaderArrayValues[i] = m_reservedArray[i] / m_highestMemory;
                m_shaderGraphMono.ShaderArrayValues[i] = m_monoArray[i] / m_highestMemory;
            }

            m_shaderGraphAllocated.UpdatePoints();
            m_shaderGraphReserved.UpdatePoints();
            m_shaderGraphMono.UpdatePoints();
        }
    }
}

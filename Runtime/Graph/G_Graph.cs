/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            23-Jan-18
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
    public abstract class G_Graph : MonoBehaviour
    {
        private bool m_isInitialized;

        public bool IsInitialized => m_isInitialized;

        public void EnsureInitialized()
        {
            if (m_isInitialized)
            {
                return;
            }

            InitializeGraph();
            UpdateGraphParameters();
            m_isInitialized = true;
        }

        public void UpdateGraphModule()
        {
            if (!m_isInitialized)
            {
                return;
            }

            UpdateGraph();
        }

        public void UpdateParameters()
        {
            if (!m_isInitialized)
            {
                return;
            }

            UpdateGraphParameters();
        }

        #region Methods -> Protected

        protected abstract void InitializeGraph();

        /// <summary>
        /// Updates the graph/s.
        /// </summary>
        protected abstract void UpdateGraph();

        protected abstract void UpdateGraphParameters();

        /// <summary>
        /// Creates the points for the graph/s.
        /// </summary>
        protected abstract void CreatePoints();

        protected static void ConfigureShaderForMode(
            G_GraphShader shader,
            GraphyMode graphyMode,
            Shader shaderFull,
            Shader shaderLight)
        {
            switch (graphyMode)
            {
                case GraphyMode.Full:
                    shader.ArrayMaxSize = G_GraphShader.ArrayMaxSizeFull;
                    shader.Image.material = new Material(shaderFull);
                    break;

                case GraphyMode.Light:
                    shader.ArrayMaxSize = G_GraphShader.ArrayMaxSizeLight;
                    shader.Image.material = new Material(shaderLight);
                    break;
            }

            shader.InitializeShader();
        }

        protected static void ResetShaderValues(G_GraphShader shader, int resolution)
        {
            if (shader.ShaderArrayValues == null || shader.ShaderArrayValues.Length != resolution)
            {
                shader.ShaderArrayValues = new float[resolution];
            }

            Array.Clear(shader.ShaderArrayValues, 0, resolution);
        }

        protected static void ApplyShaderStyle(
            G_GraphShader shader,
            Color goodColor,
            Color cautionColor,
            Color criticalColor,
            float goodThreshold = 0f,
            float cautionThreshold = 0f,
            float average = 0f)
        {
            shader.GoodColor = goodColor;
            shader.CautionColor = cautionColor;
            shader.CriticalColor = criticalColor;
            shader.UpdateColors();

            shader.GoodThreshold = goodThreshold;
            shader.CautionThreshold = cautionThreshold;
            shader.UpdateThresholds();

            shader.UpdateArrayValuesLength();

            shader.Average = average;
            shader.UpdateAverage();
        }

        #endregion
    }
}

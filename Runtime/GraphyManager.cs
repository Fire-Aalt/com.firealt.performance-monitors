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

using System;
using System.Collections.Generic;
using Tayx.Graphy.UI;
using Tayx.Graphy.Utils;
using Tayx.Graphy.Utils.NumString;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tayx.Graphy
{
    [Serializable]
    public class ModulePresetEntry
    {
        public string ModuleId = string.Empty;
        public ModuleState State = ModuleState.Full;
    }

    [Serializable]
    public class ModulePresetDefinition
    {
        public string Name = "Preset";
        public bool DisableUnspecifiedModules = true;
        public List<ModulePresetEntry> Modules = new();
    }

    public class GraphyManager : G_Singleton<GraphyManager>
    {
        [Header("Global")]
        [SerializeField] private GraphyMode m_graphyMode = GraphyMode.Full;
        [SerializeField] private bool m_enableOnStartup = true;
        [SerializeField] private bool m_keepAlive = true;
        [SerializeField] private bool m_background = true;
        [SerializeField] private Color m_backgroundColor = new(0f, 0f, 0f, 0.3f);

        [Header("Input Actions")]
        public InputAction TogglePresetAction;
        public InputAction ToggleActiveAction;

        [Header("Presets")]
        [SerializeField] private int m_activePresetIndex;
        [SerializeField] private List<ModulePresetDefinition> m_modulePresets = new();

        private readonly List<G_Module> m_modules = new();
        private readonly Dictionary<string, G_Module> m_modulesById = new(StringComparer.OrdinalIgnoreCase);

        private bool m_initialized;
        private bool m_active = true;
        private bool m_focused = true;

        protected GraphyManager()
        {
        }

        public GraphyMode GraphyMode
        {
            get => m_graphyMode;
            set
            {
                m_graphyMode = value;
                UpdateAllParameters();
            }
        }

        public bool EnableOnStartup => m_enableOnStartup;

        public bool KeepAlive => m_keepAlive;

        public bool Background
        {
            get => m_background;
            set
            {
                m_background = value;
                UpdateAllParameters();
            }
        }

        public Color BackgroundColor
        {
            get => m_backgroundColor;
            set
            {
                m_backgroundColor = value;
                UpdateAllParameters();
            }
        }

        public IReadOnlyList<G_Module> Modules => m_modules;

        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            RegisterInputActions();
        }

        private void OnDisable()
        {
            UnregisterInputActions();
        }

        private void OnDestroy()
        {
            UnregisterInputActions();
            G_IntString.Dispose();
            G_FloatString.Dispose();
        }

        private void OnApplicationFocus(bool isFocused)
        {
            m_focused = isFocused;

            if (m_initialized && isFocused)
            {
                RefreshAllParameters();
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying || !m_initialized)
            {
                return;
            }

            DiscoverModules();
            UpdateAllParameters();
            ApplyConfiguredModuleStates();

            if (!m_active)
            {
                Disable();
            }
        }

        private void Update()
        {
            if (!m_initialized || !m_active)
            {
                return;
            }

            var deltaTime = Time.deltaTime;
            var unscaledDeltaTime = Time.unscaledDeltaTime;

            foreach (var module in m_modules)
            {
                if (module is IGraphyRuntimeModule runtimeModule)
                {
                    runtimeModule.UpdateModule(deltaTime, unscaledDeltaTime);
                }
            }
        }

        public bool TryGetModule(string moduleId, out G_Module module)
        {
            return m_modulesById.TryGetValue(moduleId, out module);
        }

        public T GetModule<T>() where T : G_Module
        {
            foreach (var module in m_modules)
            {
                if (module is T typedModule)
                {
                    return typedModule;
                }
            }

            return null;
        }

        public bool SetModulePosition(string moduleId, ModulePosition modulePosition, Vector2 moduleOffset)
        {
            if (!TryGetModule(moduleId, out var module))
            {
                return false;
            }

            module.SetPosition(modulePosition, moduleOffset);
            return true;
        }

        public bool SetModuleState(string moduleId, ModuleState moduleState)
        {
            if (!TryGetModule(moduleId, out var module))
            {
                return false;
            }

            module.SetState(moduleState);
            return true;
        }

        public void TogglePresets()
        {
            if (m_modulePresets.Count == 0 || !m_active)
            {
                return;
            }

            m_activePresetIndex++;
            if (m_activePresetIndex >= m_modulePresets.Count)
            {
                m_activePresetIndex = 0;
            }

            ApplyPreset(m_activePresetIndex);
        }

        public void ApplyPreset(int presetIndex)
        {
            if (presetIndex < 0 || presetIndex >= m_modulePresets.Count)
            {
                Debug.LogWarning("[GraphyManager]::ApplyPreset - Tried to set a preset index that is not supported.");
                return;
            }

            m_activePresetIndex = presetIndex;
            ApplyPreset(m_modulePresets[presetIndex]);
        }

        public void ApplyPreset(string presetName)
        {
            for (var i = 0; i < m_modulePresets.Count; i++)
            {
                if (string.Equals(m_modulePresets[i].Name, presetName, StringComparison.OrdinalIgnoreCase))
                {
                    ApplyPreset(i);
                    return;
                }
            }

            Debug.LogWarning("[GraphyManager]::ApplyPreset - Tried to set a preset that does not exist.");
        }

        public void ToggleActive()
        {
            if (m_active)
            {
                Disable();
            }
            else
            {
                Enable();
            }
        }

        public void Enable()
        {
            if (m_active)
            {
                return;
            }

            if (!m_initialized)
            {
                Init();
                return;
            }

            foreach (var module in m_modules)
            {
                module.RestorePreviousState();
            }

            m_active = true;
        }

        public void Disable()
        {
            if (!m_active)
            {
                return;
            }

            foreach (var module in m_modules)
            {
                module.SetState(ModuleState.Off);
            }

            m_active = false;
        }

        private void Init()
        {
            if (m_initialized)
            {
                return;
            }

            if (m_keepAlive)
            {
                DontDestroyOnLoad(transform.root.gameObject);
            }

            DiscoverModules();
            UpdateAllParameters();
            ApplyConfiguredModuleStates();
            ApplyPreset(m_activePresetIndex);

            if (!m_enableOnStartup)
            {
                ToggleActive();

                var canvas = GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.enabled = true;
                }
            }

            m_initialized = true;
        }

        private void DiscoverModules()
        {
            m_modules.Clear();
            m_modulesById.Clear();

            var modules = GetComponentsInChildren<G_Module>(true);
            foreach (var module in modules)
            {
                if (module == null)
                {
                    continue;
                }

                m_modules.Add(module);

                if (!m_modulesById.TryAdd(module.ModuleId, module))
                {
                    Debug.LogWarning($"[GraphyManager]::DiscoverModules - Duplicate module id '{module.ModuleId}' found.", module);
                }
            }
        }

        private void ApplyConfiguredModuleStates()
        {
            foreach (var module in m_modules)
            {
                module.ApplyConfiguration();
            }
        }

        private void ApplyPreset(ModulePresetDefinition preset)
        {
            if (preset == null)
            {
                return;
            }

            if (preset.DisableUnspecifiedModules)
            {
                foreach (var module in m_modules)
                {
                    module.SetState(ModuleState.Off);
                }
            }

            foreach (var entry in preset.Modules)
            {
                if (string.IsNullOrWhiteSpace(entry.ModuleId))
                {
                    continue;
                }

                if (m_modulesById.TryGetValue(entry.ModuleId, out var module))
                {
                    module.SetState(entry.State);
                }
                else
                {
                    Debug.LogWarning(
                        $"[GraphyManager]::ApplyPreset - Module '{entry.ModuleId}' was not found for preset '{preset.Name}'.");
                }
            }
        }

        private void UpdateAllParameters()
        {
            foreach (var module in m_modules)
            {
                module.UpdateParameters();
            }
        }

        private void RefreshAllParameters()
        {
            foreach (var module in m_modules)
            {
                module.RefreshParameters();
            }
        }

        private void RegisterInputActions()
        {
            if (TogglePresetAction != null)
            {
                TogglePresetAction.Enable();
                TogglePresetAction.performed -= OnTogglePresetPerformed;
                TogglePresetAction.performed += OnTogglePresetPerformed;
            }

            if (ToggleActiveAction != null)
            {
                ToggleActiveAction.Enable();
                ToggleActiveAction.performed -= OnToggleActivePerformed;
                ToggleActiveAction.performed += OnToggleActivePerformed;
            }
        }

        private void UnregisterInputActions()
        {
            if (TogglePresetAction != null)
            {
                TogglePresetAction.performed -= OnTogglePresetPerformed;
                TogglePresetAction.Disable();
            }

            if (ToggleActiveAction != null)
            {
                ToggleActiveAction.performed -= OnToggleActivePerformed;
                ToggleActiveAction.Disable();
            }
        }

        private void OnTogglePresetPerformed(InputAction.CallbackContext context)
        {
            if (m_focused)
            {
                TogglePresets();
            }
        }

        private void OnToggleActivePerformed(InputAction.CallbackContext context)
        {
            if (m_focused)
            {
                ToggleActive();
            }
        }
    }
}

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityLightsAuditTool.Editor
{
    public class UnityLightsAuditToolWindow : EditorWindow
    {
        public VisualTreeAsset rootVisualTreeAsset;

        private const LightSortingOption DefaultLightSortingMode = LightSortingOption.Range;
        private const bool DefaultSortIsAscending = false;
        
        private Light[] _lights;
        private Toggle _realtimeTg;
        private Toggle _mixedTg;
        private Toggle _bakedTg;
        private Button _resetParametersBt;
        private Button _auditSceneBt;
        private Button _clearAuditResultsBt;
        private ScrollView _resultsSv;
        private Button _sortByNameBt;
        private Button _sortByTypeBt;
        private Button _sortByModeBt;
        private Button _sortByRangeBt;
        private Button _sortByIntensityBt;
        private Button _sortByIndirectMultiplierBt;
        private Button _sortByShadowTypeBt;
        private Button _sortByRenderModeBt;
        private Button _sortByCullingMaskBt;

        private LightSortingOption _currentLightSortingMode;
        private LightSortingOption _previousLightSortingMode;
        private bool _isSortAscending;

        [MenuItem("Raz's Tools/Lights Audit Tool")]
        public static void ShowWindow()
        {
            var window = GetWindow<UnityLightsAuditToolWindow>();
            window.titleContent = new GUIContent("Lights Audit Tool");
            window.minSize = new Vector2(1300, 600);
            window.Show();
        }

        public void OnEnable()
        {
            // Load and clone the window structure
            rootVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UnityLightsAuditTool/Editor/Resources/UnityLightsAuditToolRoot.uxml");
            if (rootVisualTreeAsset == null)
            {
                rootVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.razluta.unitylightsaudittool/Editor/Resources/UnityLightsAuditToolRoot.uxml");
            }
            rootVisualTreeAsset.CloneTree(rootVisualElement);
            
            // Find / query elements
            _realtimeTg = rootVisualElement.Q<Toggle>("Tg_Realtime");
            _mixedTg = rootVisualElement.Q<Toggle>("Tg_Mixed");
            _bakedTg = rootVisualElement.Q<Toggle>("Tg_Baked");
            _resetParametersBt = rootVisualElement.Q<Button>("Bt_ResetParameters");
            _auditSceneBt = rootVisualElement.Q<Button>("Bt_AuditScene");
            _clearAuditResultsBt = rootVisualElement.Q<Button>("Bt_ClearResults");
            _resultsSv = rootVisualElement.Q<ScrollView>("Sv_Results");
            _sortByNameBt = rootVisualElement.Q<Button>("Bt_TableColumn_Name");
            _sortByTypeBt = rootVisualElement.Q<Button>("Bt_TableColumn_Type");
            _sortByModeBt = rootVisualElement.Q<Button>("Bt_TableColumn_Mode");
            _sortByRangeBt = rootVisualElement.Q<Button>("Bt_TableColumn_Range");
            _sortByIntensityBt = rootVisualElement.Q<Button>("Bt_TableColumn_Intensity");
            _sortByIndirectMultiplierBt = rootVisualElement.Q<Button>("Bt_TableColumn_IndirectMultiplier");
            _sortByShadowTypeBt = rootVisualElement.Q<Button>("Bt_TableColumn_ShadowType");
            _sortByRenderModeBt = rootVisualElement.Q<Button>("Bt_TableColumn_RenderMode");
            _sortByCullingMaskBt = rootVisualElement.Q<Button>("Bt_TableColumn_CullingMask");
            
            // Define behaviour
            _resetParametersBt.clickable.clicked += () => ResetParameters();
            _auditSceneBt.clickable.clicked += () => AuditScene();
            _clearAuditResultsBt.clickable.clicked += () => ClearResultsVisualElement();
            _sortByNameBt.clickable.clicked += () => ForceUpdateUiAfterSort(LightSortingOption.Name);
            _sortByTypeBt.clickable.clicked += () => ForceUpdateUiAfterSort(LightSortingOption.Type);
            _sortByModeBt.clickable.clicked += () => ForceUpdateUiAfterSort(LightSortingOption.Mode);
            _sortByRangeBt.clickable.clicked += () => ForceUpdateUiAfterSort(LightSortingOption.Range);
            _sortByIntensityBt.clickable.clicked += () => ForceUpdateUiAfterSort(LightSortingOption.Intensity);
            _sortByIndirectMultiplierBt.clickable.clicked += () => ForceUpdateUiAfterSort(LightSortingOption.IndirectMultiplier);
            _sortByShadowTypeBt.clickable.clicked += () => ForceUpdateUiAfterSort(LightSortingOption.ShadowType);
            _sortByRenderModeBt.clickable.clicked += () => ForceUpdateUiAfterSort(LightSortingOption.RenderMode);
            _sortByCullingMaskBt.clickable.clicked += () => ForceUpdateUiAfterSort(LightSortingOption.CullingMask);
            
            // Set defaults
            _currentLightSortingMode = DefaultLightSortingMode;
            _isSortAscending = DefaultSortIsAscending;
        }

        private void ResetParameters()
        {
            LightAuditorParameters lightAuditorParameters = new LightAuditorParameters();
            lightAuditorParameters.Reset();
            _realtimeTg.value = lightAuditorParameters.IsRealtime;
            _mixedTg.value = lightAuditorParameters.IsMixed;
            _bakedTg.value = lightAuditorParameters.IsBaked;
        }

        private void AuditScene()
        {
            // Collect UI information
            LightAuditorParameters lightAuditorParameters = new LightAuditorParameters();
            lightAuditorParameters.Reset();
            lightAuditorParameters.IsRealtime = _realtimeTg.value;
            lightAuditorParameters.IsMixed = _mixedTg.value;
            lightAuditorParameters.IsBaked = _bakedTg.value;
            
            // Clear dynamic pieces of theUI
            ClearResultsVisualElement();
            
            // Update data and repopulate UI
            PopulateUiWithData(lightAuditorParameters);
        }

        private void ClearResultsVisualElement()
        {
            _resultsSv.Clear();
        }
        
        private void PopulateUiWithData(LightAuditorParameters lightAuditorParameters)
        {
            var allLights = UnityLightsAuditTool.Editor.LightUtils.GetAllLights();
            var validLights = new List<Light>();
            foreach (var light in allLights)
            {
                if ((lightAuditorParameters.IsRealtime && light.lightmapBakeType == LightmapBakeType.Realtime) || 
                    (lightAuditorParameters.IsMixed && light.lightmapBakeType == LightmapBakeType.Mixed) || 
                    (lightAuditorParameters.IsBaked && light.lightmapBakeType == LightmapBakeType.Baked)) 
                    validLights.Add(light);
            }
            
            validLights = GetSortedLights(validLights, _currentLightSortingMode, _isSortAscending);
            _lights = validLights.ToArray();

            VisualTreeAsset rowAuditResult = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UnityLightsAuditTool/Editor/Resources/Ve_AuditResult.uxml");
            if (rowAuditResult == null)
            {
                rowAuditResult = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.razluta.unitylightsaudittool/Editor/Resources/Ve_AuditResult.uxml");
            }
            foreach (var light in _lights)
            {
                // Clone the visual element
                var rowAuditResultVe = rowAuditResult.CloneTree();

                // Query the visual element pieces
                var auditResultBt = rowAuditResultVe.Q<Button>("Bt_AuditResult");
                var lightNameLb = rowAuditResultVe.Q<Label>("Lb_Ar_Name");
                var lightTypeLb = rowAuditResultVe.Q<Label>("Lb_Ar_Type");
                var lightBakeModeLb = rowAuditResultVe.Q<Label>("Lb_Ar_Mode");
                var lightRangeLb = rowAuditResultVe.Q<Label>("Lb_Ar_Range");
                var lightIntensityLb = rowAuditResultVe.Q<Label>("Lb_Ar_Intensity");
                var lightIndirectMultLb = rowAuditResultVe.Q<Label>("Lb_Ar_IndirectMultiplier");
                var lightShadowTypeLb = rowAuditResultVe.Q<Label>("Lb_Ar_ShadowType");
                var lightRenderModeLb = rowAuditResultVe.Q<Label>("Lb_Ar_RenderMode");
                var lightCullingMaskLb = rowAuditResultVe.Q<Label>("Lb_Ar_CullingMask");

                // Define behavior
                auditResultBt.clickable.clicked += () => EditorGUIUtility.PingObject(light);
                
                // Update the row based on the light info
                lightNameLb.text = light.name;
                lightTypeLb.text = light.type.ToString();
                lightBakeModeLb.text = light.lightmapBakeType.ToString();
                lightRangeLb.text = light.range.ToString(CultureInfo.InvariantCulture);
                lightIntensityLb.text = light.intensity.ToString(CultureInfo.InvariantCulture);
                lightIndirectMultLb.text = light.bounceIntensity.ToString(CultureInfo.InvariantCulture);
                lightShadowTypeLb.text = light.shadows.ToString();
                lightRenderModeLb.text = light.renderMode.ToString();
                lightCullingMaskLb.text = light.cullingMask.ToString();

                // Add the row to the container
                _resultsSv.contentContainer.Add(rowAuditResultVe);
            }
        }

        private void ForceUpdateUiAfterSort(LightSortingOption lightSortingOption)
        {
            SetLightSortingMode(lightSortingOption);
            AuditScene();
        }

        private List<Light> GetSortedLights(List<Light> lights, LightSortingOption sortingMode, bool isSortAscending)
        {
            switch (sortingMode)
            {
                case LightSortingOption.Name:
                {
                    var orderedList = lights.OrderBy(light => light.name);
                    lights = new List<Light>(orderedList);
                }
                    break;

                case LightSortingOption.Type:
                {
                    var orderedList = lights.OrderBy(light => light.type);
                    lights = new List<Light>(orderedList);
                }
                    break;
                
                case LightSortingOption.Mode:
                {
                    var orderedList = lights.OrderBy(light => light.lightmapBakeType);
                    lights = new List<Light>(orderedList);
                }
                    break;
                
                case LightSortingOption.Range:
                {
                    var orderedList = lights.OrderBy(light => light.range);
                    lights = new List<Light>(orderedList);
                }
                    break;
                
                case LightSortingOption.Intensity:
                {
                    var orderedList = lights.OrderBy(light => light.intensity);
                    lights = new List<Light>(orderedList);
                }
                    break;
                
                case LightSortingOption.IndirectMultiplier:
                {
                    var orderedList = lights.OrderBy(light => light.bounceIntensity);
                    lights = new List<Light>(orderedList);
                }
                    break;
                
                case LightSortingOption.ShadowType:
                {
                    var orderedList = lights.OrderBy(light => light.shadows);
                    lights = new List<Light>(orderedList);
                }
                    break;
                
                case LightSortingOption.RenderMode:
                {
                    var orderedList = lights.OrderBy(light => light.renderMode);
                    lights = new List<Light>(orderedList);
                }
                    break;
                
                case LightSortingOption.CullingMask:
                {
                    var orderedList = lights.OrderBy(light => light.cullingMask);
                    lights = new List<Light>(orderedList);
                }
                    break;
            }

            if (!isSortAscending)
            {
                lights.Reverse();
            }
            
            return lights;
        }

        private void SetLightSortingMode(LightSortingOption lightSortingOption)
        {
            if (_previousLightSortingMode == _currentLightSortingMode)
            {
                _isSortAscending = !_isSortAscending;
            }
            else
            {
                _isSortAscending = true;
            }
            
            _previousLightSortingMode = _currentLightSortingMode;
            _currentLightSortingMode = lightSortingOption;
        }
    }
}

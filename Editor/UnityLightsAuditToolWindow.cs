using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityLightsAuditTool.Editor
{
    public class UnityLightsAuditToolWindow : EditorWindow
    {
        public VisualTreeAsset rootVisualTreeAsset;

        private Light[] _lights;
        private Toggle _realtimeTg;
        private Toggle _mixedTg;
        private Toggle _bakedTg;
        private Button _resetParametersBt;
        private Button _auditSceneBt;
        private Button _clearAuditResultsBt;
        private ScrollView _resultsSv;

        [MenuItem("Raz's Tools/Lights Audit Tool")]
        public static void ShowWindow()
        {
            var window = GetWindow<UnityLightsAuditToolWindow>();
            window.titleContent = new GUIContent("Lights Audit Tool");
            window.minSize = new Vector2(1000, 600);
            window.Show();
        }

        public void OnEnable()
        {
            // Load and clone the window structure
            rootVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UnityLightsAuditTool/Editor/Resources/UnityLightsAuditToolRoot.uxml");
            rootVisualTreeAsset.CloneTree(rootVisualElement);
            
            // Find / query elements
            _realtimeTg = rootVisualElement.Q<Toggle>("Tg_Realtime");
            _mixedTg = rootVisualElement.Q<Toggle>("Tg_Mixed");
            _bakedTg = rootVisualElement.Q<Toggle>("Tg_Baked");
            _resetParametersBt = rootVisualElement.Q<Button>("Bt_ResetParameters");
            _auditSceneBt = rootVisualElement.Q<Button>("Bt_AuditScene");
            _clearAuditResultsBt = rootVisualElement.Q<Button>("Bt_ClearResults");
            _resultsSv = rootVisualElement.Q<ScrollView>("Sv_Results");
            
            // Define behaviour
            // _resetParametersBt.clickable.clicked += () => Something();
            _auditSceneBt.clickable.clicked += () => AuditScene();
            _clearAuditResultsBt.clickable.clicked += () => ClearResultsVisualElement();
        }

        private void AuditScene()
        {
            // Collect UI information
            LightAuditorParameters lightAuditorParameters = new LightAuditorParameters();
            lightAuditorParameters.Reset();
            // lightAuditorParameters.HumanoidHeight = 
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

            _lights = validLights.ToArray();

            VisualTreeAsset rowAuditResult = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UnityLightsAuditTool/Editor/Resources/Ve_AuditResult.uxml");
            foreach (var light in _lights)
            {
                // Clone the visual element
                var rowAuditResultVe = rowAuditResult.CloneTree();

                // Query the visual element pieces
                var auditResultBt = rowAuditResultVe.Q<Button>("Bt_AuditResult");
                var lightNameLb = rowAuditResultVe.Q<Label>("Lb_Ar_Name");

                // Define behavior
                auditResultBt.clickable.clicked += () => EditorGUIUtility.PingObject(light);
                
                // Update the row based on the light info
                lightNameLb.text = light.name;

                // Add the row to the container
                _resultsSv.contentContainer.Add(rowAuditResultVe);
            }
        }
    }
}

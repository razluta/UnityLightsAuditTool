#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace razluta
{
    public class UnityLightsAuditToolWindow : EditorWindow
    {
        public VisualTreeAsset rootVisualTreeAsset;

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
            var root = rootVisualElement;
            rootVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UnityLightsAuditTool/Editor/Resources/UnityLightsAuditToolRoot.uxml");
            rootVisualTreeAsset.CloneTree(root);
            
            // Create a table
            
        }
    }
}
#endif

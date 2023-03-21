using UnityEngine;

namespace UnityLightsAuditTool.Editor
{
    public static class LightUtils
    {
        public static Light[] GetAllLights()
        {
            return UnityEngine.Object.FindObjectsOfType<Light>();
        }
    }

    public struct LightAuditorParameters
    {
        public bool IsRealtime;
        public bool IsMixed;
        public bool IsBaked;
        
        private const bool IsRealtimeDefault = true;
        private const bool IsMixedDefault = false;
        private const bool IsBakedDefault = false;

        public void Reset()
        {
            IsRealtime = IsRealtimeDefault;
            IsMixed = IsMixedDefault;
            IsBaked = IsBakedDefault;
        }
    }
}
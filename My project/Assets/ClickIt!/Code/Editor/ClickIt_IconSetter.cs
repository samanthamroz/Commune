using UnityEditor;
using UnityEngine;

namespace ClickIt.Editor {
    [InitializeOnLoad]
    internal class ClickItIconSetter {
        private static readonly string SessionKey = "ClickIt_IconsSet";
        private static readonly string[] componentScriptNames = {
            "ClickableObject",
            "ReleaseableObject",
            "ClickAwayObject"
        };

        static ClickItIconSetter() {
            if (SessionState.GetBool(SessionKey, false)) return;
            EditorApplication.delayCall += SetIcons;
        }

        private static void SetIcons() {
            if (SessionState.GetBool(SessionKey, false)) return;

            Texture2D icon = FindAsset<Texture2D>("ClickItIcon");
            if (icon == null) {
                Debug.LogWarning("ClickIt: Could not find ClickItIcon texture.");
                return;
            }

            foreach (var scriptName in componentScriptNames) {
                MonoScript script = FindAsset<MonoScript>(scriptName);
                if (script == null) {
                    Debug.LogWarning($"ClickIt: Could not find component script: {scriptName}");
                    continue;
                }

                MonoImporter importer = (MonoImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(script));
                if (importer.GetIcon() != icon) {
                    importer.SetIcon(icon);
                    importer.SaveAndReimport();
                }
            }

            SessionState.SetBool(SessionKey, true);
        }

        private static T FindAsset<T>(string name) where T : Object {
            string typeFilter = typeof(T) == typeof(MonoScript) ? "t:MonoScript" : $"t:{typeof(T).Name}";
            var guids = AssetDatabase.FindAssets($"{name} {typeFilter}");
            if (guids.Length == 0) return null;

            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
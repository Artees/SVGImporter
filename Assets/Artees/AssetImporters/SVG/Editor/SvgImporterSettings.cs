using System.IO;
using UnityEditor;
using UnityEngine;

namespace Artees.AssetImporters.SVG.Editor
{
    internal class SvgImporterSettings : ScriptableObject
    {
        public static SvgImporterSettings Load()
        {
            const string path = "Assets/Resources/SvgImporterSettings.asset";
            var settings = AssetDatabase.LoadAssetAtPath<SvgImporterSettings>(path);
            if (settings != null) return settings;
            settings = CreateInstance<SvgImporterSettings>();
            Directory.CreateDirectory("Assets/Resources");
            AssetDatabase.CreateAsset(settings, path);
            return settings;
        }
        
        [SerializeField] public string InkscapeExecutable =
#if UNITY_EDITOR_OSX
            "/Applications/Inkscape.app/Contents/Resources/bin/inkscape";
#else
            "inkscape";
#endif
    }
}
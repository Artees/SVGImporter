using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Artees.SVGImporter.Editor
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

        [FormerlySerializedAs("InkscapeExecutable"), SerializeField]
        private string inkscapeExecutable =
#if UNITY_EDITOR_OSX
            "/Applications/Inkscape.app/Contents/Resources/bin/inkscape";
#else
            "inkscape";
#endif

        [FormerlySerializedAs("DefaultPixelDataStorage"), SerializeField]
        private SvgPixelDataStorage defaultPixelDataStorage = SvgPixelDataStorage.Metadata;

        public string InkscapeExecutable
        {
            get { return inkscapeExecutable; }
            set { inkscapeExecutable = value; }
        }

        public SvgPixelDataStorage DefaultPixelDataStorage
        {
            get { return defaultPixelDataStorage; }
            set { defaultPixelDataStorage = value; }
        }
    }
}
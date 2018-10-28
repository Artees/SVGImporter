using UnityEditor;
using UnityEngine;

namespace Artees.AssetImporters.SVG.Editor
{
    internal class SvgImporterWindow : EditorWindow
    {
        [MenuItem("Window/Artees/SVG Importer")]
        public static void ShowWindow()
        {
            var window = GetWindow<SvgImporterWindow>(true, "SVG Importer");
            window.Show();
        }

        private SvgImporterSettings _settings;

        private void Awake()
        {
            _settings = SvgImporterSettings.Load();
            minSize = new Vector2(300f, 150f);
        }

        private void OnGUI()
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Download Inkscape"))
            {
                Application.OpenURL("https://inkscape.org/");
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("Inkscape executable:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            var minWidth = GUILayout.MinWidth(10f);
            _settings.InkscapeExecutable =
                GUILayout.TextField(_settings.InkscapeExecutable, minWidth);
            var maxWidth = GUILayout.MaxWidth(100f);
            if (GUILayout.Button("Browse", maxWidth))
            {
                _settings.InkscapeExecutable = EditorUtility.OpenFilePanel("Inkscape executable",
                    _settings.InkscapeExecutable, "");
            }

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Default pixel data storage:", EditorStyles.boldLabel);
            var metadata = _settings.DefaultPixelDataStorage == SvgPixelDataStorage.Metadata;
            metadata = EditorGUILayout.Toggle("Metadata", metadata, EditorStyles.radioButton);
            metadata = !EditorGUILayout.Toggle("Png", !metadata, EditorStyles.radioButton);
            _settings.DefaultPixelDataStorage =
                metadata ? SvgPixelDataStorage.Metadata : SvgPixelDataStorage.Png;
            GUILayout.FlexibleSpace();
        }

        private void OnDestroy()
        {
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
        }
    }
}
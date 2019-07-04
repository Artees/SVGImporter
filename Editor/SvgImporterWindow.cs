using UnityEditor;
using UnityEngine;

namespace Artees.SVGImporter.Editor
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
            position = new Rect(position.position, minSize);
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
            var metadata = CreateToggleGroup("Default pixel data storage:",
                _settings.DefaultPixelDataStorage == SvgPixelDataStorage.Metadata,
                "Metadata",
                "Png");
            _settings.DefaultPixelDataStorage =
                metadata ? SvgPixelDataStorage.Metadata : SvgPixelDataStorage.Png;
            GUILayout.FlexibleSpace();
        }

        private static bool CreateToggleGroup(string header, bool value, string labelTrue, string labelFalse)
        {
            GUILayout.Label(header, EditorStyles.boldLabel);
            var result = EditorGUILayout.Toggle(labelTrue, value, EditorStyles.radioButton);
            result = !EditorGUILayout.Toggle(labelFalse, !result, EditorStyles.radioButton);
            return result;
        }

        private void OnDestroy()
        {
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
        }
    }
}
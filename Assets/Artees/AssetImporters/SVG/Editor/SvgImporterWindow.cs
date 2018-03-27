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
            minSize = new Vector2(300f, 100f);
        }

        private void OnGUI()
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Download Inkscape"))
            {
                Application.OpenURL("https://inkscape.org/");
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("Inkscape executable:");
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
        }

        private void OnDestroy()
        {
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
        }
    }
}
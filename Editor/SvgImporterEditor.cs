using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Artees.SVGImporter.Editor
{
    [CustomEditor(typeof(SvgImporter))]
    public class SvgImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Pixel Data Storage");
            var importer = (SvgImporter) target;
            var enumPopup = 
                (SvgPixelDataStorage) EditorGUILayout.EnumPopup(importer.PixelDataStorage);
            if (importer.PixelDataStorage != enumPopup)
            {
                importer.PixelDataStorage = enumPopup;
                EditorUtility.SetDirty(importer);
            }
            GUILayout.EndHorizontal();
        }
    }
}
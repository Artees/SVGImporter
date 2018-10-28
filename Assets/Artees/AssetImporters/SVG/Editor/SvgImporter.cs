using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Artees.AssetImporters.SVG.Editor
{
    [Serializable, ScriptedImporter(2, "svg")]
    // ReSharper disable once UnusedMember.Global
    internal class SvgImporter : ScriptedImporter
    {
        [SerializeField, HideInInspector] public SvgPixelDataStorage PixelDataStorage;

        [SerializeField, HideInInspector] private string _data;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var settings = SvgImporterSettings.Load();
            var texture = new Texture2D(4, 4) {alphaIsTransparency = true};
            const string svgExt = ".svg";
            var relativeSvgPath = ctx.assetPath;
            var relativePngPath = relativeSvgPath.Replace(svgExt, ".png");
            const string assetsDirectory = "Assets";
            var dataPath = Application.dataPath;
            var svgPath = dataPath.Replace(assetsDirectory, relativeSvgPath);
            var pngPath = dataPath.Replace(assetsDirectory, relativePngPath);
            var arguments = string.Format("\"{0}\" --export-png=\"{1}\"", svgPath, pngPath);
            var startInfo = new ProcessStartInfo(settings.InkscapeExecutable, arguments);
            StartProcessAndWaitForExit(startInfo);
            if (File.Exists(pngPath))
            {
                var data = File.ReadAllBytes(pngPath);
                if (PixelDataStorage == SvgPixelDataStorage.Png ||
                    PixelDataStorage == SvgPixelDataStorage.Default &&
                    settings.DefaultPixelDataStorage == SvgPixelDataStorage.Png)
                {
                    AssetDatabase.ImportAsset(relativePngPath);
                }
                else
                {
                    File.Delete(pngPath);
                    AssetDatabase.DeleteAsset(relativePngPath);
                }

                texture.LoadImage(data);
                _data = Convert.ToBase64String(data, Base64FormattingOptions.InsertLineBreaks);
            }
            else
            {
                var data = Convert.FromBase64String(_data);
                texture.LoadImage(data);
                LogInkscapeWarning();
            }

            var rect = new Rect(0f, 0f, texture.width, texture.height);
            var pivot = new Vector2(0.5f, 0.5f);
            var sprite = Sprite.Create(texture, rect, pivot);
            sprite.name = relativeSvgPath.Split('/').Last().Replace(svgExt, string.Empty);
            ctx.AddObjectToAsset("texture", texture, texture);
            ctx.AddObjectToAsset("sprite", sprite);
            ctx.SetMainObject(sprite);
        }

        private static void StartProcessAndWaitForExit(ProcessStartInfo startInfo)
        {
            using (var process = new Process())
            {
                process.StartInfo = startInfo;
                try
                {
                    process.Start();
                }
                catch (Exception)
                {
                    LogInkscapeWarning();
                }

                process.WaitForExit();
            }
        }

        private static void LogInkscapeWarning()
        {
            const string mes = "Inkscape not found.";
            Debug.LogWarning(mes);
            SvgImporterWindow.ShowWindow();
        }
    }
}
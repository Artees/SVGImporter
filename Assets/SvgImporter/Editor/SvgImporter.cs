using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SvgImporter.Editor
{
    [ScriptedImporter(1, "svg")]
    // ReSharper disable once UnusedMember.Global
    internal class SvgImporter : ScriptedImporter
    {
        [SerializeField, HideInInspector] private string _data;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var settings = SvgImporterSettings.Load();
            var texture = new Texture2D(4, 4) {alphaIsTransparency = true};
            var svgPath = Application.dataPath.Replace("Assets", ctx.assetPath);
            var pngPath = svgPath.Replace(".svg", ".png");
            var arguments = string.Format("\"{0}\" --export-png=\"{1}\"", svgPath, pngPath);
            var startInfo = new ProcessStartInfo(settings.InkscapeExecutable, arguments);
            StartProcessAndWaitForExit(startInfo);
            if (File.Exists(pngPath))
            {
                var data = File.ReadAllBytes(pngPath);
                File.Delete(pngPath);
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
            sprite.name = ctx.assetPath.Split('/').Last().Replace(".svg", string.Empty);
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
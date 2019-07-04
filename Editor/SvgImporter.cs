﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace Artees.SVGImporter.Editor
{
    [Serializable, ScriptedImporter(3, "svg")]
    // ReSharper disable once UnusedMember.Global
    internal class SvgImporter : ScriptedImporter
    {
        [FormerlySerializedAs("PixelDataStorage"), SerializeField]
        private SvgPixelDataStorage pixelDataStorage;

        [FormerlySerializedAs("_data"), SerializeField, HideInInspector]
        private string data;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var relativeSvgPath = ctx.assetPath;
            if (string.IsNullOrEmpty(relativeSvgPath)) return;
            var settings = SvgImporterSettings.Load();
            var texture = new Texture2D(4, 4) {alphaIsTransparency = true};
            var svgFolder = Path.GetDirectoryName(relativeSvgPath);
            const string pngFolderName = "Rasterized";
            var pngFolder = string.Format("{0}/{1}", svgFolder, pngFolderName);
            if (!AssetDatabase.IsValidFolder(pngFolder))
            {
                AssetDatabase.CreateFolder(svgFolder, pngFolderName);
                EditorUtility.SetDirty(this);
                AssetDatabase.ImportAsset(relativeSvgPath);
                return;
            }

            var relativePngPath = string.Format("{0}/{1}.png", pngFolder,
                Path.GetFileNameWithoutExtension(relativeSvgPath));
            const string assetsDirectory = "Assets";
            var dataPath = Application.dataPath;
            var svgPath = dataPath.Replace(assetsDirectory, relativeSvgPath);
            var pngPath = dataPath.Replace(assetsDirectory, relativePngPath);
            var obsoletePngPath = Path.ChangeExtension(relativeSvgPath, "png");
            AssetDatabase.MoveAsset(obsoletePngPath, relativePngPath);
            ExportPng(settings.InkscapeExecutable, svgPath, pngPath);
            if (File.Exists(pngPath))
            {
                var bytes = ReadPng(pngFolder, relativePngPath, pngPath, settings);
                texture.LoadImage(bytes);
            }
            else
            {
                var bytes = Convert.FromBase64String(data);
                texture.LoadImage(bytes);
                var message = string.Format("File \"{0}\" was not created.", pngPath);
                Debug.LogError(message);
            }

            var svgName = Path.ChangeExtension(relativeSvgPath.Split('/').Last(), string.Empty)
                .Replace(".", string.Empty);
            texture.name = svgName + " Texture";
            var rect = new Rect(0f, 0f, texture.width, texture.height);
            var pivot = new Vector2(0.5f, 0.5f);
            var sprite = Sprite.Create(texture, rect, pivot);
            sprite.name = svgName + " Sprite";
            ctx.AddObjectToAsset("texture", texture, texture);
            ctx.AddObjectToAsset("sprite", sprite);
            ctx.SetMainObject(sprite);
        }

        private byte[] ReadPng(string pngFolder, string relativePngPath, string pngPath, SvgImporterSettings settings)
        {
            var bytes = File.ReadAllBytes(pngPath);
            if (pixelDataStorage == SvgPixelDataStorage.Png ||
                pixelDataStorage == SvgPixelDataStorage.Default &&
                settings.DefaultPixelDataStorage == SvgPixelDataStorage.Png)
            {
                AssetDatabase.ImportAsset(relativePngPath);
            }
            else
            {
                File.Delete(pngPath);
                AssetDatabase.DeleteAsset(relativePngPath);
                var isPngFolderEmpty = !AssetDatabase.GetAllAssetPaths()
                    .Any(s => s.StartsWith(pngFolder) && s != pngFolder);
                if (isPngFolderEmpty)
                {
                    AssetDatabase.DeleteAsset(pngFolder);
                }
            }

            data = Convert.ToBase64String(bytes, Base64FormattingOptions.InsertLineBreaks);
            return bytes;
        }

        private static void ExportPng(string inkscape, string svgPath, string pngPath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = inkscape,
                Arguments = string.Format("\"{0}\" --export-png=\"{1}\"", svgPath, pngPath),
                UseShellExecute = false,
                RedirectStandardError = true,
            };
            using (var process = new Process())
            {
                process.StartInfo = startInfo;
                try
                {
                    process.Start();
                    process.WaitForExit();
                    var error = process.StandardError.ReadToEnd();
                    if (!string.IsNullOrEmpty(error)) Debug.LogError(error);
                }
                catch (Exception)
                {
                    const string mes = "Inkscape not found.";
                    Debug.LogError(mes);
                    SvgImporterWindow.ShowWindow();
                }
            }
        }
    }
}
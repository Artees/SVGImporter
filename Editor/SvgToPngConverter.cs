// ReSharper disable UnusedMember.Global, UnusedMember.Local, UnusedParameter.Local

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.SvgToPngConverter.Editor
{
    public class SvgToPngConverter : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(IEnumerable<string> importedAssets,
            string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string importedAsset in importedAssets.Where(i => i.EndsWith(".svg")))
            {
                string svgPath = Application.dataPath.Replace("Assets", importedAsset),
                    pngPath = svgPath.Replace(".svg", ".png"),
                    arguments = string.Format("\"{0}\" --export-png=\"{1}\"", svgPath, pngPath);
                ProcessStartInfo startInfo = new ProcessStartInfo("inkscape", arguments);
                StartProcessAndWaitForExit(startInfo);
            }
            AssetDatabase.Refresh();
        }

        private static void StartProcessAndWaitForExit(ProcessStartInfo startInfo)
        {
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                try
                {
                    process.Start();
                }
                catch (Exception)
                {
                    const string mes = "Please install Inkscape and add it to your windows " +
                                       "path: https://inkscape.org";
                    UnityEngine.Debug.LogWarning(mes);
                }
                process.WaitForExit();
            }
        }
    }
}
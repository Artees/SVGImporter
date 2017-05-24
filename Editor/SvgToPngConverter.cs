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
                ProcessStartInfo processStartInfo =
                    new ProcessStartInfo("inkscape", arguments);
                Process process = StartProcess(processStartInfo);
                if (process == null) continue;
                process.WaitForExit();
                process.Close();
            }
            AssetDatabase.Refresh();
        }

        private static Process StartProcess(ProcessStartInfo processStartInfo)
        {
            try
            {
                return Process.Start(processStartInfo);
            }
            catch (Exception)
            {
                const string mes = "Please install Inkscape and add it to your windows path: " +
                                   "https://inkscape.org";
                UnityEngine.Debug.LogWarning(mes);
                return null;
            }
        }
    }
}
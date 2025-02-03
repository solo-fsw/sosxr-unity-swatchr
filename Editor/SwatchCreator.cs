﻿using System;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace swatchr
{
    public static class SwatchCreator
    {
        [MenuItem("SOSXR/Create Project Wide Swatch")]
        [MenuItem("Assets/Create/SOSXR/Create Project Wide Swatch")]
        public static void CreateProjectWideSwatch()
        {
            var resourcePath = "Project Wide Swatch"; // Path for Resources.Load (without "Resources/" and without ".asset")
            var assetPath = "Assets/_SOSXR/swatchr/Resources/Project Wide Swatch.asset"; // Full path for AssetDatabase

            var existingSwatch = Resources.Load<Swatch>(resourcePath);

            if (existingSwatch != null)
            {
                if (!EditorUtility.DisplayDialog("Overwrite Project Wide Swatch?",
                        "A Project Wide Swatch already exists. Do you want to overwrite it?", "Yes", "No"))
                {
                    return;
                }

                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(existingSwatch));
            }

            var newSwatch = ScriptableObject.CreateInstance<Swatch>();

            if (!Directory.Exists(Path.GetDirectoryName(assetPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
            }

            AssetDatabase.CreateAsset(newSwatch, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        [MenuItem("SOSXR/Create New Swatch")]
        [MenuItem("Assets/Create/SOSXR/Create New Swatch")]
        public static void CreateSwatch()
        {
            var asset = ScriptableObject.CreateInstance<Swatch>();
            ProjectWindowUtil.CreateAsset(asset, "New Swatch.asset");
            AssetDatabase.SaveAssets();
        }


        public static Swatch CreateSwatchFromASEFile(SwatchASEFile aseFile, string projectSaveDestination)
        {
            var swatch = Swatch.FromSwatchASEFile(aseFile);

            projectSaveDestination = AssetDatabase.GenerateUniqueAssetPath(projectSaveDestination);
            AssetDatabase.CreateAsset(swatch, projectSaveDestination);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = swatch;

            return swatch;
        }


        // [MenuItem("SOSXR/Swatchr/Duplicate Swatch")]
        public static void DuplicateSwatch()
        {
            var activeObject = (Swatch) Selection.activeObject;
            var asset = ScriptableObject.CreateInstance<Swatch>();
            asset.AddColorsFromOtherSwatch(activeObject);
            ProjectWindowUtil.CreateAsset(asset, activeObject.name + ".asset");
        }


        // [MenuItem("SOSXR/Swatchr/Duplicate Swatch", true)]
        public static bool ValidateDuplicateSwatch()
        {
            var activeObject = Selection.activeObject;

            return activeObject != null && activeObject is Swatch;
        }


        // [MenuItem("SOSXR/Swatchr/Export Swatch To Color Presets")]
        public static void ExportToPresets()
        {
            var activeObject = (Swatch) Selection.activeObject;
            SwatchPresetExporter.ExportToColorPresetLibrary(activeObject);
        }


        // [MenuItem("SOSXR/Swatchr/Export Swatch To Color Presets", true)]
        public static bool ValidateExportToPresets()
        {
            var activeObject = Selection.activeObject;

            return activeObject != null && activeObject is Swatch;
        }


        // [MenuItem("SOSXR/Swatchr/Import ASE File")]
        private static void ImportSelectedASEFile()
        {
            var activeObject = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(activeObject.GetInstanceID());
            var fullPath = path.Replace("Assets", Application.dataPath);
            var saveDestination = path.Replace(".ase", ".asset");

            var aseFile = new SwatchASEFile(fullPath);
            CreateSwatchFromASEFile(aseFile, saveDestination);
        }


        // [MenuItem("SOSXR/Swatchr/Import ASE File", true)]
        private static bool ValidateImportSelectedASEFile()
        {
            var activeObject = Selection.activeObject;

            if (activeObject == null)
            {
                return false;
            }

            var path = AssetDatabase.GetAssetPath(activeObject.GetInstanceID());

            return path.EndsWith(".ase");
        }


        // [MenuItem("SOSXR/Swatchr/Import ASE File (Browse...)")]
        private static void ImportASEFileBrowse()
        {
            var path = EditorUtility.OpenFilePanel("Swatchr Import", "", "ase");

            if (path != null && path != string.Empty)
            {
                var aseFile = new SwatchASEFile(path);
                CreateSwatchFromASEFile(aseFile, GetSelectedSavePath(aseFile.title));
            }
        }


        // [MenuItem("SOSXR/Swatchr/Import ASE Folder Into One Swatch (Browse...)")]
        private static void ImportASEFolderIntoOne()
        {
            var path = EditorUtility.OpenFolderPanel("Swatchr Import Folder", "", "");

            if (path != null && path != string.Empty)
            {
                var files = Directory.GetFiles(path);
                Swatch parentSwatch = null;

                for (var i = 0; i < files.Length; i++)
                {
                    var file = files[i];

                    if (file.EndsWith(".ase"))
                    {
                        var aseFile = new SwatchASEFile(file);

                        if (parentSwatch == null)
                        {
                            parentSwatch = CreateSwatchFromASEFile(aseFile, GetSelectedSavePath(aseFile.title));
                        }
                        else
                        {
                            parentSwatch.AddColorsFromASEFile(aseFile);
                        }
                    }
                }
            }
        }


        // [MenuItem("SOSXR/Swatchr/Import ASE Folder Into Seperate Swatches (Browse...)")]
        private static void ImportASEFolderIntoMany()
        {
            var path = EditorUtility.OpenFolderPanel("Swatchr Import Folder", "", "");

            if (path != null && path != string.Empty)
            {
                var files = Directory.GetFiles(path);

                for (var i = 0; i < files.Length; i++)
                {
                    var file = files[i];

                    if (file.EndsWith(".ase"))
                    {
                        var aseFile = new SwatchASEFile(file);
                        CreateSwatchFromASEFile(aseFile, GetSelectedSavePath(aseFile.title));
                    }
                }
            }
        }


        // [MenuItem("SOSXR/Swatchr/Import Swatch From Texture (Browse...)")]
        public static void ImportSwatchFromTexture()
        {
            var path = EditorUtility.OpenFilePanel("Swatchr Import Texture", "", "png");

            if (path != null && path != string.Empty)
            {
                Debug.Log("[SwatchEditorGUI] importing texture at path " + path);
                var bytes = File.ReadAllBytes(path);
                var tex = new Texture2D(1, 1);
                tex.LoadImage(bytes);
                var pixels = tex.GetPixels();

                if (pixels != null && pixels.Length > 0)
                {
                    var swatch = ScriptableObject.CreateInstance<Swatch>();
                    Array.Resize(ref swatch.colors, pixels.Length);

                    for (var j = 0; j < pixels.Length; j++)
                    {
                        swatch.colors[j] = pixels[j];
                    }

                    ProjectWindowUtil.CreateAsset(swatch, "New Swatch.asset");
                    AssetDatabase.SaveAssets();
                }
            }
        }


        // [MenuItem("SOSXR/Swatchr/Export Swatch To Texture")]
        public static void ExportSwatchToTexture()
        {
            var selectedSwatch = (Swatch) Selection.activeObject;
            var assetLocation = GetSelectedPath() + "/" + GetSelectedFileName() + ".png";
            var saveLocation = ConvertAssetPathToFullPath(assetLocation);
            SwatchrExportToTexture.ExportSwatchToTexture(selectedSwatch, saveLocation);
            AssetDatabase.ImportAsset(assetLocation);
        }


        // [MenuItem("SOSXR/Swatchr/Export Swatch To Texture", true)]
        public static bool ExportSwatchToTexture_Validate()
        {
            var activeObject = Selection.activeObject;

            return activeObject != null && activeObject is Swatch;
        }


        private static string GetSelectedSavePath(string title)
        {
            return AssetDatabase.GenerateUniqueAssetPath(GetSelectedPath() + "/" + title + ".asset");
        }


        private static string GetSelectedFileName()
        {
            return Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(Selection.activeObject));
        }


        private static string GetSelectedPath()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(path), "");
            }

            return path;
        }


        public static string ConvertAssetPathToFullPath(string assetPath)
        {
            var fullPath = Application.dataPath + assetPath.Substring("Assets".Length);

            return fullPath;
        }
    }
}
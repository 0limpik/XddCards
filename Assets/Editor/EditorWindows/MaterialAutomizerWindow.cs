using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MaterialAutomizerWindow : EditorWindow
{
    private Material source;
    private DefaultAsset importFolder;
    private DefaultAsset exportFolder;

    void OnEnable()
    {
        GetWindow<MaterialAutomizerWindow>();
    }

    void OnGUI()
    {
        source = (Material)EditorGUILayout.ObjectField("Source", source, typeof(Material), false);

        importFolder = (DefaultAsset)EditorGUILayout.ObjectField("Import Folder",
                importFolder, typeof(DefaultAsset), false);

        exportFolder = (DefaultAsset)EditorGUILayout.ObjectField("Export Folder",
                exportFolder, typeof(DefaultAsset), false);


        if (GUILayout.Button("Create"))
        {
            var path = Application.dataPath;
            path = path.Substring(0, path.Length - 6);
            path += AssetDatabase.GetAssetPath(importFolder);
            var files = Directory.GetFiles(path)
                .Where(x => Path.GetExtension(x) != ".meta")
                .Select(x => x.Remove(0, x.LastIndexOf("Assets")))
                .ToList();

            foreach (var file in files)
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(file);

                Debug.Log(texture.name);
                var name = texture.name;
                var material = Instantiate(source);
                material.mainTexture = texture;
                material.SetTexture("_HeightMap ", texture);
                AssetDatabase.CreateAsset(material, @$"{AssetDatabase.GetAssetPath(exportFolder)}/{Path.GetFileNameWithoutExtension(file)}.mat");
            }
        }
    }
}

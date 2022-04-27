using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Xdd.Editor.EditorWindows;
using Debug = UnityEngine.Debug;

namespace Xdd.Editor.MenuItems
{
    public class MenuItems : MonoBehaviour
    {
        [MenuItem("Xdd/Material Automizer")]
        static void CreateMaterial()
        {
            var popUp = ScriptableObject.CreateInstance<MaterialAutomizerWindow>();

            popUp.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);

            popUp.ShowPopup();
        }

        [MenuItem("Xdd/Card Form Material")]
        static void CardFromMaterial()
        {
            var popUp = ScriptableObject.CreateInstance<CardFromMaterialCreator>();

            popUp.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);

            popUp.ShowPopup();
        }

        readonly static string compilerPath = $"{Application.dataPath}/Plugins/Protoc.Compiler/protoc.exe";

        readonly static string protosInput = $"{Application.dataPath}/Source/XddCards.Protos";
        readonly static string protosOut = $"{Application.dataPath}/Source/XddCards.Protos/Protos.Compile";

        [MenuItem("Xdd/Recompile Protos")]
        static void RecompileProtos()
        {
            Directory.GetFiles(protosOut, "*", SearchOption.AllDirectories).ToList().ForEach(File.Delete);
            Directory.GetDirectories(protosOut).ToList().ForEach(Directory.Delete);

            var files = Directory.GetFiles(protosInput, "*.proto", SearchOption.AllDirectories).Select(x => x.Replace('\\', '/'));

            foreach (var file in files)
            {
                var path = $"{file.Replace($"{protosInput}/", "")}";
                var dir = Path.GetDirectoryName(path).Replace('\\', '/');

                var outDir = $"{protosOut}/{dir}";

                Debug.Log($"Compile: {path}");

                Directory.CreateDirectory(outDir);

                var proto_path = $"--proto_path=\"{protosInput}\"";
                var csharp_out = $"--csharp_out=\"{outDir}\"";
                var grpc_out = $"--grpc_out=\"{outDir}\"";
                var plugin = $"--plugin=protoc-gen-grpc=grpc_csharp_plugin.exe";

                var arg = $"\"{path}\" {proto_path} {csharp_out} {grpc_out} {plugin}";
                Debug.Log($"Args: {arg}");

                var process = new Process();
                process.StartInfo.FileName = compilerPath;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(compilerPath);
                process.StartInfo.Arguments = $"{arg}";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.OutputDataReceived += (s, e) => Debug.Log(e.Data);
                process.ErrorDataReceived += (s, e) => Debug.LogError(e.Data);

                process.Start();
                process.WaitForExit();
            }
        }
    }
}

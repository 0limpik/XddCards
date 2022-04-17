using System.Diagnostics;
using System.IO;
using System.Linq;
using Assets.Editor.EditorWindows;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Editor.MenuItems
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

        readonly static string protosPath = $"{Application.dataPath}/Source/Network/Protos/XddCards.Protos/";
        readonly static string csharpOut = $"{Application.dataPath}/Source/Network/ProtosCompile/CS";
        readonly static string grpcOut = $"{Application.dataPath}/Source/Network/ProtosCompile/GRPC";

        [MenuItem("Xdd/Recompile Protos")]
        static void RecompileProtos()
        {
            Directory.GetFiles(csharpOut).ToList().ForEach(File.Delete);
            Directory.GetFiles(grpcOut).ToList().ForEach(File.Delete);

            var files = Directory.GetFiles(protosPath, "*.proto", SearchOption.AllDirectories).Select(x => x.Replace('\\', '/'));

            foreach (var file in files)
            {
                var path = $"{file.Replace(protosPath, "")}";
                var dir = Path.GetDirectoryName(path);

                var csharpDir = $"{csharpOut}/{dir}";
                var grpcDir = $"{grpcOut}/{dir}";

                Debug.Log($"Compile: {path}");

                Directory.CreateDirectory(csharpDir);
                Directory.CreateDirectory(grpcDir);

                var proto_path = $"--proto_path=\"{protosPath}\"";
                var csharp_out = $"--csharp_out=\"{csharpDir}\"";
                var grpc_out = $"--grpc_out=\"{grpcDir}\"";
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

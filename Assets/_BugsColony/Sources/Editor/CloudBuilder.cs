using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Builder {
    public enum BuildAppTarget { Apk , Aab }
    private static string _pass = "123qwe";

    private static string[] GetActiveScenes() {
        var result = new List<string>();

        foreach (var scene in EditorBuildSettings.scenes)
            if (scene.enabled)
                result.Add(scene.path);

        return result.ToArray();
    }

    private static string AppRoot() => Application.dataPath + "/../";

    private static BuildPlayerOptions Create(BuildAppTarget app, BuildTarget target, BuildOptions options, string version = "") {
        var targetExtension = app == BuildAppTarget.Aab ? "aab" : "apk";
        
        return new BuildPlayerOptions {
            locationPathName = Path.Combine(Application.dataPath, $"../Builds/{Application.productName}-{Application.version}{version}-Install.{targetExtension}"),
            scenes = GetActiveScenes(),
            target = target,
            options = options,
        };
    }
    
    private static string GetAndroidBundleSubversion() => "." + PlayerSettings.Android.bundleVersionCode;
    
    [MenuItem("Builder/[Debug] Build APK Android")]
    public static void BuildDebugAndroidPlayer() {
        Build(BuildAppTarget.Apk, Create(BuildAppTarget.Apk, BuildTarget.Android, BuildOptions.ShowBuiltPlayer | BuildOptions.Development, GetAndroidBundleSubversion()));
    }

    [MenuItem("Builder/Build APK Android")]
    public static void BuildAndroidPlayer() {
        Build(BuildAppTarget.Apk, Create(BuildAppTarget.Apk, BuildTarget.Android, BuildOptions.ShowBuiltPlayer, GetAndroidBundleSubversion()));
    }
    
    [MenuItem("Builder/[Debug] Build AAB Android")]
    public static void BuildDebugAbbAndroidPlayer() {
        Build(BuildAppTarget.Aab, Create(BuildAppTarget.Aab, BuildTarget.Android, BuildOptions.ShowBuiltPlayer | BuildOptions.Development, GetAndroidBundleSubversion()));
    }

    [MenuItem("Builder/Build AAB Android")]
    public static void BuildAabAndroidPlayer() {
        Build(BuildAppTarget.Aab, Create(BuildAppTarget.Aab, BuildTarget.Android, BuildOptions.ShowBuiltPlayer, GetAndroidBundleSubversion()));
    }

    private static void Build(BuildAppTarget target, BuildPlayerOptions options) {
        //VersionHelper.CreateBundleData(Application.version, PlayerSettings.Android.bundleVersionCode);
        
        //CreateCheckSum();
        
        PlayerSettings.Android.keyaliasPass = _pass;
        PlayerSettings.Android.keystorePass = _pass;
        
        EditorUserBuildSettings.buildAppBundle = target == BuildAppTarget.Aab;
        
        if(!KeyStoreFileExists()) Debug.Log($"Keystore not found!");
        
        BuildPipeline.BuildPlayer(options);

        //var checksum = "this".GatherChecksum();
        //File.WriteAllText(Path.Combine(Path.GetDirectoryName(options.locationPathName), "checksum.txt"), checksum);
    }

    private static bool KeyStoreFileExists() {
        var projectPath = Path.Combine(Application.dataPath, "../user.keystore");
        return File.Exists(projectPath);
    }
    
    [MenuItem("Builder/Firebase Functions")]
    public static void OpenVSCode() {
        //Application.OpenURL($"W:/work/gotoJesusFunctions/Functions/vc.cmd");
        Process.Start("cmd.exe", " /c \"cd .. && cd W:/NewWork/SuperCashFunctions/functions && vc.cmd\"");
    }
    
    /*[MenuItem("Builder/Update checksums")]
    public static void CreateCheckSum() {
        var checksum = GetChecksumOfAllCSFiles(Application.dataPath);

        var resoures = Path.Combine(Application.dataPath, "Resources", "version.json");

        File.WriteAllText(resoures, checksum);
    }

    private static string GetChecksumOfAllCSFiles(string directory) {
        using var sha256 = SHA256.Create();
        using var combinedStream = new MemoryStream();
        var csFiles = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);

        foreach (var file in csFiles)
        {
            var fileContent = File.ReadAllBytes(file);
            combinedStream.Write(fileContent, 0, fileContent.Length);
        }

        var bundleCode = Encoding.UTF8.GetBytes(VersionHelper.BundleCode());
        combinedStream.Write(bundleCode, 0, bundleCode.Length);

        var hash = sha256.ComputeHash(combinedStream.ToArray());
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }*/

}

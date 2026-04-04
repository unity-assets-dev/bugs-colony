using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public partial class EditorBootloader {
    
    private const string PARTIAL_PATH = "_BugsColony/Sources/Editor/EditorBootloadMaps.cs";
    private const string MAPS_PATH = "Scenes";
    
    private const string BOOTSTRAP_SCENE = "Client";
    private const string SERVER_SCENE = "Server";
    private const string CLIENT_SCENE = "Client";
    
    private static string _currentScene;

    static EditorBootloader() => GenerateMaps();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoad() {
        if (SceneManager.GetActiveScene().name == BOOTSTRAP_SCENE) return;
        
        _currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(BOOTSTRAP_SCENE, LoadSceneMode.Single);
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state) {
        if (string.IsNullOrEmpty(_currentScene) || state != PlayModeStateChange.ExitingPlayMode) return;
        
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        SceneManager.LoadScene(_currentScene, LoadSceneMode.Single);
        
        _currentScene = null;
    }

    private static void LoadScene(string sceneName, OpenSceneMode mode) => EditorSceneManager.OpenScene($"Assets/Scenes/{sceneName}.unity",mode);
    private static void LoadScene(string sceneName) => EditorSceneManager.OpenScene(sceneName, OpenSceneMode.Single);

    //[MenuItem("Tools/Scenes/Bootstrap", priority = 100)]
    private static void LoadBootstrapScene() => LoadScene(BOOTSTRAP_SCENE, OpenSceneMode.Single);
    
    //[MenuItem("Tools/Scenes/Server", priority = 101)]
    private static void LoadServerScene() => LoadScene(SERVER_SCENE, OpenSceneMode.Single);
    
    [MenuItem("Tools/Scenes/Client", priority = 102)]
    private static void LoadClientScene() => LoadScene(CLIENT_SCENE, OpenSceneMode.Single);
    
    private static void GenerateMaps() {
        var builder = new StringBuilder();
        // Header
        builder.Append(
            "using UnityEditor;\n\n" +
            
            "public partial class EditorBootloader {\n"
        );
        builder.Append(FillMaps(Path.Combine(Application.dataPath, MAPS_PATH)));
        builder.Append("\n}");

        SaveGeneration(Path.Combine(Application.dataPath, PARTIAL_PATH), builder.ToString());
    }

    private static void SaveGeneration(string path, string content) => File.WriteAllText(path, content, Encoding.UTF8);

    private static StringBuilder FillMaps(string mapsPath) {
        var maps = Directory
            .GetFiles(mapsPath)
            .Where(p => {
                var fileName = Path.GetFileName(p);
                return fileName.StartsWith("Map") && !fileName.Contains(".meta");
            });
        var builder = new StringBuilder();
        foreach (var map in maps) {
            var mapName = Path.GetFileNameWithoutExtension(map);
            var mapPath = "Assets" + map.Replace(Application.dataPath, "");
            builder.Append(
                $"\n\t[MenuItem(\"Tools/Scenes/Maps/({mapName})\")]\n" +
                $"\tprivate static void LoadMap{mapName}() => LoadScene(\"{mapPath}\");\n"
            );
        }
        return builder;
    }
}

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class KitInstaller {
    
    [MenuItem("Tools/UIKit/Add brand new scene")]
    public static void NewProject() {
        var scene = SceneConstructor.Create("NewApp");
    }

    private class SceneConstructor {
        private Scene _scene;


        public void CreateScene(string sceneName) {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            scene.name = sceneName;
            EditorSceneManager.SetActiveScene(scene);
            
            AddCommons(scene);
            AddDependencies(scene);
            AddEntryPoint(scene);
            AddCanvas(scene);
            
        }

        private void AddCanvas(Scene scene) {
            var root = new GameObject("[UI]");
            
            CanvasComponents(root);

            var items = new Dictionary<string, Transform>(3);
            foreach (var itemName in new [] { "Screens", "Popups", "System" }) {
                items.Add(itemName, AddCanvasChild(root.transform, itemName));
            };
            
            var bootstrap = AddCanvasChild(items["Screens"], "BootstrapScreen");
            
            bootstrap.GetComponent<BootstrapScreen>();
            bootstrap.GetComponent<ScreenAnimator>();
            
            AddCanvasChild(bootstrap, "Layout");
            
            

            Transform AddCanvasChild(Transform parent, string itemName) {
                var item = new GameObject($"[{itemName}]");
                item.transform.SetParent(parent);
                var rect = item.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = Vector2.one * .5f;
                rect.offsetMax = Vector2.zero;
                rect.offsetMin = Vector2.zero;
                item.AddComponent<CanvasRenderer>();
                return item.transform;
            }
        }

        private static void CanvasComponents(GameObject root) {
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var scaler = root.AddComponent<CanvasScaler>();
            
            scaler.referenceResolution = new Vector2(828, 1792);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.matchWidthOrHeight = .5f;
            
            var raycaster = root.AddComponent<GraphicRaycaster>();
        }

        private void AddEntryPoint(Scene scene) {
            var entry = new GameObject("[AppEntry]");
            entry.AddComponent<AppEntry>();
        }

        private void AddDependencies(Scene scene) {
            var root = new GameObject("[Dependencies]");
            var context = root.AddComponent<SceneContext>();
            var installer = root.AddComponent<UIKitInstaller>();
            
            context.Installers = new [] {installer};
        }

        private void AddCommons(Scene scene) {
            var commons = new GameObject("[Common]");
            
            Object.FindAnyObjectByType<Camera>().transform.SetParent(commons.transform);
            Object.FindAnyObjectByType<Light>().transform.SetParent(commons.transform);
            
            var eventSystemRoot = new GameObject("EventSystem");
            var eventSystem = eventSystemRoot.AddComponent<EventSystem>();
            var inputModule = eventSystemRoot.AddComponent<StandaloneInputModule>();
            eventSystemRoot.transform.SetParent(commons.transform);
        }

        public static SceneConstructor Create(string sceneName) {
            var instance = new SceneConstructor();
            instance.CreateScene(sceneName);
            return instance;
        }
    }
}

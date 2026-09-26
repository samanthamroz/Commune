using UnityEngine;

namespace ClickIt.Backend {
    internal class ClickItInitializer {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnRuntimeMethodLoad() //This gets called automatically by Unity
        {
            GameObject prefab = Resources.Load<GameObject>("ClickIt!");
            GameObject obj = GameObject.Instantiate(prefab);
            obj.name = "ClickIt!";
        }
    }
}
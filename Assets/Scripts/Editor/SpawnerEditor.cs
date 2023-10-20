using UnityEditor;
using UnityEngine;

namespace FallGuys.Editor
{
    [CustomEditor(typeof(Spawner))]
    public class SpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Spawner spawner = (Spawner)target;

            if (GUILayout.Button("Spawn")) spawner.Spawn();
        }
    }
}
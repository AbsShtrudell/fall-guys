using UnityEditor;
using UnityEngine;

namespace FallGuys.Editor
{
    [CustomEditor(typeof(CharacterController))]
    public class CharacterControllersEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CharacterController controller = (CharacterController)target;

            if (GUILayout.Button("Kill")) controller.Kill();
        }
    }
}
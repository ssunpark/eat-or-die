// #if UNITY_EDITOR
// using UnityEngine;
// using UnityEditor;
//
// public class DragonStateDebuggerWindow : EditorWindow
// {
//     private DragonStateMachine _dragon;
//     private EBossState _selectedState;
//
//     [MenuItem("Tools/Dragon State Debugger")]
//     public static void ShowWindow()
//     {
//         GetWindow<DragonStateDebuggerWindow>("Dragon State Debugger");
//     }
//
//     private void OnGUI()
//     {
//         GUILayout.Label("드래곤 상태 디버거", EditorStyles.boldLabel);
//
//         _dragon = (DragonStateMachine)EditorGUILayout.ObjectField("Dragon", _dragon, typeof(DragonStateMachine), true);
//
//         if (_dragon == null)
//         {
//             EditorGUILayout.HelpBox("Hierarchy에서 DragonStateMachine을 가진 오브젝트를 Drag & Drop 하세요.", MessageType.Info);
//             return;
//         }
//
//         _selectedState = (EBossState)EditorGUILayout.EnumPopup("Target State", _selectedState);
//
//         if (GUILayout.Button("Change State"))
//         {
//             ApplyStateChange();
//         }
//     }
//
//     private void ApplyStateChange()
//     {
//         if (Application.isPlaying)
//         {
//             if (_dragon.HasStateAuthority)
//             {
//                 // _dragon.ChangeState(_selectedState);
//                 Debug.Log($"드래곤 상태를 {_selectedState}로 변경했습니다.");
//             }
//             else
//             {
//                 Debug.LogWarning("StateAuthority가 아니므로 상태 변경이 적용되지 않습니다.");
//             }
//         }
//         else
//         {
//             Debug.LogWarning("재생 중에만 상태를 변경할 수 있습니다.");
//         }
//     }
// }
// #endif
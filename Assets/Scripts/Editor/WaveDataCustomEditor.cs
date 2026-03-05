using UnityEditor;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
[CustomEditor(typeof(WaveData))]
public class WaveDataCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 그리기
        DrawDefaultInspector();

        WaveData waveData = (WaveData)target;


        // 연결되어 있고 리스트가 있으면 드롭다운 표시
        if (waveData.expItemDB != null && waveData.expItemDB.entryList != null && waveData.expItemDB.entryList.Count > 0)
        {
            string[] options = waveData.expItemDB.entryList.Select(e => e.exp.ToString()).ToArray();

            // 현재 선택된 인덱스 찾기
            int currentIndex = Mathf.Max(0, waveData.expItemDB.entryList.IndexOf(waveData.GetSelectedExpItemEntry()));

            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUILayout.Popup("선택된 경험치 : ", currentIndex, options);

            // 드롭다운을 클릭해서 값을 바꿨을 때만 True
            if (EditorGUI.EndChangeCheck()) 
            {
                // 단순히 에디터가 재실행되거나 그려질 때는 실행되지 않아 안전하다.
                Undo.RecordObject(waveData, "Change Exp Item"); // 실행 취소 기록
                waveData.SetSelectedExpItemEntry(waveData.expItemDB.entryList[newIndex]);
                EditorUtility.SetDirty(waveData); // 저장 표시
            }
        }


        if (GUI.changed)
        {
            EditorUtility.SetDirty(waveData);
        }
    }
}
#endif
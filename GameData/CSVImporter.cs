using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CsvImporter : EditorWindow
{
    // CSV 파일 경로 (Assets 폴더 기준)
    static string csvFilePath = "Assets/Scripts/GameData/AbilityData.csv";
    // 저장될 SO 경로
    static string soSavePath = "Assets/Scripts/GameData/AbilityDatabase.asset";

    [MenuItem("Tools/Import CSV to SO")]
    public static void ImportCsvData()
    {
        if (!File.Exists(csvFilePath))
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {csvFilePath}");
            return;
        }

        string[] lines = File.ReadAllLines(csvFilePath);
        if (lines.Length <= 1) return; // 헤더만 있거나 빈 파일

        Debug.Log($"lines의 길이 {lines.Length}"); 
        List<AbilityData> dataList = new List<AbilityData>();

        var header = lines[0].Split(','); // Key 값만 저장되어 있는 데이터

        // 키값이 몇 번째 열의 순서인지를 저장.
        Dictionary<int, string> indexKeyTable = new Dictionary<int, string>();
        for(int i = 0; i < header.Length;++i)
        {
            indexKeyTable[i] = header[i].Trim();
        }

        for(int i = 1; i < lines.Length;++i)
        {
            var stringAbilityDataTable = lines[i].Split(',');
            AbilityData data = new AbilityData();

            foreach(var keyValuePair in indexKeyTable)
            {
                var idx = keyValuePair.Key;
                var key = keyValuePair.Value;
                data.DataParse(key,stringAbilityDataTable[idx].Trim());
            }

            dataList.Add(data);
        }

        // ScriptableObject에 저장
        AbilityDataBaseSO database = AssetDatabase.LoadAssetAtPath<AbilityDataBaseSO>(soSavePath);

        // SO가 없으면 새로 생성
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<AbilityDataBaseSO>();
            AssetDatabase.CreateAsset(database, soSavePath);
        }

        // 데이터 덮어쓰기
        database.allAbility = dataList;

        // 변경 사항 저장
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("CSV -> SO 변환 완료!");
        
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class CsvImporter : EditorWindow
{
    // CSV 파일 경로 (Assets 폴더 기준)
    static string csvFilePath = "Assets/Scripts/GameData/AbilityData.csv";
    static string soSavePath  = "Assets/Scripts/GameData/AbilityDatabase.asset";

    [MenuItem("Tools/Import CSV -> AbilityDataBaseSO ")]
    public static void ImportCsvAbilityData()
    {
        List<AbilityData> dataList = MapCsvToObjects<AbilityData>(csvFilePath);

        if(dataList.Count == 0)
        {
            Debug.LogError("CSV 데이터가 없거나 잘못되었습니다.");
            return;
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

    public static List<T> MapCsvToObjects<T>(string filePath) where T : new()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {filePath}");
            return new List<T>();
        }

        string[] lines = File.ReadAllLines(filePath);
        if (lines.Length <= 1) return new List<T>(); // 키만 있거나 빈 파일

        // 정규표현식: 따옴표 안의 쉼표는 무시하고 분리
        string csvSplitPattern = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";

        // 헤더 파싱
        var keyList = Regex.Split(lines[0], csvSplitPattern)
                           .Select(h => h.Trim())
                           .ToList();
        var results = new List<T>();

        // 필드 정보 가져오기
        Type type = typeof(T);

        for (int i = 1; i < lines.Length; ++i)
        {
            var values = lines[i].Split(',');
            T obj = new T();
            
            for (int j = 0; j < keyList.Count; ++j)
            {
                // 공백 데이터 건너뛰기
                if (string.IsNullOrWhiteSpace(values[j])) continue;

                // 문자열 정제: 양끝 따옴표 제거 및 "" -> " 변환
                string finalValue = values[j].Trim('"').Replace("\"\"", "\"");

                // 리플렉션으로 필드 찾기
                var field = type.GetField(keyList[j]);

                if (field != null)
                {
                    try
                    {
                        // 타입 동적 변환 및 할당
                        object value = Convert.ChangeType(finalValue, field.FieldType);
                        field.SetValue(obj, value);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[CSV 파싱 에러] Line {i + 1}, Field '{keyList[j]}': {e.Message}");
                    }
                }
            }
            results.Add(obj);
        }

        return results;
    }
}
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;

public class UILevelUpSelect : MonoBehaviour
{
    [SerializeField] Button[] selectBtnList;

    Dictionary<Button,UnityEngine.Events.UnityAction> actionMap = new Dictionary<Button, UnityEngine.Events.UnityAction>();

    public void UpdateSelect(List<AbilityData> abilityDatas)
    {
        for(int i = 0; i < selectBtnList.Length; ++i)
        {
            var btn = selectBtnList[i];
            var txt_Name = btn.GetComponentInChildren<TMP_Text>();

            if(abilityDatas.Count > i){
                var abilityData = abilityDatas[i];
                txt_Name.text = abilityData.name;

                UnityEngine.Events.UnityAction action = () => ChoiceAbility(btn,abilityData);
                // 반복문 안에서 별도 변수에 담아야 올바르게 전달
                btn.onClick.AddListener(action);

                actionMap[btn] = action;
            }
            else
            {
                txt_Name.text = "None";
            }
        }

        gameObject.SetActive(true);
        
        var inputMgr = FindFirstObjectByType<PlayerInputManager>();
        inputMgr.SetCursorLockMode(false);
    }

    void Init()
    {
        for(int i = 0; i < selectBtnList.Length; ++i){
            var btn = selectBtnList[i];
            var action = actionMap[btn];
            if(action != null)
                btn.onClick.RemoveListener(action);
        }
    }

    void ChoiceAbility(Button btn , AbilityData abilityData)
    {
        LevelUpManager.Instance.ApplyChoiceAbility(abilityData);
        gameObject.SetActive(false);

        var inputMgr = FindFirstObjectByType<PlayerInputManager>();
        inputMgr.SetCursorLockMode(true);

        Init();
    }
}

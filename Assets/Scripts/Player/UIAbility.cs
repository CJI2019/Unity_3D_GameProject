using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAbility : MonoBehaviour
{
    [SerializeField] Slider ui_Exp;
    [SerializeField] TMP_Text txt_Level;
    PlayerAbility player;

    public void Init(GameObject playerObject)
    {
        this.player = playerObject.GetComponent<PlayerAbility>();
        player.OnAddExp += UpdateEXPUI;
        player.OnLevelUp += UpdateLevelUI;
    }
    public void DeInit()
    {
        player.OnAddExp -= UpdateEXPUI;
        player.OnLevelUp -= UpdateLevelUI;
    }
    
    void UpdateEXPUI(object sender, OnAddExpArgs args)
    {
        int exp = args.CurrentExp;
        int maxExp = args.MaxExp;

        ui_Exp.value = (float)exp / maxExp;
    }

    void UpdateLevelUI(int playerLevel)
    {
        txt_Level.text = $"LV.{playerLevel}";
    }
}

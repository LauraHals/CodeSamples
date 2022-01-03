using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameSystems;

/// <summary>
/// Fills xp-bar smoothly according to current lvl and xp-amount
/// </summary>
public class XpBar : MonoBehaviour
{
    public static XpBar instance;

    public float fillSpeed = 2f;
    public Image bar;
    public TextMeshProUGUI lvl;
    public TextMeshProUGUI nextlvl;
    public float displayedXP;
    public float levelMaxXP;
    public float currentFill;
    public float targetFill;
    public float overflow;

    //Queue xp-amounts and fillafter last coroutine has ended
    private Queue<float> xpAdds = new Queue<float>();
    private Coroutine lerpCoroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializeBar();
        InitializeLevel();
    }

    private void InitializeBar()
    {
        levelMaxXP = Save.xpToLevelUp;
        displayedXP = Save.currentXP;
        if (displayedXP != 0)
        {
            bar.fillAmount = displayedXP / levelMaxXP;
        }
        else
        {
            bar.fillAmount = 0;
        }
    }

    public void InitializeLevel()
    {
        if(lvl!= null)
        {
            lvl.text = Save.playerLevel.ToString();
        }

        if(nextlvl != null)
        {
            nextlvl.text = (Save.playerLevel + 1).ToString();
        }
    }

    public void Fill(int addedXP)
    {
        xpAdds.Enqueue(addedXP);

        if(lerpCoroutine == null)
        {
            lerpCoroutine = StartCoroutine(FillBar());
        }
    }

    private IEnumerator FillBar()
    {
        float xpToAdd = xpAdds.Dequeue();

        GetRatios(xpToAdd);

        float t = 0;
        while(t < fillSpeed)
        {
            bar.fillAmount = Mathf.Lerp(currentFill, targetFill, t / fillSpeed);
            t += Time.deltaTime;
            yield return null;
        }

        bar.fillAmount = targetFill;
        displayedXP += (xpToAdd - overflow);

        Save.currentXP += xpToAdd;

        if(bar.fillAmount == 1)
        {
            LevelUp();
        }


        if(overflow > 0)
        {
            xpAdds.Enqueue(overflow);
            lerpCoroutine = StartCoroutine(FillBar());
        }else if(xpAdds.Count > 0)
        {
            lerpCoroutine = StartCoroutine(FillBar());
            
        }else
        {
            lerpCoroutine = null;
        }

    }

    public void LevelUp()
    {
        Save.playerLevel++;
        Save.xpToLevelUp *= GameSystems.SaveManager.xpIncrease;
        levelMaxXP = Save.xpToLevelUp;
        Save.currentXP = 0;
        bar.fillAmount = 0;
        currentFill = 0;
        displayedXP = 0;
        DoozyUIManager.ShowPopup("LevelUp");
        InitializeLevel();
    }

    private void GetRatios(float xpToAdd)
    {
        currentFill = bar.fillAmount;
        targetFill = Mathf.Clamp((displayedXP+xpToAdd) / levelMaxXP, 0, 1);

        if(displayedXP+xpToAdd > levelMaxXP)
        {
            overflow = xpToAdd - (levelMaxXP - displayedXP);
        }else
        {
            overflow = 0;
        }
    }
}

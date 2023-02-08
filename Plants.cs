using BansheeGz.BGDatabase;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Plants : MonoBehaviour, IPointerClickHandler
{
    public static Plants instance;

    public GameObject[] plantSlot;
    public BoxCollider2D[] box;
    public PlantSlot[] slots;

    public int seedNum; 
    public GameObject seedPup;

    private bool isPup;

    public GameObject lockPanel;

    public TMP_Text hub0Txt;
    public TMP_Text hub1Txt;
    public TMP_Text hub2Txt;

    private void Awake()
    {
        instance = this;

    }

    void Start()
    {
        DataManager.instance.LoadFile();

        GetComponent<BoxCollider2D>();

        isPup = false;
        seedPup.SetActive(false);

        // ÇØ±Ý ½Ã panel Á¦°Å
        if (DataManager.instance.boilUnlock) Destroy(lockPanel);
    }

    void Update()
    {
        // ¾¾¾Ñ ½É±â ÆË¾÷
        if (isPup)
        {
            var repo = BGRepo.I;
            var meta = repo["PlayData"];
            var entity = meta[0];
            var seedKind = entity.Get<List<BGEntity>>("Seed");

            // ¾¾¾Ñ °³¼ö
            hub0Txt.text = seedKind[0].Get<int>("count").ToString();
            hub1Txt.text = seedKind[1].Get<int>("count").ToString();
            hub2Txt.text = seedKind[2].Get<int>("count").ToString();

        }
        // ÀÚµ¿ ÀúÀå
        if (!isSaved)
        {
            isSaved = true;
            StartCoroutine(PlantsAutoSave());
        }
    }

    // ÀÚµ¿ÀúÀå
    bool isSaved;
    WaitForSeconds secs = new WaitForSeconds(10f);
    IEnumerator PlantsAutoSave()
    {
        yield return secs;

        // BGDatabase ÀúÀå
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];

        var meta1 = entity.Get<List<BGEntity>>("PlantSlot");
        for (int i = 0; i < plantSlot.Length; i++)
        {
            if (slots[i].isSowed)
            {
                meta1[i].Set("isSowed", true);
                meta1[i].Set("seedNum", slots[i].seedNum);
                meta1[i].Set("time", slots[i].curTime);
            }
            else
            {
                meta1[i].Set("isSowed", false);
                meta1[i].Set<int>("seedNum", slots[i].seedNum);
                meta1[i].Set("time", slots[i].curTime);
            }
        }
        isSaved = false;

    }

    #region ¾¾¾Ñ ÆË¾÷ °ü¸®
    public bool IsPup
    {
        get { return isPup; }
        set { isPup = value; }
    }

    public void ClosePup()
    {
        isPup = false;
        seedPup.SetActive(false);
    }

    public int slotNum;

    public void SowPup(int pupNum)
    {
        Debug.Log("pupNum: " + pupNum);
        slotNum = pupNum;
        seedPup.gameObject.SetActive(true);
        isPup = true;
    }
    #endregion

    #region ¾¾¾Ñ ½É±â ±â´É
    public void SowSeed(int seed)
    {
        slots[slotNum].seedNum = seed;

        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var seedKind = entity.Get<List<BGEntity>>("Seed");
        var meta1 = entity.Get<List<BGEntity>>("PlantSlot");

        if (seedKind[slots[slotNum].seedNum].Get<int>("count") < 1)
        {
            Debug.Log("¾¾¾Ñ ºÎÁ·");
            return;
        }
        else
        {
            seedKind[slots[slotNum].seedNum].Set<int>("count", seedKind[slots[slotNum].seedNum].Get<int>("count") - 1);
            SoundManager.instance.EffectPlay(SoundManager.instance.sowEffect);
        }

        switch (seedNum)
        {
            case 0:
                slots[slotNum].growTime = 20;
                slots[slotNum].slider.maxValue = slots[slotNum].growTime;
                break;
            case 1:
                slots[slotNum].growTime = 30;
                slots[slotNum].slider.maxValue = slots[slotNum].growTime;
                break;
            case 2:
                slots[slotNum].growTime = 40;
                slots[slotNum].slider.maxValue = slots[slotNum].growTime;
                break;
        }

        slots[slotNum].isSowed = true;
        meta1[slotNum].Set("isSowed", true);
        slots[slotNum].curTime = 0;
        meta1[slotNum].Set("time", slots[slotNum].curTime);
        slots[slotNum].icon.color = Color.white;


        seedPup.gameObject.SetActive(false);
        slots[slotNum].slider.gameObject.SetActive(true);
        isPup = false;

        PlantsSave();
        DataManager.instance.SaveFile();

    }
    #endregion

    #region ¾¾¾Ñ ÀúÀå
    public void PlantsSave()
    {
        Debug.Log("PlantsSave()");
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];

        var meta1 = entity.Get<List<BGEntity>>("PlantSlot");
        for (int i = 0; i < plantSlot.Length; i++)
        {
            if (slots[i].isSowed)
            {
                meta1[i].Set("isSowed", true);
                meta1[i].Set("seedNum", slots[i].seedNum);
                meta1[i].Set("time", slots[i].curTime);

            }
            else
            {
                meta1[i].Set("isSowed", false);
                meta1[i].Set<int>("seedNum", slots[i].seedNum);
                meta1[i].Set("time", slots[i].curTime);
            }
        }
    }
    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("PlantsOnPointClick");

        if (isPup) { ClosePup(); }
    }
}

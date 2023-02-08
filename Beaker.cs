using BansheeGz.BGDatabase;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Beaker : MonoBehaviour, IPointerClickHandler
{

    public static Beaker instance;

    public InventorySlot slot0;
    public InventorySlot slot1;
    public InventorySlot slot2;

    public Vector2 basePos;
    public Image img;
    public Image img1;
    public Image img2;

    public Image beakerImg;

    public Transform parent;

    public DB_Potion curPotion;
    public DB_Potion curPotion1;

    public int potionNum;
    public int potionNum1;
    public int addPotionNum;


    public BoxCollider2D beakerCol;
    Vector3 slot0BasePos;
    Vector3 slot1BasePos;
    Vector3 slot2BasePos;

    public GameObject lockPanel;

    public GameObject mixToast;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        basePos = transform.position;
        parent = transform.parent;
        slot0.transform.parent.gameObject.SetActive(false);
        slot1.transform.parent.gameObject.SetActive(false);
        slot2.transform.parent.gameObject.SetActive(false);
        slot2BasePos = slot2.transform.parent.position;

        // 해금시 판넬 제거
        if (DataManager.instance.mixUnlock) Destroy(lockPanel);
    }

    // 비커 슬롯 찼는지 확인
    public bool isSlot0Full;
    public bool isFull;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 비커 슬롯 다 찼을 때 클릭 시 발생 이벤트
        if (isFull)
        {
            MixPotion(potionNum, potionNum1);
            SoundManager.instance.effectAudioSource.clip = SoundManager.instance.mixEffect;
            SoundManager.instance.effectAudioSource.Play();

            slot0.transform.parent.gameObject.SetActive(false);
            slot1.transform.parent.gameObject.SetActive(false);
            isSlot0Full = false;
            isFull = false;
            if (addPotionNum != -1)
            {
                DataManager.instance.AddPotion(addPotionNum);
                StartCoroutine(AddMixPotion());
            }
            else StartCoroutine(Toast());
        }
        else return;
        Debug.Log(gameObject.tag);
    }


    public string potionName;

    // 비커 속 포션 변경
    public void ChangeItem()
    {
        Debug.Log("ChangeItem");
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var potionKind = entity.Get<List<BGEntity>>("Potion");

        for (int i = 0; i < potionKind.Count; i++)
        {
            if (potionKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
            {
                curPotion = DB_Potion.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot0.transform.parent.gameObject.SetActive(true);
                slot0.InitItem(potionKind[i].Get<string>("key"));
                potionNum = potionKind[i].Get<int>("PotionNum");
                isSlot0Full = true;
            }
        }
    }

    // 비커 속 포션 변경
    public void ChangeItem1()
    {
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var potionKind = entity.Get<List<BGEntity>>("Potion");

        for (int i = 0; i < potionKind.Count; i++)
        {
            if (potionKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
            {
                curPotion1 = DB_Potion.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot1.transform.parent.gameObject.SetActive(true);
                slot1.InitItem(potionKind[i].Get<string>("key"));
                potionNum1 = potionKind[i].Get<int>("PotionNum");
                isFull = true;
            }
        }
    }

    // 포션 섞기
    public void MixPotion(int potionNum, int potionNum1) 
    {
        Debug.Log("MixPotion");
        switch (potionNum)
        {
            case 0:
                switch (potionNum1)
                {
                    case 1:
                        addPotionNum = 5;
                        Debug.Log("OrangePotion");
                        break;
                    case 2:
                        addPotionNum = 4;
                        Debug.Log("PuplePotion");
                        break;
                    default:
                        Debug.Log("잘못된 조합");
                        addPotionNum = -1;
                        break;
                }
                break;
            case 1:
                switch (potionNum1)
                {
                    case 0:
                        addPotionNum = 5;
                        Debug.Log("OrangePotion");
                        break;
                    case 2:
                        addPotionNum = 3;
                        Debug.Log("GreenPotion");
                        break;
                    default:
                        addPotionNum = -1;
                        Debug.Log("잘못된 조합");
                        break;
                }
                break;
            case 2:
                switch (potionNum1)
                {
                    case 0:
                        addPotionNum = 4;
                        Debug.Log("PurplePotion");
                        break;
                    case 1:
                        addPotionNum = 3;
                        Debug.Log("GreenPotion");
                        break;
                    default:
                        Debug.Log("잘못된 조합");
                        addPotionNum = -1;
                        break;
                }
                break;
            default:
                Debug.Log("잘못된 조합");
                addPotionNum = -1;
                break;
        }
    }

    WaitForSeconds addMixPotionTime = new WaitForSeconds(0.2f);
    WaitForSeconds addMixPotionTime2 = new WaitForSeconds(0.8f);
    WaitForSeconds addMixPotionTime3 = new WaitForSeconds(0.5f);
    // 섞인 포션 인벤토리 추가
    IEnumerator AddMixPotion()
    {
        var basePos = slot2.transform.parent.position;

        yield return addMixPotionTime;
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var potionKind = entity.Get<List<BGEntity>>("Potion");

        for (int i = 0; i < potionKind.Count; i++)
        {
            if (potionKind[i].Get<int>("PotionNum") == potionKind[addPotionNum].Get<int>("PotionNum"))
            {
                curPotion = DB_Potion.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));

                // 비커 중간에 완성된 포션 이미지 표시
                slot2.transform.parent.gameObject.SetActive(true);

                // 비커 스프라이트 키값에 맞게 변경
                beakerImg.sprite = ExtendFunction.ins.BeakerSpriteReturn(potionKind[i].Get<string>("key"));

                slot2.InitItem(potionKind[i].Get<string>("key"));
            }
        }
        yield return addMixPotionTime2;

        // 포션이 인벤토리 아이콘으로 들어가는 모션
        slot2.transform.parent.transform.DOMove(GameSceneManager.ins.inventoryBtn.position, 0.5f);
        slot2.transform.parent.transform.DOScale(0, 0.5f);
        yield return addMixPotionTime3;

        // 슬롯 크기, 위치 원위치 
        slot2.transform.parent.transform.localScale = new Vector3(1, 1, 1);
        slot2.transform.parent.transform.position = slot2BasePos;
        slot2.transform.parent.gameObject.SetActive(false);

        // 비커 스프라이트 변경
        beakerImg.sprite = ExtendFunction.ins.BeakerSpriteReturn("Mix_Machine(empty)");

        // 인벤토리 새로고침
        if (GameSceneManager.ins.isInventory) Inventory.ins.SetInventory();

        DataManager.instance.SaveFile();
    }


    WaitForSeconds waitSec = new WaitForSeconds(0.8f);

    // 토스트 기능
    public IEnumerator Toast()
    {
        mixToast.SetActive(true);
        yield return waitSec;
        mixToast.SetActive(false);
    }
}

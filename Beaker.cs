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
    public DB_HubPowder curHubPowder;

    public int potionNum;
    public int potionNum1;
    public int addPotionNum;
    public int hubPowderNum;

    public int itemNum;
    public int itemNum1;

    bool isHubPowder;
    bool isPotion;


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
        img = GetComponentInChildren<Image>();
        img1 = GetComponentInChildren<Image>();
        img2 = GetComponentInChildren<Image>();
        basePos = transform.position;
        parent = transform.parent;
        slot0.transform.parent.gameObject.SetActive(false);
        slot1.transform.parent.gameObject.SetActive(false);
        slot2.transform.parent.gameObject.SetActive(false);
        slot2BasePos = slot2.transform.parent.position;
        // 비커가 해금되었다면 lockPanel 제거
        if (DataManager.instance.mixUnlock) Destroy(lockPanel);
    }

    public bool isSlot0Full;
    public bool isFull;

    // IPointer Interface를 통한 클릭 이벤트 관리
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(beakerCol.gameObject.name);
        
        // 비커의 두개의 슬롯이 모두 찼는지 판정
        if (isFull)
        {
            // 약초가 들어가있지 않다면 물약 + 물약 조합법으로 실행
            if (!isHubPowder) MixPotion(potionNum, potionNum1);
            else
            {
                // 약초가 들어가있고, 첫번째 물약이 두번째 물약과 같지 않다면 물약 + 약초 조합법으로 실행.
                if (!isPotion) potionNum = potionNum1;
                HubPotion(potionNum, hubPowderNum);
                isHubPowder = false;
            }
            SoundManager.instance.effectAudioSource.clip = SoundManager.instance.mixEffect;
            SoundManager.instance.effectAudioSource.Play();

            // 첫번째, 두번째 슬롯 SetActive(false)
            slot0.transform.parent.gameObject.SetActive(false);
            slot1.transform.parent.gameObject.SetActive(false);
            
            // 잘못된 조합법이 아니라면 AddMixPotion()실행
            if (addPotionNum != -1)
            {
                SoundManager.instance.effectAudioSource.clip = SoundManager.instance.mixEffect;
                SoundManager.instance.effectAudioSource.Play();

                DataManager.instance.AddPotion(addPotionNum);
                StartCoroutine(AddMixPotion());
            }
            else
            {
                // 잘못된 조합법이라면 토스트 실행
                StartCoroutine(Toast());
                SoundManager.instance.effectAudioSource.clip = SoundManager.instance.wrongEffect;
                SoundManager.instance.effectAudioSource.Play();
            }
            isSlot0Full = false;
            isFull = false;

        }

        else return;
        Debug.Log(gameObject.tag);
    }


    public string potionName;

    // 첫번째 슬롯에 들어간 물약 또는 약초 변경
    public void ChangeItem()
    {
        Debug.Log("ChangeItem");

        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var potionKind = entity.Get<List<BGEntity>>("Potion");
        var hubPowderKind = entity.Get<List<BGEntity>>("HubPowder");

        for (int i = 0; i < potionKind.Count; i++)
        {
            if (potionKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
            {
                curPotion = DB_Potion.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot0.transform.parent.gameObject.SetActive(true);
                slot0.InitItem(potionKind[i].Get<string>("key"));
                potionNum = potionKind[i].Get<int>("PotionNum");
                isPotion = true;
                isSlot0Full = true;
                return;
            }
        }
        for (int i = 0; i < hubPowderKind.Count; i++)
        {
            if (hubPowderKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
            {
                curHubPowder = DB_HubPowder.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot0.transform.parent.gameObject.SetActive(true);
                slot0.InitItem(hubPowderKind[i].Get<string>("key"));
                hubPowderNum = hubPowderKind[i].Get<int>("powderNum");
                isHubPowder = true;
                isSlot0Full = true;
            }
        }

    }

    // 두번째 슬롯에 들어간 물약 또는 약초 변경 
    public void ChangeItem1()
    {
        Debug.Log("ChangeItem1()");

        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var potionKind = entity.Get<List<BGEntity>>("Potion");
        var hubPowderKind = entity.Get<List<BGEntity>>("HubPowder");

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

        for (int i = 0; i < hubPowderKind.Count; i++)
        {
            if (hubPowderKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
            {
                curHubPowder = DB_HubPowder.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot1.transform.parent.gameObject.SetActive(true);
                slot1.InitItem(hubPowderKind[i].Get<string>("key"));
                hubPowderNum = hubPowderKind[i].Get<int>("powderNum");
                isHubPowder = true;
                isFull = true;
            }
        }

    }

    // 물약과 물약 조합
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

    // 물약과 약초 조합
    public void HubPotion(int potionNum, int hubNum)
    {
        switch (potionNum)
        {
            case 0:
                switch (hubNum)
                {
                    case 0:
                        addPotionNum = 6;
                        break;
                    default:
                        Debug.Log("잘못된 조합");
                        addPotionNum = -1;
                        break;
                }
                break;
            case 1:
                switch (hubNum)
                {
                    case 1:
                        addPotionNum = 8;
                        break;
                    default:
                        addPotionNum = -1;
                        Debug.Log("잘못된 조합");
                        break;
                }
                break;
            case 2:
                switch (hubNum)
                {
                    case 2:
                        addPotionNum = 7;
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
 
    // 조합한 포션을 인벤토리에 넣는 기능
    IEnumerator AddMixPotion()
    {
        //var basePos = slot2.transform.parent.position;

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
                slot2.transform.parent.gameObject.SetActive(true);

                // Database에 저장된 key를 통해 비커의 색 변경
                beakerImg.sprite = ExtendFunction.ins.BeakerSpriteReturn(potionKind[i].Get<string>("key"));

                slot2.InitItem(potionKind[i].Get<string>("key"));
            }
        }
        yield return addMixPotionTime2;

        // DOTween을 통해 inventoryBtn의 위치로 조합된 물약 오브젝트의 위치 이동, 물약 오브젝트의 크기를 변경
        slot2.transform.parent.transform.DOMove(GameSceneManager.ins.inventoryBtn.position, 0.5f);
        slot2.transform.parent.transform.DOScale(0, 0.5f);
        yield return addMixPotionTime3;

        // DOTween에 의해 변경된 오브젝트의 크기와 위치를 기존의 위치로 재설정
        slot2.transform.parent.transform.localScale = new Vector3(1, 1, 1);
        slot2.transform.parent.transform.position = slot2BasePos;
        slot2.transform.parent.gameObject.SetActive(false);

        // 비커의 이미지를 빈 이미지로 변경
        beakerImg.sprite = ExtendFunction.ins.BeakerSpriteReturn("Mix_Machine(empty)");

        // 인벤토리 새로고침
        if (GameSceneManager.ins.isInventory) Inventory.ins.SetInventory();

        DataManager.instance.SaveFile();
    }


    WaitForSeconds waitSec = new WaitForSeconds(0.8f);

    // 잘못된 조합으로 물약 생성 시 토스트 생성
    public IEnumerator Toast()
    {
        mixToast.SetActive(true);
        yield return waitSec;
        mixToast.SetActive(false);
    }
}

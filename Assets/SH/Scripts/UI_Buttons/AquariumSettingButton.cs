using Unity.VisualScripting;
using UnityEngine;

public class AquariumSettingButton : MonoBehaviour
{
    ButtonEventComponent buttonEvent;
    FishRespawnList fishRespawnList;
    int randomInt;

    GameObject fishPrefab;

    int fishID;

    private void Start()
    {
        fishRespawnList = GameObject.Find("AquariumObj").GetComponent<FishRespawnList>();
        randomInt =  Random.Range(0, 4);
        buttonEvent = GetComponent<ButtonEventComponent>();
        buttonEvent.ButtonAction(() => InstanciateFish());
    }

    private void Update()
    {
        randomInt = Random.Range(0, 4);

        fishID = gameObject.GetComponentInParent<AquariumInvenItem>().ItemsID;
        switch (fishID)
        {
            case 10:
                fishPrefab = (GameObject)Resources.Load("SH/Prefab/Fish/Blue Tang");
                break;
            case 11:
                fishPrefab = (GameObject)Resources.Load("SH/Prefab/Fish/Clownfish Black");
                break;
            case 12:
                fishPrefab = (GameObject)Resources.Load("SH/Prefab/Fish/Discus");
                break;
            case 13:
                fishPrefab = (GameObject)Resources.Load("SH/Prefab/Fish/FlameAngelfish");
                break;
            case 14:
                fishPrefab = (GameObject)Resources.Load("SH/Prefab/Fish/Purple Tang");

                break;
            
        }
    }

    public void InstanciateFish()
    {
        Instantiate(fishPrefab , fishRespawnList.respawnPos[randomInt]);
    }


}

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
                fishPrefab = (GameObject)Resources.Load("HW/Prefabs/Blue Tang(GameScene)");
                break;
            case 11:
                fishPrefab = (GameObject)Resources.Load("HW/Prefabs/Discus(GameScene)");
                break;
            case 12:
                fishPrefab = (GameObject)Resources.Load("HW/Prefabs/FlameAngelfish(GameScene)");

                break;
            case 13:
                fishPrefab = (GameObject)Resources.Load("HW/Prefabs/Fusilier(GameScene)");

                break;
            case 14:
                fishPrefab = (GameObject)Resources.Load("HW/Prefabs/Clownfish Black(GameScene)");

                break;
            
        }
    }

    public void InstanciateFish()
    {
        Instantiate(fishPrefab , fishRespawnList.respawnPos[randomInt]);
    }


}

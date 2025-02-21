using Game;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    [SerializeField] private GameObject[] allBoxes;
    [SerializeField] private HolderBox[] boxes;
    public HolderBox[] Boxes => boxes;
    public void NextLevelOrReplay(int amountBox)
    {
        foreach (GameObject obj in allBoxes)
        {
            obj.SetActive(false);
        }
        switch (amountBox)
        {
            case 3:
                allBoxes[0].SetActive(true);  
                boxes = allBoxes[0].GetComponentsInChildren<HolderBox>();
                break;
            case 4:
                allBoxes[1].SetActive(true);  
                boxes = allBoxes[1].GetComponentsInChildren<HolderBox>();
                break;
            case 5:
                allBoxes[2].SetActive(true);  
                boxes = allBoxes[2].GetComponentsInChildren<HolderBox>();
                break;
            case 6:
                allBoxes[3].SetActive(true);  
                boxes = allBoxes[3].GetComponentsInChildren<HolderBox>();
                break; 
            case 7:
                allBoxes[4].SetActive(true);  
                boxes = allBoxes[4].GetComponentsInChildren<HolderBox>();
                break;
        }
        foreach (HolderBox box in boxes)
        {
            box.ResetBox();
        }
    }
}

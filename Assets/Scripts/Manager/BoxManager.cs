using Game;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    [SerializeField] private GameObject[] allBoxes;
    [SerializeField] private HolderBox[] boxes;
    private GameObject currentBox;
    public HolderBox[] Boxes => boxes;
    public void NextLevelOrReplay(int amountBox)
    {
        if (currentBox != null)
        {
            currentBox.SetActive(false);
        }
        switch (amountBox)
        {
            case 3:
                currentBox = allBoxes[0];  
                break;
            case 4:
                currentBox = allBoxes[1];  
                break;
            case 5:
                currentBox = allBoxes[2];  
                break;
            case 6:
                currentBox = allBoxes[3];
                break; 
            case 7:
                currentBox = allBoxes[4]; 
                break;
        }
        currentBox.SetActive(true);
        boxes = currentBox.GetComponentsInChildren<HolderBox>();
        foreach (HolderBox box in boxes)
        {
            box.ResetBox();
        }
    }
}

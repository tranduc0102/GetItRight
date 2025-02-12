using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PanelController : MonoBehaviour
    {
       [SerializeField] private GameObject[] objectsMat;
       [SerializeField] private Material[] objectsMatColors;
       [SerializeField] private MeshFilter[] skins;

       public Animator anim;
       public float Ypos;

       
    }
}

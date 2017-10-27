using UnityEngine;

public class TowerBtn : MonoBehaviour {

    [SerializeField]
    private GameObject towerObject;

    public GameObject ToweObject
    {
        get
        {
            return towerObject;
        }
    }
}

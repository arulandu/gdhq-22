// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class borderScript : MonoBehaviour
{
    public GameObject northBorder;
    public GameObject westBorder;
    public GameObject southBorder;
    public GameObject eastBorder;

    public void setSize(uint size) {
        westBorder.transform.position = new Vector3 (-4, 0, 0);
        southBorder.transform.position = new Vector3 (0, 0, -4);
        eastBorder.transform.position = new Vector3 ((int)size * 3 + 12, 0, 0);
        northBorder.transform.position = new Vector3 (0, 0, (int)size* 3 + 12);
    }
}

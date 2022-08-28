// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class borderScript : MonoBehaviour
{
    public GameObject northBorder;
    public GameObject westBorder;
    public GameObject southBorder;
    public GameObject eastBorder;

    public GameObject horizontalFence;
    public GameObject verticalFence;
    public int fenceSize {get;} = 2;

    public int defaultYPos {get;} = 9;
    public int tileSize {get;} = 6;
    private uint size;


    public void setSize(uint size_) {
        
        size = size_;

        westBorder.transform.position = new Vector3 (-4, 0, 0);
        southBorder.transform.position = new Vector3 (0, 0, -4);
        eastBorder.transform.position = new Vector3 ((int)size * 3 + 12, 0, 0);
        northBorder.transform.position = new Vector3 (0, 0, (int)size* 3 + 12);

        this.setFences();
    }

    public void setFences() {
        Debug.Log(size);

        for (int x = -4; x <= (int)size * 3 + 12; x += fenceSize) {
            Instantiate(verticalFence, new Vector3 (x, defaultYPos, -4), Quaternion.Euler(0, 90, 0), transform); //south fence
            Instantiate(verticalFence, new Vector3 (x, defaultYPos, (int)size * 3 + 12), Quaternion.Euler(0, 90, 0), transform); //north fence
        }

        for (int z = -4; z <= (int)size * 3 + 12; z += fenceSize) {
            Instantiate(horizontalFence, new Vector3 (-4, defaultYPos, z), Quaternion.identity, transform); //west fence
            Instantiate(horizontalFence, new Vector3 ((int)size * 3 + 12, defaultYPos, z), Quaternion.identity, transform); //east fence
        }      
    }
}

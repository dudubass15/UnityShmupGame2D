using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    void Start()
    {
        Destroy(gameObject, 0.8f);
        switch (Util.RandInt(0, 2))
        {
            case 0: Util.PlaySound("s_explosion0", volume: 0.3f); break;
            case 1: Util.PlaySound("s_explosion1", volume: 0.5f); break;
            case 2: Util.PlaySound("s_explosion2", volume: 1.0f); break;
        }
    }

    void Update()
    {

    }

}

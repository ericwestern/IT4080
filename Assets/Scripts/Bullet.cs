using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int minZ = -8;
    public int maxZ = 100;

    public ulong spawnerID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < minZ || transform.position.z > maxZ) {
            Destroy(gameObject);
        }
    }
}

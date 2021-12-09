using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class HealthPowerup : NetworkBehaviour
{
    private float rotateSpeed = 50.0f;
    private float travelSpeed = 5.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     void FixedUpdate()
    {
        if(OffPLayingField()) {
            Destroy(gameObject);
        } else {
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.Self);
            transform.Translate(new Vector3(0, 0, -travelSpeed) * Time.fixedDeltaTime, Space.World);
         }
    }

    bool OffPLayingField() {
        return transform.position.z < -6;
    }

}

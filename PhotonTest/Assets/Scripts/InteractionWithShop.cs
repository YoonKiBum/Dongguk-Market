using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionWithShop : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject nearObject;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerStay(Collider other) {
        if(other.tag=="shop"){
           nearObject=other.gameObject;
           Debug.Log(other.gameObject);
       }
        
    }
   void OnTriggerExit(Collider other) {
       if(other.tag=="shop"){
           nearObject=null;
       }
        
    }
}

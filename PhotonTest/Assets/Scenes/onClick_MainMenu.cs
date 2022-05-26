using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onClick_MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject register;
    public GameObject purchase;

    public void register_on_clicked(){
        register.SetActive(true);

    }
    public void register_off_clicked(){
        register.SetActive(false);

    }
    public void Purchase_on_clicked(){
        purchase.SetActive(true);

    }
    public void purchase_off_clicked(){
        purchase.SetActive(false);

    }
    
   
}

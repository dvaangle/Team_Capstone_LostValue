using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    void Start()
    {
        #region Singleton
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

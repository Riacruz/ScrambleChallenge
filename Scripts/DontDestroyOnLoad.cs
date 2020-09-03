using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public static DontDestroyOnLoad estadoJuego;
    // Start is called before the first frame update
    private void Awake()
    {
        if (estadoJuego == null)
        {
            estadoJuego = this;
            DontDestroyOnLoad(gameObject);
            PlayGamesPlatform.Activate();
            PlayGamesPlatform.DebugLogEnabled = false; //OJO desactivar al finalizar

        }
        else if (estadoJuego != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //((PlayGamesPlatform)Social.Active).Authenticate((bool success) => { }, true);

        if (Social.localUser.authenticated)
        {

        }
        else
        {
            Social.localUser.Authenticate((bool success) => { });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

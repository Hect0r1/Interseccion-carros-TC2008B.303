using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System;

[Serializable]
public class CarPoolInitialization : UnityEvent<Parametros>{}

public class RequestManager : MonoBehaviour {

    [SerializeField]
    private CarPoolInitialization _carPoolInitalization;

    [SerializeField]
    private string _urlRequest = "http://127.0.0.1:5000/";

    IEnumerator Start() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        UnityWebRequest www = UnityWebRequest.Get(_urlRequest);
        // Como en cualquier request este asunto es asíncrono
        yield return www.SendWebRequest();
        if(www.result != UnityWebRequest.Result.Success){
            Debug.LogError("RequestManager: Problema con el request");
        } else {
            // print(www.downloadHandler.text);
            // Hacer parsing del JSON
            Parametros listaParametros = JsonUtility.FromJson<Parametros>(
                www.downloadHandler.text
            );
            _carPoolInitalization?.Invoke(listaParametros);
        }
    }
}
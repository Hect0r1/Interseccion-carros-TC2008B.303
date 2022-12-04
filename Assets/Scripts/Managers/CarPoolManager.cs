using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System;

[Serializable] public class RequestConArg : UnityEvent<int>{}

public class CarPoolManager : MonoBehaviour
{

    [SerializeField] private RequestConArg _habilitarCamara;

    public static CarPoolManager Instance {
        get;
        private set;
    }

    [SerializeField] private GameObject _carroOriginal;
    [SerializeField] private CarroSO[] _modelosCarros;
    [SerializeField] private Carro[] _listaCarros;
    [SerializeField] private int fps = 15;
    private GameObject[] _carrosGO;
    private IEnumerator _enumeratorCorrutina;
    private Coroutine _corrutina;
    private Parametros _datos;
    private int t = 0;
    private int step = 0;

    void Awake() {
        // If CarPoolManager already exists, delete it
        if(Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if(_carroOriginal == null){
            throw new System.Exception("CarPoolManager: Es necesario incluir un carro original");
        }

        if(_modelosCarros.Length == 0){
            throw new System.Exception("CarPoolManager: Es necesario incluir modelos de carros");
        }
    }

    public void CarPoolInitialization(Parametros datos) {
        _datos = datos;
        _carrosGO = new GameObject[datos.k];
        for(int i = 0; i < datos.k; i++){
            // Asignar un modelo al prefab Carro
            _carroOriginal.GetComponent<CarroBuilder>()._ScriptableObject = _modelosCarros[UnityEngine.Random.Range(0,_modelosCarros.Length)];
            // Instanciar el Carro con el modelo
           GameObject actual = Instantiate<GameObject>(_carroOriginal);
            // Apagar la cámara
            actual.transform.GetChild(0).gameObject.SetActive(false);
            // Renombrar
            actual.name = i.ToString();
            // Añadir a lista
            _carrosGO[i] = actual;
        }
        _listaCarros = new Carro[_datos.k];
        _enumeratorCorrutina = UpdatePositions(); 
        _corrutina = StartCoroutine(_enumeratorCorrutina);
    }

    IEnumerator UpdatePositions() {
        while(t < _datos.tMax-1){
            for(int i = 0; i < _datos.k; i++){
                if(step == 0) {
                    Vector2 orientation = new Vector2(_datos.puntos[i+((t+1)*_datos.k)].x - _datos.puntos[i+(t*_datos.k)].x, _datos.puntos[i+((t+1)*_datos.k)].z - _datos.puntos[i+(t*_datos.k)].z);
                    if(_carrosGO[i].activeSelf == false) {
                        _carrosGO[i].SetActive(true);
                    } 
                    if(orientation.magnitude > 1.5) {
                        _carrosGO[i].SetActive(false);
                    }
                    if(orientation == Vector2.up) {_carrosGO[i].transform.localEulerAngles = new Vector3(0, 0, 0);}
                    if(orientation == Vector2.one) {_carrosGO[i].transform.localEulerAngles = new Vector3(0, 45, 0);}
                    if(orientation == Vector2.right) {_carrosGO[i].transform.localEulerAngles = new Vector3(0, 90, 0);}
                    if(orientation == new Vector2(1, -1)) {_carrosGO[i].transform.localEulerAngles = new Vector3(0, 135, 0);}
                    if(orientation == Vector2.down) {_carrosGO[i].transform.localEulerAngles = new Vector3(0, 180, 0);}
                    if(orientation == new Vector2(-1, -1)) {_carrosGO[i].transform.localEulerAngles = new Vector3(0, 225, 0);}
                    if(orientation == Vector2.left) {_carrosGO[i].transform.localEulerAngles = new Vector3(0, 270, 0);}
                    if(orientation == new Vector2(-1, 1)) {_carrosGO[i].transform.localEulerAngles = new Vector3(0, 315, 0);}
                } 
                _listaCarros[i] = new Carro();
                _listaCarros[i].id = _datos.puntos[i+(t*_datos.k)].id;
                _listaCarros[i].x = _datos.puntos[i+(t*_datos.k)].x + (((_datos.puntos[i+((t+1)*_datos.k)].x - _datos.puntos[i+(t*_datos.k)].x)/fps) * step);
                _listaCarros[i].z = _datos.puntos[i+(t*_datos.k)].z  + (((_datos.puntos[i+((t+1)*_datos.k)].z - _datos.puntos[i+(t*_datos.k)].z)/fps) * step);
                _carrosGO[_listaCarros[i].id].transform.position = new Vector3(_listaCarros[i].x, 0.4f, _listaCarros[i].z);
            }
            step++;
            // Control Smoothing
            if(step == fps) {
                step = 0;
                t++;
            }
            yield return new WaitForSeconds((1/fps));
        }
        _habilitarCamara?.Invoke(1);
    }
}

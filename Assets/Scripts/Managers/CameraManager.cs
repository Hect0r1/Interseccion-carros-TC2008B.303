using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField]
    private Camera[] _camaras;

    [SerializeField]
    private int _camaraActiva;

    private RaycastHit hit;

    // Instance of CameraManager
    public static CameraManager Instance {
        get;
        private set;
    }

    // If a CameraManager already exists, destroy it
    void Awake()
    {
        if(Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start() 
    {
        HabilitarCamara(_camaraActiva);
    }
    
    public void HabilitarCamara(int camaraAHabilitar) {
        if(camaraAHabilitar < 0 || camaraAHabilitar >= _camaras.Length){
            throw new System.Exception("CameraManager: Es necesario utilizar un índice válido");
        }
        if(_camaras == null){
            throw new System.Exception("CameraManager: Es necesario incluir cámaras");
        }
        _camaraActiva = camaraAHabilitar;
        for(int i = 0; i < _camaras.Length; i++){
            if(i == camaraAHabilitar){
                _camaras[i].gameObject.SetActive(true);
            } else {
                _camaras[i].gameObject.SetActive(false);
            }
        }
    }

    public void IntercambiarCamara() {
        Camera.main.gameObject.SetActive(false); // Deactivate current camera
        _camaraActiva++; // Change active camera index
        _camaraActiva %= _camaras.Length; // Make sure new index is still valid
        HabilitarCamara(_camaraActiva); // Enable new camera
    }

    void Update() {
        // Cuando se presiona un coche
        if ( Input.GetMouseButtonDown (0)){ 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);
            if(hit.collider) {
                if(hit.collider.tag == "car"){
                    Camera.main.gameObject.SetActive(false);
                    hit.collider.transform.parent.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
        // Cuando se cruza una pared
        Vector3 pos = new Vector3(Screen.width/2, Screen.height/4, 0);
        Ray ray2 = Camera.main.ScreenPointToRay(pos);
        Physics.Raycast(ray2, out hit);
        if(hit.collider) {
            if(hit.collider.tag == "wall"){
                Camera.main.gameObject.SetActive(false);
                HabilitarCamara(0);
            }
        }
    }
}

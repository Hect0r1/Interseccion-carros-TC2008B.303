using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarroBuilder : MonoBehaviour
{

    [SerializeField]
    public CarroSO _ScriptableObject;

    private GameObject _carroInterno;

    // Start is called before the first frame update
    void Awake()
    {
        ActualizarCarrito();
    }

    private void ActualizarCarrito() {

        if(_carroInterno != null)
            Destroy(_carroInterno);

        // utilizando los datos construir carrito
        _carroInterno = Instantiate<GameObject>(
            _ScriptableObject.prefabDeModelo, 
            transform.position, 
            transform.rotation,
            transform
            );

        _carroInterno.transform.localScale = new Vector3(
            _ScriptableObject.escala, 
            _ScriptableObject.escala, 
            _ScriptableObject.escala
        );
    }
}

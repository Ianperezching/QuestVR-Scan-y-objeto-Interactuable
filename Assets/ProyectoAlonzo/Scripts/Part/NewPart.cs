using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NewPart : MonoBehaviour
{
    public float weight = 1;

    //
    public float voltage = 22.0f;  // Voltaje único de cada cubo
    public float wireSpeed = 385.0f; // Velocidad de cable única
    private float totalTime = 0.0f;
    private string finalResult = "Esperando...";
    private bool isBeingWelded = false;
    /// 
    

    Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.mass = weight;
    }
    
    
    /// 
    private void Update()
    {
        if (isBeingWelded)
        {
            totalTime += Time.deltaTime;
        }
    }

    public void StartWelding()
    {
        isBeingWelded = true;
    }

    public void StopWelding()
    {
        isBeingWelded = false;
        finalResult = GenerateWeldingResult();
    }

    private string GenerateWeldingResult()
    {
        int successRate = Random.Range(50, 85);
        return successRate + "% regular";
    }

    public float GetTotalTime()
    {
        return totalTime;
    }

    public string GetResult()
    {
        return finalResult;
    }
    /// 


    public void AbsorbPiece(NewPart piece)
    {
        weight += piece.weight;
        rigidbody.mass = weight;

        GameObject obj = piece.gameObject;

       

        //Debug.Log("this: " + this.gameObject.name + " AbsorbPiece-> piece: " + obj.gameObject.name);

        Destroy(obj.GetComponent<XRBaseInteractable>());

        Destroy(obj.GetComponent<Rigidbody>());

        Destroy(obj.GetComponent<NewPart>());

        obj.transform.parent = transform;
    }
}

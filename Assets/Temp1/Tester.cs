using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ReferenceRegistry.GetProvider(hit.collider.gameObject).
                GetAs<NormalMonster>().
                TakeDamage(10);
            }
        }   
    }
}

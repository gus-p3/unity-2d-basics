using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChecaColision : MonoBehaviour
{
    public static bool isPiso = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;
        isPiso = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;
        isPiso = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;
        isPiso = false;
    }
}
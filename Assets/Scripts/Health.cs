using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    public const int maxHealth = 100;

    [SyncVar]
    public int currentHealth = maxHealth;
    //public RectTransform healthBar;


    public void TakeDamage(int amount)
    {
        if (!isServer) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Dead!");

            currentHealth = maxHealth;

            StartCoroutine(HidePlayer());

            RpcRespawn();
        }

        //healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            // move back to zero location
            
            transform.position = Vector3.zero;



            currentHealth = maxHealth;
        }
    }

    IEnumerator HidePlayer()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(2);
        gameObject.GetComponent<Renderer>().enabled = true;
    }
}
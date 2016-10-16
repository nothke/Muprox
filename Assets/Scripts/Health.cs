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

    public bool destroyOnDeath;

    public ParticleSystem deathParticle;
    public AudioClip deathClip;

    public void TakeDamage(int amount)
    {
        if (!isServer) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            deathParticle = PoolingManager.e.explosionParticle;

            if (deathParticle)
            {
                deathParticle.transform.position = transform.position;
                deathParticle.Play();
            }

            if (deathClip)
            {
                deathClip.PlayOnce(transform.position, 1, 1, 90, 5);
            }

            if (destroyOnDeath)
            {


                Destroy(gameObject);



                return;
            }

            currentHealth = 0;
            Debug.Log("Dead!");

            currentHealth = maxHealth;

            if (gameObject.GetComponent<Renderer>())
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

        ConsoleGlobal.Log(gameObject.GetComponent<PlayerController>().nick + " died");
    }

    IEnumerator HidePlayer()
    {

        gameObject.GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(2);
        gameObject.GetComponent<Renderer>().enabled = true;
    }
}
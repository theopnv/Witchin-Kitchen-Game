using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewtManager : MonoBehaviour, IPunchable
{
    Rigidbody m_rb;
    public GameObject m_eyeIngredientPrefab;
    private int m_eyeCount = 2;
    private GameObject[] m_eyes, m_eyesockets, m_eyepatches;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_eyes = new GameObject[] { transform.Find("EyeBall2/Eye2").gameObject, transform.Find("EyeBall1/Eye1").gameObject };
        m_eyesockets = new GameObject[] { transform.Find("EyeBall2/EyeSocket2").gameObject, transform.Find("EyeBall1/EyeSocket1").gameObject };
        m_eyepatches = new GameObject[] { transform.Find("Eyepatch2").gameObject, transform.Find("Eyepatch1").gameObject };
    }

    public void Punch(Vector3 knockVelocity, float stunTime)
    {
        m_rb.AddForce(knockVelocity, ForceMode.VelocityChange);


        if (m_eyeCount > 0)
        {
            //Spawn an eye ingredient
            m_eyeCount--;
            var newEye = Instantiate(m_eyeIngredientPrefab, transform.position, Quaternion.identity);
            var eyeRB = newEye.GetComponent<Rigidbody>();
            eyeRB.AddForce(-0.5f*knockVelocity, ForceMode.VelocityChange);

            //Swap out eye for an eyepatch
            m_eyes[m_eyeCount].SetActive(false);
            StartCoroutine(AddBandage());
        }

        else if (m_eyeCount == 0)
        {
            StartCoroutine(Despawn());
        }
    }

    private IEnumerator AddBandage()
    {
        yield return new WaitForSeconds(0.5f);
        m_eyesockets[m_eyeCount].SetActive(false);
        m_eyepatches[m_eyeCount].SetActive(true);
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(1.5f);
        GameObject.Destroy(this.gameObject);
    }
}

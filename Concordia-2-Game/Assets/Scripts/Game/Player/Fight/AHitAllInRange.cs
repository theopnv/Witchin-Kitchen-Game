using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AHitAllInRange : MonoBehaviour
{
    [SerializeField] protected float m_strength, m_stunTime;

    private List<KeyValuePair<GameObject, IPunchable[]>> m_punchablesInRadius;

    private void Start()
    {
        m_punchablesInRadius = new List<KeyValuePair<GameObject, IPunchable[]>>();
        OnStart();
    }

    protected abstract void OnStart();

    void OnTriggerEnter(Collider other)
    {
        IPunchable[] targetPunchableComponents = other.gameObject.GetComponentsInChildren<IPunchable>();
        if (targetPunchableComponents.Length > 0)
            m_punchablesInRadius.Add(new KeyValuePair<GameObject, IPunchable[]>(other.gameObject, targetPunchableComponents));
    }

    void OnTriggerExit(Collider other)
    {
        KeyValuePair<GameObject, IPunchable[]> leavingObject = m_punchablesInRadius.Find(item => item.Key.Equals(other.gameObject));
        m_punchablesInRadius.Remove(leavingObject);
    }

    public void Hit()
    {
        foreach (KeyValuePair<GameObject, IPunchable[]> target in m_punchablesInRadius)
        {
            foreach (IPunchable punchableComponent in target.Value)
            {
                Vector3 hitVector = (target.Key.transform.position - transform.position) * m_strength;
                punchableComponent.Punch(ModulateHitVector(hitVector), m_stunTime);
            }
        }
        AfterHitting();
    }

    protected abstract Vector3 ModulateHitVector(Vector3 hitVector);
    protected abstract void AfterHitting();
}

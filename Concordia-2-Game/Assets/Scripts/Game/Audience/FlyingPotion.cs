using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyingPotion : MonoBehaviour
{
    private Image m_destination;
    private bool m_holdingposition = true, m_floatUp = true;
    private Transform m_flask;
    private Color m_color;

    void Start()
    {
        StartCoroutine(HoldPostion());
        m_flask = transform.Find("flask");

        var liquid = m_flask.Find("Liquid");
        var matOverrideLiquid = liquid.gameObject.GetComponent<con2.gfx.MaterialOverride>();
        matOverrideLiquid.Entries.Add(new con2.gfx.MaterialOverride.Entry("_Color", m_color));
        matOverrideLiquid.Apply();
    }

    void Update()
    {
        if (!m_holdingposition)
        {
            var destination = Camera.main.ScreenToWorldPoint(m_destination.rectTransform.position + new Vector3(0, 0, Camera.main.transform.position.magnitude));
            transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime);
            var difference = Mathf.Clamp((transform.position - destination).magnitude, 0.0f, 1.0f);
            transform.localScale = new Vector3(difference, difference, difference);
            if (difference <= 0.5)
            {
                GameObject.Destroy(this.gameObject);
            }
        }

        Vector3 pos = m_flask.localPosition;
        if (m_floatUp)
        {
            pos.y += Time.deltaTime * 2;
            m_flask.localPosition = Vector3.Lerp(m_flask.localPosition, pos, 0.5f);
            if (pos.y >= 0.3f)
                m_floatUp = false;
        }
        else
        {
            pos.y -= Time.deltaTime * 2;
            m_flask.localPosition = Vector3.Lerp(m_flask.localPosition, pos, 0.5f);
            if (pos.y <= 0.0f)
                m_floatUp = true;
        }
    }

    public void SetDestination(Image destination)
    {
        m_destination = destination;
    }

    public void SetColor(Color newColor)
    {
        m_color = newColor;
    }

    private IEnumerator HoldPostion()
    {
        yield return new WaitForSeconds(1.0f);
        m_holdingposition = false;
    }
}

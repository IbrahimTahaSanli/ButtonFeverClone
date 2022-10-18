using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPlusController : MonoBehaviour
{
    [SerializeField] private float ShowTimeInSecond;
    [SerializeField] private TMPro.TMP_Text text;

    [HideInInspector] private RectTransform trans;

    // Start is called before the first frame update
    void Start()
    {
        trans = (RectTransform)this.transform;
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = -5;
        trans.position = vec;
    }


    [HideInInspector] private Coroutine coroutine = null;
    public void ShowMoney(string str)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        this.gameObject.SetActive(true);
        text.text = str;
        coroutine = StartCoroutine(HideMoney());
    }

    private IEnumerator HideMoney()
    {
        yield return new WaitForSeconds(ShowTimeInSecond);
        this.gameObject.SetActive(false);
        coroutine = null;
    }
}

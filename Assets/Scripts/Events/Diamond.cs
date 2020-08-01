using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Diamond : MonoBehaviour
{
    public float minLifeTime = 2.0f;
    public float maxLifeTime = 4.0f;
    public float minSize = 0.5f;
    public float maxSize = 1.5f;
    public Sprite skull, diamond;
    private Image buttonImage;
    bool isSkull;
    public GameObject destroyAnimation;

    public void Randomize(bool isSkull)
    {
        buttonImage = GetComponent<Image>();
        this.isSkull = isSkull;

        if (!isSkull)
            buttonImage.sprite = diamond;

        BindButton();

        var parentXSize = transform.parent.GetComponent<RectTransform>().rect.width / 2 - 100f;
        var parentYSize = transform.parent.GetComponent<RectTransform>().rect.height / 2 - 50;

        //Randomize position
        GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-parentXSize, parentXSize),
            Random.Range(-parentYSize, parentYSize));

        transform.localScale = transform.localScale * Random.Range(minSize, maxSize);
        StartCoroutine(FadeOut());
    }

    public void DestroySkullOrDiamond()
    {
        Instantiate(destroyAnimation, transform.parent).GetComponent<RectTransform>().anchoredPosition =
            GetComponent<RectTransform>().anchoredPosition;
        Destroy(gameObject);
    }

    void BindButton()
    {
        if (isSkull)
            GetComponent<Button>().onClick.AddListener(transform.parent.GetComponent<EventClick>().SkullDestroyed);
        else
            GetComponent<Button>().onClick.AddListener(transform.parent.GetComponent<EventClick>().DiamondDestroyed);
    }


    IEnumerator FadeOut()
    {
        var lifetime = Random.Range(minLifeTime, maxLifeTime);
        var elapsedTime = 0.0f;
        while(elapsedTime < lifetime)
        {
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b,
                1 - elapsedTime/lifetime);
            elapsedTime += Time.deltaTime ;
            yield return 0;
        }
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class healthShake : MonoBehaviour
{
    // Start is called before the first frame update
    public HungerBar healthBar;

    public float durationShake = 0.5f;
    Vector3 originalPosition;
    public GameObject blackHeart;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        originalPosition = blackHeart.transform.position;
        StartCoroutine(Shake());
    }
    public IEnumerator Shake() {
        while(healthBar.CurrentHunger <= 20) {
            float elapsedtime = 0f;
            while (elapsedtime < durationShake) {
                float xOffSet = Random.Range(-1f,1f) * 5f;
                float yOffSet = Random.Range(-1f,1f) * 5f;
                transform.position = Vector3.Lerp(transform.position, originalPosition + new Vector3(xOffSet,yOffSet,0), 0.5f);
                elapsedtime += Time.deltaTime;
                yield return null;
            }
            
        }
        transform.position = blackHeart.transform.position;
    }
}

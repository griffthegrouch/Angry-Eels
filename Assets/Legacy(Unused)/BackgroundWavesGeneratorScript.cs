using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundWavesGeneratorScript : MonoBehaviour
{
    public GameObject[] wavePrefabs;
    private int sizeX = 14;
    private int sizeY = 14;

    private GameObject[] squares;

    // Start is called before the first frame update
    void Start()
    {
        squares = new GameObject[sizeX*sizeY];

        //fill a sizeX by sizeY area with all the prefabs
        for (int i = 0; i < squares.Length; i++)
        {
            int waveNum = Random.Range(0, wavePrefabs.Length);

            int posX = i % sizeY;
            
            int posY = (i - sizeX) / sizeY;

            Vector2 pos = new Vector2(posX, posY) * 2;
            pos += new Vector2(-12f,-12f);
            Debug.Log(i + " num:" + waveNum + ", pos:" + posX + "," + posY);
            squares[i] = Instantiate(wavePrefabs[waveNum], pos, new Quaternion(0, 0, 0, 0), this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

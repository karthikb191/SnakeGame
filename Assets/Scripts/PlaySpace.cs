using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Grid
{
    public int rows;
    public int cols;
}


public class PlaySpace : MonoBehaviour
{
    public Grid grid;
    public RenderTexture texture;
    public RenderTexture prevTexture;
    public Material drawTileMaterial;
    public Material otherMat;
    public float indexX = 0;
    public float indexY = 0;

    Texture2D tex;

    Color[] c;
    // Start is called before the first frame update
    void Start()
    {
        
        prevTexture = new RenderTexture(texture);
        prevTexture.Create();
        tex = new Texture2D(256, 256, TextureFormat.RGBA32, false);

        otherMat.SetTexture("_RenderTex", texture);


        drawTileMaterial.SetTexture("_RenderTex", texture);
        drawTileMaterial.SetTexture("_PreviousTexture", Texture2D.blackTexture);
        
    }

    // Update is called once per frame
    private void Update()
    {
        otherMat.SetFloat("boxPositionX", indexX);
        otherMat.SetFloat("boxPositionY", indexY);

        FillTile();
    }
    


    //Tile Fill logic
    void FillTile()
    {
        
        Graphics.Blit(tex, texture, otherMat, 0);
        
        RenderTexture temp = RenderTexture.GetTemporary(texture.descriptor);
        Graphics.Blit(texture, temp, otherMat, 1);


        Graphics.Blit(temp, texture);
        

        otherMat.SetTexture("_PreviousTexture", prevTexture);
        drawTileMaterial.SetTexture("_PreviousTexture", prevTexture);
        Graphics.Blit(texture, prevTexture);
        

        temp.Release();
    }
}

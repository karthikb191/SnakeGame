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
    public GameObject player;
    public Material boardMaterial;
    public Material tileFillerMaterial;

    RenderTexture texture;
    RenderTexture prevTexture;
    

    //Color[] c;
    Vector3 meshExtents;
    Vector3 bottomLeftPoint;
    int totalCellCount;
    HashSet<int> cellSet;
    float cellWidth;
    float cellHeight;
    // Start is called before the first frame update
    void Start()
    {
        totalCellCount = grid.rows * grid.cols;
        cellSet = new HashSet<int>();

        Quaternion rotationStore = transform.rotation;

        //After setting the rotation to identity, store the local values
        transform.rotation = Quaternion.identity;
        meshExtents = GetComponent<MeshRenderer>().bounds.extents;
        bottomLeftPoint = gameObject.transform.position - meshExtents;
        //Get the bottom left point in local coordinates
        bottomLeftPoint = transform.worldToLocalMatrix * bottomLeftPoint;
        meshExtents = transform.worldToLocalMatrix * meshExtents;

        cellWidth = (meshExtents.x * 2) / grid.rows;
        cellHeight = (meshExtents.z * 2) / grid.cols;

        transform.rotation = rotationStore;

        Debug.Log("width and height: " + cellWidth + "  " + cellHeight);
        //To get the correct bottom left point even when the mesh is rotated

        texture = new RenderTexture(256, 256, 0);
        texture.Create();
        prevTexture = new RenderTexture(texture);
        prevTexture.Create();

        tileFillerMaterial.SetTexture("_RenderTex", texture);
        
        boardMaterial.SetTexture("_RenderTex", texture);
        boardMaterial.SetTexture("_PreviousTexture", Texture2D.blackTexture);

        
        Debug.Log("Subscribing");
        //Subscribe to on finished
        GameManager.Instance.GameRestartEvent += ResetTextures;
    }

    // Update is called once per frame
    private void Update()
    {
        //This check makes sure that the FillTile isnt called once the game is over
        if (GameManager.Instance.paused)
            return;

        //Debug.DrawLine(Vector3.zero, bottomLeftPoint + new Vector3(2 * meshExtents.x, 0, 0));

        //Get the current player position and register the appropriate tile as visited\
        RegisterPlayerPosition();
        FillTile();
    }
    
    /// <summary>
    /// This function registers the player position into the hash set and sends the appropriate value 
    /// to the shader to draw the correct tile
    /// </summary>
    void RegisterPlayerPosition()
    {
        //If the cell count in the set is equal to rowsXcol, game is finished
        if (cellSet.Count == totalCellCount)
        {
            Debug.Log("All tiles are green now");
            GameManager.Instance.GameFinished();
        }


        //Calculations relative to the local space of the board, which ensures correct tile is filled even when the board is rotated
        Vector3 position = transform.worldToLocalMatrix * player.transform.position;

        Debug.DrawLine(Vector3.zero, position);

        Vector3 relPosition = position - bottomLeftPoint;
        float x = Mathf.Floor(relPosition.x / (cellWidth));
        float z = Mathf.Floor(relPosition.z / (cellHeight));


        if(x >= 0 && x < grid.rows && z >= 0 && z < grid.cols)
        {
            int index = (int)(z * grid.rows + x);
            if (cellSet.Add(index))
            {
                //Debug.Log("Added: " + index + " count: " + cellSet.Count);
            }

        }

        Vector4 boxDimensions;
        boxDimensions.x = (float)x * 1.0f / grid.rows;
        boxDimensions.y = ((float)x * 1.0f / grid.rows) + 1.0f / grid.rows;
        boxDimensions.z = (float)z * 1.0f / grid.cols;
        boxDimensions.w = ((float)z * 1.0f / grid.cols) + 1.0f / grid.cols;
        //Debug.Log("Box Dimensions" + boxDimensions);
        
        //Set the vector4 value in the shader ---xmin-xmax-ymin-ymax--- in texture coordinates
        tileFillerMaterial.SetVector("boxBounds", boxDimensions);

        //Debug.Log("X: " + x + "   Z: " + z);


        #region static board
        //Static board Logic

        //Vector3 position = player.transform.position;

        //Vector3 localPosition = transform.worldToLocalMatrix * position;

        //Vector3 relPosition = position - bottomLeftPoint;
        //float x = Mathf.Floor(relPosition.x / cellWidth);
        //float z = Mathf.Floor(relPosition.z / cellHeight);

        //Vector4 boxDimensions;
        //boxDimensions.x = (float)x * 1.0f / grid.rows;
        //boxDimensions.y = ((float)x * 1.0f / grid.rows) + 1.0f / grid.rows;
        //boxDimensions.z = (float)z * 1.0f / grid.cols;
        //boxDimensions.w = ((float)z * 1.0f / grid.cols) + 1.0f / grid.cols;
        //Debug.Log("Box Dimensions" + boxDimensions);

        ////Set the vector4 value in the shader ---xmin-xmax-ymin-ymax--- in texture coordinates
        //tileFillerMaterial.SetVector("boxBounds", boxDimensions);

        //Debug.Log("X: " + x + "   Z: " + z);
        #endregion
    }


    //Tile Fill logic
    void FillTile()
    {
        //Fills the proper tile on the plane by blitting textures.
        //additive operation is performed on the prevTexture so that the previously filles tiles are not erased
        Graphics.Blit(Texture2D.whiteTexture, texture, tileFillerMaterial, 0);
        
        RenderTexture temp = RenderTexture.GetTemporary(texture.descriptor);
        Graphics.Blit(texture, temp, tileFillerMaterial, 1);


        Graphics.Blit(temp, texture);
        

        tileFillerMaterial.SetTexture("_PreviousTexture", prevTexture);
        Graphics.Blit(texture, prevTexture);
        

        temp.Release();
    }

    /// <summary>
    /// This is subscribed to GameManager's game reset event. It's called once the button on UI is pressed
    /// </summary>
    void ResetTextures()
    {

        Debug.Log("Render textures have been reset");
        Graphics.Blit(Texture2D.blackTexture, texture);
        Graphics.Blit(Texture2D.blackTexture, prevTexture);
        //texture.Release();
        //prevTexture.Release();
    }
}

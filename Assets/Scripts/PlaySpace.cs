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
    public RenderTexture texture;
    public RenderTexture prevTexture;
    public Material drawTileMaterial;
    public Material otherMat;
    public float indexX = 0;
    public float indexY = 0;


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
        //bottomLeftPoint = Matrix4x4.Rotate(gameObject.transform.rotation) * bottomLeftPoint;
        //bottomLeftPoint = transform.worldToLocalMatrix * bottomLeftPoint;

        

        prevTexture = new RenderTexture(texture);
        prevTexture.Create();

        otherMat.SetTexture("_RenderTex", texture);
        
        drawTileMaterial.SetTexture("_RenderTex", texture);
        drawTileMaterial.SetTexture("_PreviousTexture", Texture2D.blackTexture);
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (cellSet.Count == totalCellCount)
            Debug.Log("All tiles are green now");

        

        Vector3 position = transform.worldToLocalMatrix * player.transform.position;
        
        Debug.DrawLine(Vector3.zero, position);
        
        Vector3 relPosition = position - bottomLeftPoint;
        float x = Mathf.Floor(relPosition.x / (cellWidth));
        float z = Mathf.Floor(relPosition.z / (cellHeight));

        Vector4 boxDimensions;
        boxDimensions.x = (float)x * 1.0f / grid.rows;
        boxDimensions.y = ((float)x * 1.0f / grid.rows) + 1.0f / grid.rows;
        boxDimensions.z = (float)z * 1.0f / grid.cols;
        boxDimensions.w = ((float)z * 1.0f / grid.cols) + 1.0f / grid.cols;
        //Debug.Log("Box Dimensions" + boxDimensions);

        //Set the vector4 value in the shader ---xmin-xmax-ymin-ymax--- in texture coordinates
        otherMat.SetVector("boxBounds", boxDimensions);

        //Debug.Log("X: " + x + "   Z: " + z);



        //Debug.DrawLine(Vector3.zero, bottomLeftPoint + new Vector3(2 * meshExtents.x, 0, 0));
        //Get the current player position and register the appropriate tile as visited\
        //RegisterPlayerPosition();


        otherMat.SetFloat("boxPositionX", indexX);
        otherMat.SetFloat("boxPositionY", indexY);

        FillTile();
    }
    
    /// <summary>
    /// This function registers the player position into the hash set and sends the appropriate value 
    /// to the shader to draw the correct tile
    /// </summary>
    void RegisterPlayerPosition()
    {
        Vector3 position = player.transform.position;

        Vector3 localPosition = transform.worldToLocalMatrix * position;

        Vector3 relPosition = position - bottomLeftPoint;
        float x = Mathf.Floor(relPosition.x / cellWidth);
        float z = Mathf.Floor(relPosition.z / cellHeight);

        Vector4 boxDimensions;
        boxDimensions.x = (float)x * 1.0f / grid.rows;
        boxDimensions.y = ((float)x * 1.0f / grid.rows) + 1.0f / grid.rows;
        boxDimensions.z = (float)z * 1.0f / grid.cols;
        boxDimensions.w = ((float)z * 1.0f / grid.cols) + 1.0f / grid.cols;
        Debug.Log("Box Dimensions" + boxDimensions);

        //Set the vector4 value in the shader ---xmin-xmax-ymin-ymax--- in texture coordinates
        otherMat.SetVector("boxBounds", boxDimensions);

        Debug.Log("X: " + x + "   Z: " + z);
    }


    //Tile Fill logic
    void FillTile()
    {
        
        Graphics.Blit(Texture2D.whiteTexture, texture, otherMat, 0);
        
        RenderTexture temp = RenderTexture.GetTemporary(texture.descriptor);
        Graphics.Blit(texture, temp, otherMat, 1);


        Graphics.Blit(temp, texture);
        

        otherMat.SetTexture("_PreviousTexture", prevTexture);
        drawTileMaterial.SetTexture("_PreviousTexture", prevTexture);
        Graphics.Blit(texture, prevTexture);
        

        temp.Release();
    }
}

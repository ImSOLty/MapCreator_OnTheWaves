using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    public string chosen = "";
    public Sprite show;
    public float rotation = 0;
    public Tile tilePrefab;
    public Tile currentTile;
    public TMP_InputField text;
    public Tile[][] tiles;

    private void Start()
    {
        tiles = new Tile[6][];
        for (int i = 0; i < 6; i++)
        {
            tiles[i] = new Tile[6];
            for (int j = 0; j < 6; j++)
            {
                tiles[i][j] = Instantiate(tilePrefab, transform);
                tiles[i][j].GetComponent<RectTransform>().anchoredPosition =
                    new Vector3(-375 + i * 150, -375 + j * 150, 0);
                tiles[i][j].setXY(i, j);
            }
        }
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            rotation -= 90;
            if (currentTile) currentTile.transform.rotation = Quaternion.Euler(0, 0, rotation);
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            rotation += 90;
            if (currentTile) currentTile.transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    void ShowError(string s)
    {
        text.text = "Error: "+s;
        text.textComponent.color = Color.red;
    }

    public void Generate()
    {
        int amount = 0;
        string s="";
        Tile start = null;
        bool found = false;
        bool checkpointsCheck = false;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (tiles[i][j].tileType.Equals("checkpoint"))
                {
                    checkpointsCheck = true;

                }
                if (tiles[i][j].tileType.Equals("start"))
                {
                    if (found)
                    {
                        ShowError("There are more than 2 starts on track");
                        return;
                    }
                    start = tiles[i][j];
                    found = true;
                }
            }
        }

        if (!found)
        {
            ShowError("There is no start on track");
            return;
        }
        if (!checkpointsCheck)
        {
            ShowError("There should be at least one checkpoint on track");
            return;
        }

        var curCoo = new Vector2Int(start.x, start.y);

        var rotateMatrix = start.transform.eulerAngles.z;
        Tile cur = start;
        do
        {
            s +=cur +""+(-1*rotateMatrix+180)+"~";
            amount++;
            Debug.Log("Matrix rotated on: " + rotateMatrix);
            Vector2Int next = Vector2Int.zero;
            if (cur.tileType.Equals("corner"))
            {
                int delta = Mathf.RoundToInt(cur.transform.eulerAngles.z - rotateMatrix) % 360;
                if (delta < 0 || delta > 180)
                {
                    switch (rotateMatrix)
                    {
                        case 0: case 360: case -360:
                            next = Vector2Int.left;
                            break;
                        case 270: case -90:
                            next = Vector2Int.up;
                            break;
                        case 180: case -180:
                            next = Vector2Int.right;
                            break;
                        case 90: case -270:
                            next = Vector2Int.down;
                            break;
                    }

                    Debug.Log("Turn Left");
                    rotateMatrix += 90;
                    s +=  "False*";
                }
                else
                {
                    switch (rotateMatrix)
                    {
                        case 0: case 360: case -360: 
                            next = Vector2Int.right;
                            break;
                        case 270: case -90:
                            next = Vector2Int.down;
                            break;
                        case 180: case -180:
                            next = Vector2Int.left;
                            break;
                        case 90: case -270:
                            next = Vector2Int.up;
                            break;
                    }

                    Debug.Log("Turn Right");
                    rotateMatrix -= 90;
                    s += "True*";
                }
                rotateMatrix %= 360;
            }
            else
            {
                switch (rotateMatrix)
                {
                    case 0: case -360: case 360:
                        next = Vector2Int.up;
                        break;
                    case 270: case -90:
                        next = Vector2Int.right;
                        break;
                    case 180: case -180:
                        next = Vector2Int.down;
                        break;
                    case 90: case -270:
                        next = Vector2Int.left;
                        break;
                }
                s += "False*";
            }

            if (amount > 200)
            {
                ShowError("Most likely, the track is incorrectly designed.");
                return;
            }
            curCoo.x += next.x;
            curCoo.y += next.y;
            Debug.Log("Next:" + curCoo);
            try
            {
                cur = tiles[curCoo.x][curCoo.y];
            }
            catch (IndexOutOfRangeException)
            {
                ShowError("The track, that starts from \"Start\" is reaching map's range.");
                return;
            }
            if (cur.tileType.Equals(""))
            {
                ShowError("The track, that starts from \"Start\" part not finished: found empty cell");
                return;
            }
        } while (!cur.tileType.Equals("start"));

        if (Mathf.RoundToInt(cur.transform.eulerAngles.z - rotateMatrix) % 360!=0)
        {
            ShowError("Most likely, the track is incorrectly designed.");
            return;
        }

        text.textComponent.color = Color.black;
        var encoded = System.Text.Encoding.UTF8.GetBytes(s);
        text.text = Convert.ToBase64String(encoded);
    }
}
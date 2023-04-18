using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    private Generator generator;
    private Image image;

    [SerializeField] private Sprite emptySprite;
    [SerializeField] private GameObject coinObject;

    private bool isSet = false;
    public string tileType = "";
    private Sprite tileTypeSprite;
    private bool hasCoin = false;

    public int x, y;

    private void Start()
    {
        tileTypeSprite = emptySprite;
        image = GetComponent<Image>();
        generator = FindObjectOfType<Generator>();
    }

    public void OnHover()
    {
        if (isSet && generator.chosen.Equals("coin"))
        {
            coinObject.SetActive(true);
            return;
        }

        if (isSet && !generator.chosen.Equals("delete"))
        {
            return;
        }

        if (!isSet)
        {
            generator.currentTile = this;
            transform.rotation = Quaternion.Euler(0, 0, generator.rotation);
        }

        if (!generator.chosen.Equals(""))
        {
            image.sprite = generator.show;
        }
    }

    public void OnExit()
    {
        if (!hasCoin)
        {
            coinObject.SetActive(false);
        }

        if (isSet || generator.chosen.Equals("delete"))
        {
            image.sprite = tileTypeSprite;
            return;
        }

        transform.rotation = Quaternion.Euler(Vector3.zero);
        image.sprite = emptySprite;
    }

    public void OnClick()
    {
        if (isSet && generator.chosen.Equals("coin"))
        {
            hasCoin = true;
            return;
        }

        if (generator.chosen.Equals("") || generator.chosen.Equals("coin"))
        {
            return;
        }

        if (!isSet && !generator.chosen.Equals("delete"))
        {
            generator.currentTile = null;
            isSet = true;
            image.sprite = generator.show;
            tileType = generator.chosen;
            tileTypeSprite = generator.show;
        }
        else if (generator.chosen.Equals("delete"))
        {
            isSet = false;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            image.sprite = emptySprite;
            tileTypeSprite = emptySprite;
            tileType = "";
            hasCoin = false;
            coinObject.SetActive(false);
        }
    }

    public void setXY(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        string result = "";
        result += tileType.Equals("corner") ? "Corner~" : "Straight~";

        result += tileType.Equals("start") + "~";
        result += (tileType.Equals("checkpoint") || tileType.Equals("start")) + "~";
        result += hasCoin + "~";
        result += (-175 + x * 70) + "~" + (-175 + y * 70) + "~";
        return result;
    }
}
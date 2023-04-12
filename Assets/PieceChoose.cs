using UnityEngine;
using UnityEngine.UI;

public class PieceChoose : MonoBehaviour
{
    public string color;
    public Color redChosen, redUnchosen;
    public Color greenChosen, greenUnchosen;
    public Color yellowChosen, yellowUnchosen;
    public string type;
    private Generator g;
    public PieceChoose[] other;
    private Image _image;

    public void Start()
    {
        _image = GetComponent<Image>();
        g = FindObjectOfType<Generator>();
    }

    public void OnHover()
    {
        _image.color = color switch
        {
            "red" => redChosen,
            "yellow" => yellowChosen,
            _ => greenChosen
        };
    }

    public void OnExit()
    {
        if (!g.chosen.Equals(type))
            _image.color = color switch
            {
                "red" => redUnchosen,
                "yellow" => yellowUnchosen,
                _ => greenUnchosen
            };
    }

    public void OnClick()
    {
        g.rotation = 0;
        g.show = _image.sprite;
        g.chosen = type;
        foreach (var ch in other)
        {
            if (!ch.type.Equals(type))
                ch.RemoveSelection();
        }
    }

    public void RemoveSelection()
    {
        _image.color = color switch
        {
            "red" => redUnchosen,
            "yellow" => yellowUnchosen,
            _ => greenUnchosen
        };
    }
}
using UnityEngine;
using UnityEngine.UI;

public class ImageRenderer : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites = new Sprite[2];
    private Image renderer;
   

    void Start()
    {
        renderer = GetComponent<Image>();
    }

    public void ChangeSprite(int i)
    {
        renderer.sprite = sprites[i];
    }

   
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class AnimatePopup : MonoBehaviour
{
    public Color BackgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);
    public float DestroyTime = 0.5f;
    public bool BlockBackgroundInput = true;
    private GameObject _background;
    private Animator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("AnimatePopup requires an Animator component.");
        }
    }
    
    public void Open()
    {
        gameObject.SetActive(true);
        _animator.Play("Open");
        AddBackground();
    }
    
    public void Close()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            _animator.Play("Close");
        }
        RemoveBackground();
        StartCoroutine(RunPopupDeactivate());
    }
    
    private IEnumerator RunPopupDeactivate()
    {
        yield return new WaitForSeconds(DestroyTime);
        Destroy(_background);
        gameObject.SetActive(false);
    }
    
    private void AddBackground()
    {
        Texture2D bgTex = new Texture2D(1, 1);
        bgTex.SetPixel(0, 0, BackgroundColor);
        bgTex.Apply();
        _background = new GameObject("PopupBackground");
        Image image = _background.AddComponent<Image>();
        Rect rect = new Rect(0, 0, bgTex.width, bgTex.height);
        Sprite sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
        
        // Clone the material, which is the default UI material, to avoid changing it permanently.
        
        image.material = new Material(image.material);
        image.material.mainTexture = bgTex;
        image.sprite = sprite;
        Color newColor = image.color;
        image.color = newColor;
        image.canvasRenderer.SetAlpha(0.0f);
        image.CrossFadeAlpha(1.0f, 0.4f, false);
        image.raycastTarget = BlockBackgroundInput;
        
        Canvas canvas = GetComponentInParent<Canvas>();
        _background.transform.localScale = new Vector3(1, 1, 1);
        _background.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        _background.transform.SetParent(canvas.transform, false);
        _background.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }
    
    private void RemoveBackground()
    {
        if (_background == null) return;
        
        Image image = _background.GetComponent<Image>();
        if (image != null)
        {
            image.CrossFadeAlpha(0.0f, 0.2f, false);
        }
    }
}

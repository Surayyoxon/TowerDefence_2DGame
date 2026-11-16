using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum VolumeSliderType
{
    Music,
    Volume
}

// Unity Slider o'rniga: yashil line (Image) + surish tugmachasi (Image)
public class VolumeSlider : MonoBehaviour
{
    [Header("TYPE")]
    [SerializeField] private VolumeSliderType type = VolumeSliderType.Music;

    [Header("IMAGES")]
    [SerializeField] private Image fillImage;        // yashil line (to'liq uzunlikdagi)
    [SerializeField] private RectTransform handle;   // surish tugmachasi

    [Range(0f, 1f)]
    [SerializeField] private float value = 1f;

    public event Action<float> OnValueChanged;

    public VolumeSliderType Type => type;
    public float Value => value;

    private bool initialized = false;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
        UpdateVisual();
    }

    private void Initialize()
    {
        if (initialized)
            return;

        if (fillImage == null || handle == null)
        {
            Debug.LogError("VolumeSlider: Fill Image yoki Handle ulanmagan! | " + name);
            return;
        }

        initialized = true;

        // Yashil line chapdan o'ngga kamayadigan/ko'payadigan bo'ladi
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;

        // Ham line, ham tugmacha bosilganda/surilganda ishlaydi
        fillImage.raycastTarget = true;

        Image handleImage = handle.GetComponent<Image>();

        if (handleImage != null)
            handleImage.raycastTarget = true;

        AddRelay(fillImage.gameObject);
        AddRelay(handle.gameObject);
    }

    private void AddRelay(GameObject target)
    {
        VolumeSliderRelay relay = target.GetComponent<VolumeSliderRelay>();

        if (relay == null)
            relay = target.AddComponent<VolumeSliderRelay>();

        if (relay.owner != null && relay.owner != this)
        {
            Debug.LogWarning(
                "VolumeSlider: '" + target.name + "' ikkita sliderga ulangan! " +
                "(" + relay.owner.name + " va " + name + ") - har biriga alohida Image kerak."
            );
        }

        relay.owner = this;
    }

    // =====================================
    // VALUE
    // =====================================
    public void SetValueWithoutNotify(float newValue)
    {
        value = Mathf.Clamp01(newValue);
        UpdateVisual();
    }

    public void SetValue(float newValue)
    {
        float clamped = Mathf.Clamp01(newValue);

        if (Mathf.Approximately(clamped, value))
            return;

        SetValueWithoutNotify(clamped);

        if (OnValueChanged != null)
            OnValueChanged.Invoke(value);
    }

    // =====================================
    // INPUT
    // =====================================
    public void SetFromPointer(PointerEventData eventData)
    {
        if (fillImage == null)
            return;

        RectTransform track = fillImage.rectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            track,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            Rect rect = track.rect;

            if (rect.width <= 0f)
                return;

            SetValue(Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x));
        }
    }

    // =====================================
    // VISUAL
    // =====================================
    private void UpdateVisual()
    {
        if (fillImage != null)
            fillImage.fillAmount = value;

        if (fillImage == null || handle == null)
            return;

        // Line'ning chap va o'ng chekkasi (world) orasida tugmachani joylashtirish
        Vector3[] corners = new Vector3[4];
        fillImage.rectTransform.GetWorldCorners(corners);

        float x = Mathf.Lerp(corners[0].x, corners[3].x, value);

        Vector3 position = handle.position;
        position.x = x;
        handle.position = position;
    }
}

// Line va tugmacha bosilganda hodisani VolumeSlider'ga uzatadi
public class VolumeSliderRelay : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [HideInInspector] public VolumeSlider owner;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (owner != null)
            owner.SetFromPointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (owner != null)
            owner.SetFromPointer(eventData);
    }
}
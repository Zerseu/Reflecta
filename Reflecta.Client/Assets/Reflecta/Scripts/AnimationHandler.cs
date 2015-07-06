#region Using

using UnityEngine;
using UnityEngine.UI;

#endregion

[RequireComponent(typeof (Animation))]
public class AnimationHandler : MonoBehaviour
{
    private readonly GUIStyle ComboBoxStyle = new GUIStyle();
    private ComboBox ComboBox;
    private GUIContent[] ComboBoxList;
    private Text Frame;
    private float TempSpeed = 1;

    private void Start()
    {
        GetComponent<Animation>().clip.SampleAnimation(gameObject, 0);

        Frame = GameObject.Find("Frame").GetComponent<Text>();

        ComboBoxList = new GUIContent[GetComponent<Animation>().GetClipCount()];
        var crt = 0;
        foreach (AnimationState a in GetComponent<Animation>())
            ComboBoxList[crt++] = new GUIContent(a.name);

        ComboBoxStyle.normal.textColor = Color.white;
        ComboBoxStyle.onHover.background = new Texture2D(1, 1);
        ComboBoxStyle.hover.background = new Texture2D(1, 1);
        ComboBoxStyle.padding.left = 4;
        ComboBoxStyle.padding.right = 4;
        ComboBoxStyle.padding.top = 4;
        ComboBoxStyle.padding.bottom = 4;

        ComboBox = new ComboBox(new Rect(0, 0, 100, 20), ComboBoxList[0], ComboBoxList, "button", "box", ComboBoxStyle);
    }

    private void Update()
    {
        Frame.text =
            (GetComponent<Animation>()[GetComponent<Animation>().clip.name].time*
             GetComponent<Animation>().clip.frameRate).ToString();
    }

    private void OnGUI()
    {
        var selectedItemIndex = ComboBox.Show();
        var selectedItemName = ComboBoxList[selectedItemIndex].text;
        if (selectedItemName != GetComponent<Animation>().clip.name)
        {
            GetComponent<Animation>().clip = GetComponent<Animation>().GetClip(selectedItemName);
            SetStop();
        }
    }

    public void SetPlay()
    {
        GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed = TempSpeed;
        GetComponent<Animation>().Rewind();
        GetComponent<Animation>().Play();
    }

    public void SetPause()
    {
        if (GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed > 0)
            GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed = 0;
        else
            GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed = TempSpeed;
    }

    public void SetStop()
    {
        GetComponent<Animation>().Stop();
        GetComponent<Animation>().Rewind();
        GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed = TempSpeed;
        GetComponent<Animation>().clip.SampleAnimation(gameObject, 0);
    }

    public void SetSpeed(float speed)
    {
        TempSpeed = speed;
        if (GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed > 0)
            GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed = speed;
    }
}
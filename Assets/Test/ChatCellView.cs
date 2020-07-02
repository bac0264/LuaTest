using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

public class ChatCellView : EnhancedScrollerCellView
{
    public Text message;
    public Image avatar;
    public RectTransform avatarRectransform;
    // Use this for initialization
    public void SetData(ChatPublic chatPublic)
    {
        message.text = chatPublic.playerName;
    }
}

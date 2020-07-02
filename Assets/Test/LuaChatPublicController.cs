using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[System.Serializable]
public class Injection
{
    public string name;
}

public class ChatPublic
{
    public string playerName;
    public int avatarId;
    public int playerLevel;
    public string message;
    public long createdTime;
}
[CSharpCallLua]
public class LuaChatPublicController : MonoBehaviour, IEnhancedScrollerDelegate
{
    //scroller:JumpToDataIndex(count)
    [CSharpCallLua]
    delegate int Jump(int a, int b);

    public List<ChatPublic> _data = new List<ChatPublic>();
    public EnhancedScroller scroller;
    public ChatCellView cellViewPrefab;
    public TextAsset luaScript;
    Injection _dataInjection;
    internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    Action resize;

    public float GetHeightDefault()
    {
        var tempText = cellViewPrefab.message;
        var setting = tempText.GetGenerationSettings(new Vector2(tempText.rectTransform.rect.width, 0));
        var heightText = tempText.cachedTextGenerator.GetPreferredHeight("a", setting);
        return heightText;
    }
    void Start()
    {
        LuaTable scriptEnv = luaEnv.NewTable();
        scriptEnv.Set("scroller", scroller);
        luaEnv.DoString("require 'LuaChatPublicFunction'");
        //scriptEnv.Get("resizePanel", out resize);
        //==scriptEnv.Get("start", out start);
        Jump _jump = luaEnv.Global.Get<Jump>("jump");
        scroller.Delegate = this;
        luaEnv.DoString("require 'LuaChatPublicController'");
        _data.Add(luaEnv.Global.Get<ChatPublic>("ChatPublic"));
        _data.Add(luaEnv.Global.Get<ChatPublic>("ChatPublic"));
        _data.Add(luaEnv.Global.Get<ChatPublic>("ChatPublic"));
        _data.Add(luaEnv.Global.Get<ChatPublic>("ChatPublic"));
        _data.Add(luaEnv.Global.Get<ChatPublic>("ChatPublic"));
        _data.Add(luaEnv.Global.Get<ChatPublic>("ChatPublic"));
        _data.Add(luaEnv.Global.Get<ChatPublic>("ChatPublic"));
        _data.Add(luaEnv.Global.Get<ChatPublic>("ChatPublic"));
        resize();
        int count = _data.Count - 1;
        if (count > 0)
            print(_jump(1, 1));
        foreach (var data in _data)
        {
            print(data.playerName);
        }
    }
    public void AddNewRow(ChatPublic message)
    {
        // first, clear out the cells in the scroller so the new text transforms will be reset
        // scroller.ClearAll();

        // reset the scroller's position so that it is not outside of the new bounds
        // scroller.ScrollPosition = 0;

        // second, reset the data's cell view sizes

        // now we can add the data row
        _data.Add(message);
        ResizeScroller();
        // optional: jump to the end of the scroller to see the new content
        //Jump();
        //StartCoroutine(Jump());
    }
    //private IEnumerator Jump()
    //{
    //    scroller.Velocity = Vector2.down;
    //    yield return new WaitForEndOfFrame();
    //    scroller.JumpToDataIndex(_data.Count - 1, 1f, 1f, true, tweenType: EnhancedScroller.TweenType.linear, isForceJump: true);
    //    scroller.ScrollRect.verticalNormalizedPosition = scroller.ScrollRect.verticalScrollbar.value;
    //}

    private void ResizeScroller()
    {
        scroller.ReloadData(1);
    }

    #region EnhancedScroller Handlers

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        // we pull the size of the cell from the model.
        // First pass (frame countdown 2): this size will be zero as set in the LoadData function
        // Second pass (frame countdown 1): this size will be set to the content size fitter in the cell view
        // Third pass (frmae countdown 0): this set value will be pulled here from the scroller
        var tempText = cellViewPrefab.message;
        var setting = tempText.GetGenerationSettings(new Vector2(tempText.rectTransform.rect.width, 0));
        var heightText = tempText.cachedTextGenerator.GetPreferredHeight(_data[dataIndex].message, setting);
        float _count = heightText / GetHeightDefault();
        return Mathf.Max(cellViewPrefab.avatarRectransform.rect.height, _count * tempText.rectTransform.rect.height + 80);
        //Mathf.Max(_data[dataIndex].cellSize, cellViewPrefab.avatarRectransform.rect.height);
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        ChatCellView cellView = scroller.GetCellView(cellViewPrefab) as ChatCellView;
        cellView.gameObject.SetActive(true);
        cellView.SetData(_data[dataIndex]);
        return cellView;
    }


    #endregion

}

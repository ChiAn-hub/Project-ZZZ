using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager instance;
    public static UIManager Instance//单例
    {
        get
        {
            if (instance == null)
            {
                instance = new UIManager();
            }
            return instance;
        }
    }
    private Transform canvasTransform;
    private GameObject canvasObj;
    public Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                canvasObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Canvas"));
            }
            GameObject.DontDestroyOnLoad(canvasObj);
            canvasTransform = canvasObj.transform;
            return canvasTransform;
        }
    }
    private Dictionary<UIPanelType, string> panelPathDict = new Dictionary<UIPanelType, string>();//用于存放面板名和对应的面板路径
    private Dictionary<UIPanelType, BasePanel> panelPath = new Dictionary<UIPanelType, BasePanel>();//用于存放已经实例化过的面板
    private Stack<BasePanel> panelStack = new Stack<BasePanel>();//用于存放显示的面板
    private UIManager()//构造函数调用配置解析函数
    {
        Parse();
    }
    public void Parse()//解析配置文件
    {
        TextAsset ta = Resources.Load<TextAsset>("Config/UIPanelType");
        UIPanelList jsonObject = JsonUtility.FromJson<UIPanelList>(ta.text);
        foreach (UIPanelInfo info in jsonObject.infoList)
        {
            panelPathDict.Add(info.panelType, info.path);
        }
    }
    private BasePanel GetPanel(UIPanelType panelType)//实例化对应面板
    {
        BasePanel basePanel = null;
        panelPath.TryGetValue(panelType, out basePanel);
        if (basePanel != null)//证明已经实例化过了,显示对应面板
        {
            return basePanel.GetComponent<BasePanel>();
        }
        else//证明没有实例化过面板
        {
            string path = null;
            panelPathDict.TryGetValue(panelType, out path);
            if (path != null)//证明路径存在
            {
                GameObject instancePanel = GameObject.Instantiate(Resources.Load<GameObject>(path), CanvasTransform);//根据路径去实例化对应的面板
                AddComponent(instancePanel, panelType);
                panelPath.Add(panelType, instancePanel.GetComponent<BasePanel>());
                return instancePanel.GetComponent<BasePanel>();
            }
            else//路径不存在
            {
                Debug.LogError("没有找到对应的面板路径");
                return null;
            }
        }
    }
    public void PushPanel(UIPanelType panelType)//将面板压入栈中
    {
        BasePanel basePanel = GetPanel(panelType);

        if (basePanel == null)
        {
            Debug.LogError("打开面板失败：" + panelType);
            return;
        }

        if (panelStack.Count > 0 && panelStack.Peek() == basePanel)
        {
            return;
        }

        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek();
            topPanel.OnPause();
        }

        basePanel.OnEnter();
        panelStack.Push(basePanel);
    }
    public void PopPanel()//将栈顶面板弹出
    {
        if (panelStack.Count <= 0)
        {
            return;
        }
        BasePanel topPanel = panelStack.Pop();
        topPanel.OnExit();
        if (panelStack.Count <= 0)
        {
            return;
        }
        BasePanel newTopPanel = panelStack.Peek();
        newTopPanel.OnResume();
    }
    public void AddComponent(GameObject go, UIPanelType panelType)//添加面板组件
    {
        string stringName = Enum.GetName(typeof(UIPanelType), panelType);
        Type scriptType = Type.GetType(stringName);
        if (scriptType == null)
        {
            Debug.LogError("没有找到面板脚本：" + stringName);
            return;
        }
        if (go.GetComponent(scriptType) == null)
        {
            go.AddComponent(scriptType);
        }
        GetAndAddComponent<CanvasGroup>(go);
    }
    public T GetAndAddComponent<T>(GameObject go) where T : Component
    {
        if (!go.GetComponent<T>())
        {
            go.AddComponent<T>();
        }
        return go.GetComponent<T>();
    }
    public void ClearAllPanel()
    {
        while (panelStack.Count > 0)
        {
            BasePanel panel = panelStack.Pop();

            if (panel != null)
            {
                panel.OnExit();
            }
        }

        foreach (BasePanel panel in panelPath.Values)
        {
            if (panel != null)
            {
                GameObject.Destroy(panel.gameObject);
            }
        }

        panelPath.Clear();
    }
}

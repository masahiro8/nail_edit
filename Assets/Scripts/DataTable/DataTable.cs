using System;
using UnityEngine;

public class DataTable : MonoBehaviour
{
    static DataTable _Instance = null;
    public static DataTable Instance
    {
        get {
            if (_Instance == null) {
                _Instance = FindObjectOfType(typeof(DataTable)) as DataTable;
                if (_Instance == null) {
                    var obj = new GameObject("DataTable");
                    DontDestroyOnLoad(obj);
                    _Instance = obj.AddComponent<DataTable>();
                    _Instance.Setup();
                }
            }
            return _Instance;
        }
    }

    public ParamTable _Param;
    public static ParamTable Param {
        get { return Instance._Param; }
    }

    public LocalizedTable _Localized;
    public static LocalizedTable Localized {
        get { return Instance._Localized; }
    }

    public MenuTable _Menu;
    public static MenuTable Menu {
        get { return Instance._Menu; }
    }

    public MyListTable _MyList;
    public static MyListTable MyList {
        get { return Instance._MyList; }
    }

    public CategoryTable _Category;
    public static CategoryTable Category {
        get { return Instance._Category; }
    }

    public TutorialTable _Tutorial;
    public static TutorialTable Tutorial {
        get { return Instance._Tutorial; }
    }

    public NailInfoTable _NailInfo;
    public static NailInfoTable NailInfo {
        get { return Instance._NailInfo; }
    }

	private void Setup()
    {
        var folder = "Data/";
		_Param = Resources.Load<ParamTable>(folder + "ParamTable");
		// _Localized = Resources.Load<LocalizedTable>(folder + "LocalizedTable");
		_Menu = Resources.Load<MenuTable>(folder + "MenuTable");
		_MyList = Resources.Load<MyListTable>(folder + "MyListTable");
		_Category = Resources.Load<CategoryTable>(folder + "CategoryTable");
		_Tutorial = Resources.Load<TutorialTable>(folder + "TutorialTable");
		_Localized = new LocalizedTable();
        _NailInfo = new NailInfoTable();

        // 初期化
        foreach (MyListType type in Enum.GetValues(typeof(MyListType))) {
            _MyList.Reset(type);
        }
        _Category.Reset();
        _NailInfo.UpdateCategory(_Category.list[_Category.showList[0]].type);
	}
}

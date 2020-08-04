using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
// using Firebase;
// using UniRx;

public class DataTable : MonoBehaviour
{
    static DataTable _Instance = null;
    public delegate void OnChangeNail (string json);
    private OnChangeNail callbacks;

    public void addCallback( OnChangeNail callback) {
        // コールバックを登録します
        callbacks += callback;
    }

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

    public ColorCategoryTable _ColorCategory;
    public static ColorCategoryTable ColorCategory {
        get { return Instance._ColorCategory; }
    }

    public TutorialTable _Tutorial;
    public static TutorialTable Tutorial {
        get { return Instance._Tutorial; }
    }

    public StoreTable _Store;
    public static StoreTable Store {
        get { return Instance._Store; }
    }

    public NailInfoTable _NailInfo;
    public static NailInfoTable NailInfo {
        get { return Instance._NailInfo; }
    }

    // public NailInfoTableApi _NailInfo;
    // public static NailInfoTableApi NailInfo {
    //     get { return Instance._NailInfo; }
    // }

	private void Setup()
    {
        Application.targetFrameRate = 60;

        var folder = "Data/";
		_Param = Resources.Load<ParamTable>(folder + "ParamTable");
		// _Localized = Resources.Load<LocalizedTable>(folder + "LocalizedTable");
		_Menu = Resources.Load<MenuTable>(folder + "MenuTable");
		_MyList = Resources.Load<MyListTable>(folder + "MyListTable");
		// _Category = Resources.Load<CategoryTable>(folder + "CategoryTable");
		_ColorCategory = Resources.Load<ColorCategoryTable>(folder + "ColorCategoryTable");
		_Tutorial = Resources.Load<TutorialTable>(folder + "TutorialTable");
		_Store = Resources.Load<StoreTable>(folder + "StoreTable");
		_Localized = new LocalizedTable();
        _Category = new CategoryTable();
        _NailInfo = new NailInfoTable();
        // _NailInfo = new NailInfoTableApi();

        // 初期化（順番に注意）
        _Param.Setup();
        foreach (MyListType type in Enum.GetValues(typeof(MyListType))) {
            _MyList.Reset(type);
        }
        _Category.Reset();
        _ColorCategory.Reset();
        _Store.Reset();
        // _NailInfo.UpdateCategory();

        // // 初期選択ネイル
        // _Param.selectedNail.Value = _NailInfo.list.Length > 0 ? _NailInfo.list[0] : null;

        // Debug.Log(DataTable.Param.releaseDate);
        // DataTable.Param.releaseDate = new System.DateTime(1976, 2, 3);
        Debug.Log("Now date: " + System.DateTime.Now);

        // SetupCrashlytics();
	}
}

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
                    _Instance = obj.AddComponent<DataTable>();
                    _Instance.Setup();
                    DontDestroyOnLoad(obj);
                }
            }
            return _Instance;
        }
    }

    public ParamTable _Param;
    public static ParamTable Param {
        get { return Instance._Param; }
    }

    public MenuTable _Menu;
    public static MenuTable Menu {
        get { return Instance._Menu; }
    }

    public NailTable _Nail;
    public static NailTable Nail {
        get { return Instance._Nail; }
    }

    public CategoryTable _Category;
    public static CategoryTable Category {
        get { return Instance._Category; }
    }

	private void Setup()
    {
        var folder = "Data/";
		_Param = Resources.Load<ParamTable>(folder + "ParamTable");
		_Menu = Resources.Load<MenuTable>(folder + "MenuTable");
		_Nail = Resources.Load<NailTable>(folder + "NailTable");
		_Category = Resources.Load<CategoryTable>(folder + "CategoryTable");
	}
}

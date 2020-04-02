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

    public NailTable _Nail;
    public static NailTable Nail {
        get { return Instance._Nail; }
    }

    // public static BaseRecord[] GetList(DataType type)
    // {
    //     switch (type) {
    //     case DataType.Nail:
    //         return Nail.list;
    //     default:
    //         return Nail.list;
    //     }
    // }

	private void Setup()
    {
		_Nail = Resources.Load<NailTable>("NailTable");
	}
}

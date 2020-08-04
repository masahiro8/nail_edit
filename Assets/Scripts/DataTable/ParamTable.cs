using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/ParamTable", fileName = "ParamTable")]
public class ParamTable : ScriptableObject
{
    [SerializeField, HeaderAttribute("APIにアクセスしない起動")]
    public bool noUseAPI = false;

    [SerializeField, HeaderAttribute("テスト画像を使う")]
    public bool useDummyImage = false;

    [SerializeField, HeaderAttribute("テスト解析データを使う")]
    public bool useDummyDetection;

    [SerializeField, HeaderAttribute("共通のアニメーション時間")]
    public float duration = 0.1f;

    [SerializeField, HeaderAttribute("選択バー移動時のアニメーション時間")]
    public float selectedBarDuration = 0.2f;

    [SerializeField, HeaderAttribute("発売日からNEW表示を付ける日数")]
    public int newInfoDays = 30;

    [SerializeField, HeaderAttribute("非表示にするカテゴリID")]
    public int hideCategoryId = 9000;

    [SerializeField, HeaderAttribute("トップコートMetallic")]
    public float[] topcoatMetallicPer;

    [SerializeField, HeaderAttribute("トップコートSmoothness")]
    public float[] topcoatSmoothnessPer;

    public ReactiveProperty<NailInfoRecord> selectedNail = new ReactiveProperty<NailInfoRecord>(null);
    public ReactiveProperty<NailFilterType> filterType = new ReactiveProperty<NailFilterType>(NailFilterType.All);
    public ReactiveProperty<NailTopcoatType> topcoatType = new ReactiveProperty<NailTopcoatType>(NailTopcoatType.None);

    public ReactiveProperty<MainViewType> mainView = new ReactiveProperty<MainViewType>(MainViewType.Movie);
    public ReactiveProperty<ShootHandType> shootHand = new ReactiveProperty<ShootHandType>(ShootHandType.Left);
    public ReactiveProperty<ShootModeType> shootMode = new ReactiveProperty<ShootModeType>(ShootModeType.HandPaa);

    // エディット時のパラメータ変化を受け取るため
#if UNITY_EDITOR
    public ReactiveProperty<float> validateTime = new ReactiveProperty<float>(0);

    public void OnValidate()
    {
        // Debug.Log(Time.time);
        validateTime.Value = Time.time;
    }
#endif

    // 設定
    public void Setup()
    {
        // 絞り込みフィルター
        filterType.Value = (NailFilterType)SaveName.NailItemFilter.GetInt((int)filterType.Value);
        filterType
            .SkipLatestValueOnSubscribe()
            .Subscribe(type => {
                SaveName.NailItemFilter.SetInt((int)type);
            })
            .AddTo(DataTable.Instance.gameObject);

        // トップコート
        topcoatType.Value = (NailTopcoatType)SaveName.NailItemTopcoat.GetInt((int)topcoatType.Value);
        topcoatType
            .SkipLatestValueOnSubscribe()
            .Subscribe(type => {
                SaveName.NailItemTopcoat.SetInt((int)type);
            })
            .AddTo(DataTable.Instance.gameObject);

        // 手の方向選択
        shootHand.Value = (ShootHandType)SaveName.ShootHand.GetInt((int)shootHand.Value);
        shootHand
            .SkipLatestValueOnSubscribe()
            .Subscribe(type => {
                SaveName.ShootHand.SetInt((int)type);
            })
            .AddTo(DataTable.Instance.gameObject);

        // 撮影モード選択
        shootMode.Value = (ShootModeType)SaveName.ShootMode.GetInt((int)shootMode.Value);
        shootMode
            .SkipLatestValueOnSubscribe()
            .Subscribe(type => {
                SaveName.ShootMode.SetInt((int)type);
            })
            .AddTo(DataTable.Instance.gameObject);
    }
}

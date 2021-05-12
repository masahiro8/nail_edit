# Unity Edit 用 CSV 変換

csv変換する前にネイルチップとボトルの画像の名前を正しいものに変更する

### 1.ファイル名を変換

```
DLMI_BL923_color.png -> DLMI_BL923.png
```

sc_png_rename.jsを開き、画像パスを設定してnodeコマンド

```
node sc_png_rename.js
```

で変換完了

ちなみに sc_png_rename.jsを実行すると、このあとに使用する**newList.csv**を書き換えるための**out_newList.csv**も一緒に作成する



### 2.ファイル名からファイルリストのcsvを作る

名前変更したファイルをGUI上で選択して、./csv/newList.csvを空にしてから、中でペーストするとファイル名の一覧が作られる。そのまま上書き保存。

もしくは、YUMEMIからもらったエクセルをもとに、/work_files/テクスチャ名一覧作成用ファイル.xlsx を使って、そこから./csv/newList.csvを作る

### 3.csv変換

```
node index.js
```

を実行すると、csvの中のout_NailProduct.csvとout_NailCategory.csvに出力される。

unityの /Assets/Resources/CSV/ の中の、NailProduct.csvとNailCategory.csvの最後に追加して保存して完了

**※ 以降何か変更するときはcsvファイルをエクセルで開いて修正すればOK**



# ダミーのシェーダファイルを作成する

/csv/newlist.csvを最新にする

```
node material_rename.js
```



# APIのnailsからアプリ内で使うシリーズid対応表を変換する



### 1.POSTMANでApi の nailsからデータをとってくる

```
https://api.app-dev.nailholic.net/api/v1/nails?os_type=2&app_version=1.0.4
```



### 2.とってきたjsonをコピペして保存

```
./master_json/nails.json
```



### 3.変換

```
node nailsToApp.js
```



###4.変換されたデータをjson形式に保存

```
./json/out_nails_table.txt
```



### 5.Unityのソースを書き換え

```
Assets/Scripts/HTTP/apiNails.cs
```

の25行目から書き換える




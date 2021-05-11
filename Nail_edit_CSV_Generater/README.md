# Unity Edit 用 CSV 変換



csv変換する前にネイルチップとボトルの画像の名前を正しいものに変更する

### 1.ファイル名を変換

```
DLMI_BL923_color.png -> DLMI_BL923.png
```

ここはbrewのrenameでコマンドから実行する

```
rename 's/(.*)_(.*)_color.png/$1_$2.png/' ./*
```

こんな感じで正規表現でマッチさせて置き換えする



### 2.ファイル名からファイルリストのcsvを作る

名前変更したファイルをGUI上で選択して、./csv/newList.csvを空にしてから、中でペーストするとファイル名の一覧が作られる。そのまま上書き保存。

### 3.csv変換

```
node index.js
```

を実行すると、csvの中のout_NailProduct.csvとout_NailCategory.csvに出力される。

unityの /Assets/Resources/CSV/ の中の、NailProduct.csvとNailCategory.csvの最後に追加して保存して完了

**※ 以降何か変更するときはcsvファイルをエクセルで開いて修正すればOK**



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




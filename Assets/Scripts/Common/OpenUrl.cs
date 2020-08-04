using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class OpenUrl
{
    public static void productCode(NailInfoRecord data){
        // //ストア番号取得
        // int n = SaveName.StoreSelect.GetInt();
        // //商品を検索
        // var data = DataTable.NailInfo.list
        //                 .Where(v => v.productCode == code)
        //                 .ToArray();
        // if( data[0].url[n].Length > 0 ) {
        //     Application.OpenURL(data[0].url[n]);
        // }
        if (data.useURL.Length > 0) {
            Application.OpenURL(data.useURL);
        }
    }
}

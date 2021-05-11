const fs = require("fs");
const readline = require("readline");

/**
 * newlistから仮のシェーダーアセットファイルを作る
 */
const main = async () => {
  const filelist = "./csv/newlist.csv";
  //ファイル読み込み
  const stream = fs.createReadStream(filelist, "utf8");
  let reader = readline.createInterface({ input: stream });
  for await (const data of reader) {
    const _data = data.match(/(.*)_(.*)(\.png)/);
    //${_data[2]}
    fs.copyFile(
      "./material/_src.asset",
      `./material/${_data[2]}.asset`,
      (err) => {
        if (err) throw err;
        console.log(`copy ${_data[2]}.asset`);
      }
    );
  }
};

main();

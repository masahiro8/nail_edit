const fs = require("fs");
const readline = require("readline");

/**
 * newlistから仮のボトル・ネイルチップ画像を作る
 */
const main = async () => {
  const filelist = "./csv/newlist.csv";
  //ファイル読み込み
  const stream = fs.createReadStream(filelist, "utf8");
  let reader = readline.createInterface({ input: stream });
  for await (const data of reader) {
    fs.copyFile("./png/_src_bottle.png", `./png/bottle/${data}`, (err) => {
      if (err) throw err;
      console.log(`copy bottle ${data}`);
    });
    fs.copyFile("./png/_src_sample.png", `./png/bottle/${data}`, (err) => {
      if (err) throw err;
      console.log(`copy sample ${data}`);
    });
  }
};

main();

const fs = require("fs");
const readline = require("readline");

/**
 * newlistから仮のボトル・ネイルチップ画像を作る
 */

const bottleFrom = "./png/2021_07/bottle";
const bottleTo = "./png/2021_07/bottle_rename";

const sampleFrom = "./png/2021_07/sample";
const sampleTo = "./png/2021_07/sample_rename";

const newList = "./csv/out_newList.csv";

/**
 * ボトル画像はこの形式を
 * DLMI034_GY034.png
 *
 * この形式に変換
 * DLMI_GY034.png
 */
const bottle = () => {
  fs.readdir(bottleFrom, (err, files) => {
    files.map((file) => {
      const _file = file.match(/(.*)_(.*)(\.png)/);
      const _prefix = _file[1].match(/([A-z)]{4})(\d{3})/);
      console.log(`${bottleTo}/${_prefix[1]}_${_file[2]}.png`);
      fs.copyFile(
        `${bottleFrom}/${file}`,
        `${bottleTo}/${_prefix[1]}_${_file[2]}.png`,
        (err) => {
          if (err) throw err;
        }
      );
    });
  });
};

/**
 * ネイル画像はこの形式を
 * DLMI034_GY034_color.png
 *
 * この形式に変換
 * DLMI_GY034.png
 */
const sample = () => {
  fs.readdir(sampleFrom, (err, files) => {
    files.map((file) => {
      const _file = file.match(/(.*)_(.*)_color(\.png)/);
      if (_file) {
        console.log(`${sampleTo}/${_file[1]}_${_file[2]}.png`);
        fs.copyFile(
          `${sampleFrom}/${file}`,
          `${sampleTo}/${_file[1]}_${_file[2]}.png`,
          (err) => {
            if (err) throw err;
          }
        );
      }
    });
  });
};

const newListOut = async () => {
  let csv_product = "";
  await fs.readdir(sampleTo, (err, files) => {
    files.map((file) => {
      csv_product += `${file}\n`;
      console.log(file);
    });
    fs.writeFile(newList, csv_product, (error) => {
      console.log("NailProduct出力しました。");
    });
  });
};

console.log("Rename bottle image");
bottle();
console.log("Rename sample image");
sample();
newListOut();

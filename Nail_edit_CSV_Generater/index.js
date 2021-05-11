const fs = require("fs");
const readline = require("readline");

const main = async () => {
  const filelist = "./csv/newlist.csv";
  const category_name = "2021年春新色";
  const category_display_name = "2021年春新色";
  const category_id = "8000";
  let counter = 247;
  let csv_product = "";
  let csv_category = "";
  let csv_ecurl = "";

  //ファイル読み込み
  const stream = fs.createReadStream(filelist, "utf8");

  //1行づつ読み込み
  let reader = readline.createInterface({ input: stream });
  for await (const data of reader) {
    const _data = data.match(/(.*)_(.*)(\.png)/);
    const split = _data[2].match(/([A-Z]{2})(\d{3})/);
    const line_category = `${counter},${category_name},,${_data[2]},${category_id},${category_name},青,${category_name},,`;
    const line_product = `${counter},${category_name},,${_data[2]},,,,${_data[1]}${split[2]},¥999,2020/02/01,2099/12/31,0,2020/02/01,,${category_id},${category_name},${category_display_name}`;
    const line_ecurl = `${counter},${category_name},${_data[1]}${split[2]},,,,,,,,,,,,,,,,,,,,,,,`;
    csv_product += `${line_product}\n`;
    csv_category += `${line_category}\n`;
    csv_ecurl += `${line_ecurl}\n`;
    counter++;
  }

  fs.writeFile("./csv/out_NailProduct.csv", csv_product, (error) => {
    console.log("NailProduct出力しました。");
  });
  fs.writeFile("./csv/out_NailCategory.csv", csv_category, (error) => {
    console.log("NailCategory出力しました。");
  });
  fs.writeFile("./csv/out_NailECUEL.csv", csv_ecurl, (error) => {
    console.log("NailECURL出力しました。");
  });
};

main();

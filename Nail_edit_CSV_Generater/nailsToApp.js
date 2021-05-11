const fs = require("fs");
const json = require("./master_json/nails.json");

const main = () => {
  const list = json.nails.map((item) => {
    const n =
      +item.series_code > 10000
        ? `${+item.series_code - 10000}`
        : item.series_code;
    return {
      [item.color_code]: n,
    };
  });

  let _json = "";
  list.map((item) => {
    _json = _json + JSON.stringify(item).replace(/\:/g, ",") + ",\n";
  });

  fs.writeFile("./json/out_nails_table.txt", _json, (error) => {
    console.log("nails table出力しました。");
  });
};
main();

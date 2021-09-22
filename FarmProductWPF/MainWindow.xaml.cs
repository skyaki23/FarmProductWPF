using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FarmProductWPF
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        //解析農產品產地價格(月平均價格)資料json URL
        static string jsonFarmProductDataURL = "https://data.coa.gov.tw/OpenData/Service/OpenData/DataFileService.aspx?UnitId=652";

        //FarmProductDataClass List
        List<FarmProductDataClass> farmProductDataList = new List<FarmProductDataClass>();

        //農產品產地價格(月平均價格)資料json檔key Array
        string[] farmProductKeyArr = { "作物", "年份", "1月價格", "2月價格", "3月價格", "4月價格", "5月價格", "6月價格"
                                    , "7月價格", "8月價格", "9月價格", "10月價格", "11月價格", "12月價格"};

        //年份篩選List
        List<string> cbFilterYearList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            //開啟後畫面置中
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //設定 TLS(Transport Layer Security) 安全性通訊協定 版本
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            cbFilterYearList.Add(""); // 加入至年份篩選List，此選項為還原預設用

            //farmProduct_Data為爬取jsonFarmProductDataURL後的資料
            string farmProduct_Data = GetResponse(jsonFarmProductDataURL);

            ParseData_jsonFarmProductDataURL(farmProduct_Data);
        }

        //爬取URL資料並成字串做回傳
        static string GetResponse(string url)
        {
            string ResponseData = "";
            try
            {
                //WebRequest
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.Timeout = 60000; // 預設逾時60秒
                webRequest.Method = "GET";

                //WebResponse
                WebResponse webResponse = webRequest.GetResponse();

                //Stream
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                    ResponseData = streamReader.ReadToEnd(); // 讀取資料塞給ResponseData
                    streamReader.Close();
                    responseStream.Close();
                }

                //關閉webResponse
                webResponse.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ResponseData;
        }

        //解析json資料(jsonFarmProductDataURL)，農產品產地價格(月平均價格)資訊
        void ParseData_jsonFarmProductDataURL(string data)
        {
            JArray jArray = JArray.Parse(data); // 將資料轉成JArray，會有多組陣列

            //foreach取出jArray陣列每一筆資料
            foreach (JObject jo in jArray)
            {
                for (int i = 2; i < farmProductKeyArr.Length; i++)
                {
                    jo[farmProductKeyArr[i]] = (string.IsNullOrEmpty(jo[farmProductKeyArr[i]].ToString()) ||
                                                "-".Equals(jo[farmProductKeyArr[i]].ToString())) ?
                                                "0" : jo[farmProductKeyArr[i]];
                }
                
                //建立FarmProductDataClass物件，將物件屬性做設定
                FarmProductDataClass farmProductData = new FarmProductDataClass();
                farmProductData.name = jo[farmProductKeyArr[0]].ToString();
                farmProductData.year = jo[farmProductKeyArr[1]].ToString();
                farmProductData.price_Jan = Convert.ToDouble(jo[farmProductKeyArr[2]]);
                farmProductData.price_Feb = Convert.ToDouble(jo[farmProductKeyArr[3]]);
                farmProductData.price_Mar = Convert.ToDouble(jo[farmProductKeyArr[4]]);
                farmProductData.price_Apr = Convert.ToDouble(jo[farmProductKeyArr[5]]);
                farmProductData.price_May = Convert.ToDouble(jo[farmProductKeyArr[6]]);
                farmProductData.price_Jun = Convert.ToDouble(jo[farmProductKeyArr[7]]);
                farmProductData.price_Jul = Convert.ToDouble(jo[farmProductKeyArr[8]]);
                farmProductData.price_Aug = Convert.ToDouble(jo[farmProductKeyArr[9]]);
                farmProductData.price_Sep = Convert.ToDouble(jo[farmProductKeyArr[10]]);
                farmProductData.price_Oct = Convert.ToDouble(jo[farmProductKeyArr[11]]);
                farmProductData.price_Nov = Convert.ToDouble(jo[farmProductKeyArr[12]]);
                farmProductData.price_Dec = Convert.ToDouble(jo[farmProductKeyArr[13]]);

                farmProductDataList.Add(farmProductData); // 農產品產地價格(月平均價格)資料進List
                cbFilterYearList.Add(farmProductData.year); // 將月份資料進List
            }

            //去除重複月份資料，再將List繫結至月份篩選下拉式選單
            cbFilterYearList = cbFilterYearList.Distinct().ToList();
            cbYear.ItemsSource = cbFilterYearList;

            dgFarmProduct.ItemsSource = farmProductDataList; // List資料繫結至dgFarmProduct

            AdjustDG();
        }

        //查詢按鈕點選時
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            FilterData();
        }

        //選取年份篩選下拉式選單時
        private void cbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterData();
        }

        //篩選農產品產地價格(月平均價格)資料
        private void FilterData()
        {
            //依照輸入的作物、年份做篩選並繫結至dgFarmProduct
            var result = farmProductDataList.Where(f => f.name.Contains(txtName.Text.ToLower())).ToList();
            result = result.Where(f => f.year.Contains(cbYear.SelectedItem.ToString())).ToList();
            dgFarmProduct.ItemsSource = result;

            AdjustDG();
        }

        //調整欄位表頭文字及寬度
        private void AdjustDG()
        {
            //修改dgFarmProduct Header text
            for (int i = 0; i < dgFarmProduct.Columns.Count; i++)
            {
                dgFarmProduct.Columns[i].Header = farmProductKeyArr[i];

                //調整欄位寬度
                switch (i)
                {
                    case 0:
                        dgFarmProduct.Columns[i].Width = 230;
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                        dgFarmProduct.Columns[i].Width = 90;
                        break;
                }
            }
        }

        //FarmProductDataClass類別
        public class FarmProductDataClass
        {
            //作物
            public string name { get; set; }

            //年份
            public string year { get; set; }

            //1月價格
            public double price_Jan { get; set; }

            //2月價格
            public double price_Feb { get; set; }

            //3月價格
            public double price_Mar { get; set; }

            //4月價格
            public double price_Apr { get; set; }

            //5月價格
            public double price_May { get; set; }

            //6月價格
            public double price_Jun { get; set; }

            //7月價格
            public double price_Jul { get; set; }

            //8月價格
            public double price_Aug { get; set; }

            //9月價格
            public double price_Sep { get; set; }

            //10月價格
            public double price_Oct { get; set; }

            //11月價格
            public double price_Nov { get; set; }

            //12月價格
            public double price_Dec { get; set; }
        }
    }
}

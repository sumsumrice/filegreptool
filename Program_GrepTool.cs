using System;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;

namespace msgbox
{
     class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog ofDialog = new OpenFileDialog();
            String file_path = null;

            // デフォルトのフォルダを指定する
            ofDialog.InitialDirectory = @"C:";

            // ダイアログのタイトルを指定する
            ofDialog.Title = "ダイアログのタイトル";

            // ダイアログを表示する
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                file_path = ofDialog.FileName;
            }
            else
            {
                Console.WriteLine("キャンセルされました");
                return;
            }
            // オブジェクトを破棄する
            ofDialog.Dispose();
            // ファイル一括読込
            ArrayList list = new ArrayList();
            string line = "";
            Console.WriteLine(file_path+ "を読み込んでいます・・.");
            using (System.IO.StreamReader file = new System.IO.StreamReader(file_path,System.Text.Encoding.GetEncoding("shift_jis")))
            {
                // test.txtを1行ずつ読み込んでいき、末端(何もない行)までwhile文で繰り返す
                while ((line = file.ReadLine()) != null)
                {
                    list.Add(line);
                }
                Console.WriteLine( "ファイルを読み込みが完了しました.");
            }

            System.Threading.Thread.Sleep(2000);
            // エンキュー・デキュー行の仕分け⇒それぞれ配列作成
            ArrayList enqueue_lines = new ArrayList();
            ArrayList dqueue_lines = new ArrayList();

            foreach(string lines in list){
                Match enqueue_matchs = Regex.Match(lines, "エンキュー回数");
                Match dqueue_matchs = Regex.Match(lines, "デキュー回数");

                // 仕分け
                if(enqueue_matchs.Success){
                    enqueue_lines.Add(lines);
                }
                if(dqueue_matchs.Success) {
                    dqueue_lines.Add(lines);
                }
            }
            // エンキューデキューの時刻取得⇒比較
            ArrayList time_difference = new ArrayList();

                Console.WriteLine( "スループットを測定します..");
                int diff_cnt = enqueue_lines.Count.CompareTo(dqueue_lines.Count);
                if(diff_cnt == 1){
                    for(int i = 0; i < dqueue_lines.Count; i++){
                    // 一行取得
                    string e_str = enqueue_lines[i] as string;
                    string d_str = dqueue_lines[i] as string;
                    // ToDo小数点桁数を確認すること
                    Match enqueue_str_time = Regex.Match(e_str, @"[0-9][0-9]:[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9][0-9][0-9][0-9][0-9]", RegexOptions.RightToLeft);
                    Match dqueue_str_time = Regex.Match(d_str, @"[0-9][0-9]:[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9][0-9][0-9][0-9][0-9]", RegexOptions.RightToLeft);
                    DateTime enqueue_time = DateTime.Parse(enqueue_str_time.Value);
                    DateTime dqueue_time = DateTime.Parse(dqueue_str_time.Value);
                    TimeSpan result = enqueue_time - dqueue_time;
                    time_difference.Add(result.TotalMilliseconds);
                    }
                } else {
                    for(int i = 0; i < enqueue_lines.Count; i++){

                    // 一行取得
                    string e_str = enqueue_lines[i] as string;
                    string d_str = dqueue_lines[i] as string;
                    // ToDo小数点桁数を確認すること
                    Match enqueue_str_time = Regex.Match(e_str, @"[0-9][0-9]:[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9][0-9][0-9][0-9][0-9]", RegexOptions.RightToLeft);
                    Match dqueue_str_time = Regex.Match(d_str, @"[0-9][0-9]:[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9][0-9][0-9][0-9][0-9]", RegexOptions.RightToLeft);
                    DateTime enqueue_time = DateTime.Parse(enqueue_str_time.Value);
                    DateTime dqueue_time = DateTime.Parse(dqueue_str_time.Value);
                    TimeSpan result = enqueue_time - dqueue_time;
                    time_difference.Add(result.TotalMilliseconds);
                    }
                }
                using (StreamWriter outputFile = new StreamWriter(@"C:\Users\y.miyachi\Documents\File_Grep_Tool\msgbox\QUEUE_THROUGHPUT.log"))
                {
                    outputFile.WriteLine(Convert.ToString("デキューとエンキューの処理終了時刻差分を表示しています. 単位はミリ秒です."));
                    for(int j = 0; j < time_difference.Count; j++){
                        outputFile.WriteLine(Convert.ToString(time_difference[j]));
                    }
                }
                Console.WriteLine("処理が終了しました.");
                Console.WriteLine("C:\\Users\\y.miyachi\\Documents\\File_Grep_Tool\\msgbox\\QUEUE_THROUGHPUT.log");
                Console.ReadKey();
        }
    }
}
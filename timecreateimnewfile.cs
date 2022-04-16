using System;
using System.Text.RegularExpressions;
 using System.IO;
 using System.Text;
namespace NHK_File_Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("置換処理をはじめます・・・");
            Console.WriteLine("・・・処理中・・・");
            // 08時～
            DateTime dt = new DateTime(2022, 4, 08, 08, 00, 00);
            string default_time = dt.ToString("HH:mm:ss");
            string result;
            int Ttime_comparison_result = 0;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            foreach (string line in System.IO.File.ReadLines(@"C:\replace_file\subject.log"))
            {
                string temp_line = line;

                // UTC時刻を取得
                Match matches = Regex.Match(line, @"[0-4][0-4]:[0-9][0-9]:[0-9][0-9]");
                if(matches.Success){
                    // 抽出したUTC時刻をJSTに変換する⇒defaultTime
                    DateTime dTime = DateTime.Parse(matches.Value);
                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                    DateTime jst_dtime = System.TimeZoneInfo.ConvertTimeFromUtc(dTime,tzi);
                    result = jst_dtime.ToString("HH:mm:ss");

                    Ttime_comparison_result = default_time.CompareTo(result);

                    if((Ttime_comparison_result == -1) || (Ttime_comparison_result == 0)){
                        DateTime d_default_time = DateTime.Parse(default_time);
                        DateTime d_result = DateTime.Parse(result);
                        TimeSpan d_time_result = d_result- d_default_time;
                        if(d_time_result.TotalSeconds < 5.0){
                        // 更新
                            default_time = result;
                        }else{
                            result = default_time;
                        }
                    }else {
                        result = default_time;
                    }
                } else {
                    result = default_time;
                }
                    // 置換対象箇所を特定する
                    Match JstTime = Regex.Match(line, @"[0-9][0-9]:[0-9][0-9]:[0-9][0-9]");
                    // 置換する
                    temp_line = line.Replace(JstTime.Value, result);

                    temp_line = temp_line + "\r\n";
                    File.AppendAllText(@"C:\replace_file\output.log", temp_line, System.Text.Encoding.GetEncoding("shift_jis"));
            }
            Console.WriteLine("置換終了しました⇒ output.log");
            Console.ReadKey();
        }
    }
}

using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Voice
{
    class ReadCommand
    {
        CommandLineApplication app = new CommandLineApplication(throwOnUnexpectedArg: false)
        {
            // アプリケーション名（ヘルプの出力で使用される）
            Name = "Voice",
        };
        
        public ReadCommand(string[] args)
        {
            Init();
            app.Execute(args);
        }

        private void Init()
        {
            app.OnExecute(() =>
            {
                return 0;
            });
            SetHelp();
            SetReturnJSON();
            SetTalk();

        }
        private void SetHelp()
        {
            // ヘルプ出力のトリガーとなるオプションを指定
            app.HelpOption("-?|-h|--help");
        }
        private void SetReturnJSON()
        {
            app.Command("get", (command) =>
            {
                // 説明（ヘルプの出力で使用される）
                command.Description = "SeikaCenterの情報を返す";
                // コマンドについてのヘルプ出力のトリガーとなるオプションを指定
                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {
                    SeikaData seikaData = new SeikaData();
                    string json = SeikaConnect.Serialize(seikaData);
                    Console.OutputEncoding = Encoding.UTF8;
                    Console.WriteLine(json);
                    return 0;
                });
            });
        }

        private void SetTalk()
        {
            app.Command("talk", (command) =>
            {
                // 説明（ヘルプの出力で使用される）
                command.Description = "SeikaCenterへ発声情報を送る";

                // コマンドについてのヘルプ出力のトリガーとなるオプションを指定
                command.HelpOption("-?|-h|--help");

                var talkArgs = command.Argument("話者名 text", "発話者の登録名とテキスト",true);
                var effectOption = command.Option("-effect",
                  "例:-effect=\"speed,1.0\" -effect=\"pitch,1.3\"",
                  CommandOptionType.MultipleValue);
                var emotionOption = command.Option("-emotion",
                  "例:-emotion=\"喜び,1.0\"",
                  CommandOptionType.MultipleValue);
                var outputOption = command.Option("-o",
                  "出力パス 指定しない場合は発声だけとなる",
                  CommandOptionType.MultipleValue);

                command.OnExecute(() =>
                {
                    //Console.WriteLine(talkArgs.Value);
                    if(talkArgs.Values.Count != 2)
                    {
                        return 0;
                    }

                    SeikaData seikaData = new SeikaData();
                    SeikaStruct seikaStruct = seikaData.GetActor(talkArgs.Values[0]);
                    string text = talkArgs.Values[1];
                    string savePath = "";
                    int cid = seikaStruct.cid;

                    text=text.Replace("<br>", "\r\n").Replace("\r\r", "\r");

                    SeikaPOSTpram pram = new SeikaPOSTpram();
                    pram.TalkText = text;
                    foreach (var value in effectOption.Values)
                    {
                        string[] str = value.Split(',');
                        pram.Effects.Add(str[0],decimal.Parse(str[1]));
                        //Console.WriteLine("-effect= " + value);
                    }
                    foreach (var value in emotionOption.Values)
                    {
                        string[] str = value.Split(',');
                        pram.Emotions.Add(str[0], decimal.Parse(str[1]));
                        //Console.WriteLine("-effect= " + value);
                    }
                    if(outputOption.Value() != null)
                    {
                        savePath = outputOption.Value();
                    }
                    if (savePath != "")
                    {
                        Encoding utf = Encoding.UTF8;
                        StreamWriter writer =
                          new StreamWriter(Path.GetDirectoryName(savePath) +@"/"+ Path.GetFileNameWithoutExtension(savePath) + ".txt", false, utf);
                        writer.WriteLine(text);
                        writer.Close();

                    }

                    SeikaConnect.Talk(@"http://localhost:7180", cid,SeikaConnect.Serialize(pram));
                    //SeikaConnect.Instance().scc.Talk(cid, text, savePath, effects, emotions);
                    //SeikaConnect.Instance().scc.Talk(cid, "", "",new Dictionary<string, decimal>(), new Dictionary<string, decimal>());


                    return 0;
                });
            });
        }

    }
}

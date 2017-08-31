using System.Linq;
using Assets.Code.Common;
using Assets.Code.Net;
using BashDotNet;
using UnityEngine.UI;

namespace Assets.Code.Interface
{
    public class AdministrationConsole : SingletonBehaviour<AdministrationConsole>
    {
        private string Content
        {
            get { return GetComponent<Text>().text; }
            set { GetComponent<Text>().text = value; }
        }

        private string _currentCommand;

        private Library _library;


        protected override void Start()
        {
            base.Start();

            _library = new Library(
                1,
                new Command(
                    "set_speed",
                    new [] {"value"},
                    new Option[0],
                    (args, opts) =>
                    {
                        float speed;
                        Settings.Current.GameSpeed =
                            float.TryParse(args["value"], out speed)
                                ? speed
                                : Settings.Current.GameSpeed;
                    }));
        }


        public void WriteCommand(string text)
        {
            var commands = text.Split('\n');
            commands[0] = _currentCommand + commands[0];
            _currentCommand = commands.Last();

            foreach (var command in commands.Where((c, i) => i < commands.Length - 1))
            {
                _library.TryExecute(command);
                string output;
                Write(NetManager.Current.TryExecute(command, out output) ? output : "Wrong command\n");
            }

            Write(text);
        }

        public void Write(string text)
        {
            Content += text;
        }

        public void RemoveCharacter()
        {
            Content = Content.Substring(0, Content.Length - 1);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace con2.lobby.messages
{

    /// <summary>
    /// Implementation of the protocol used to communicate with the server
    /// https://docs.google.com/document/d/1Emv4Ce7AKEK7Xwwe3QJfFm7377PNbc0GcI2_kTas4uA/edit#
    /// </summary>

    public class Command
    {
        // Requests
        public const string MAKE_GAME = "makeGame";
        public const string REGISTER_PLAYERS = "registerPlayers";

        // Responses
        public const string MESSAGE = "message";
        public const string GAME_CREATED = "gameCreated";
    }

    public enum Code
    {
        make_game_error = 211,

        register_players_success = 220,
        register_players_error = 221,

        quit_game_success = 230,
        quit_game_error = 231,
    }

    public class Base
    {
        public User User;
        public Code Code;
        public string Content;
    }

    public class Game
    {
        public string Id;
        public string Host;
        public List<string> Players;
    }

    public class User
    {
        public string Name;
    }

    public class Players
    {
        // Unfortunately lowercase because JsonConverter literally translates with camel case.
        public string color1;
        public string name1;

        public string color2;
        public string name2;
    }
}
